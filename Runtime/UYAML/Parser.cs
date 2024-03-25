using System.Collections.Generic;

namespace Lachee.UYAML
{
    /// <summary>
    /// Simple Parser for Unity YAML files. Able to produce basic tree structures, it is suitable for raw manipulation of the data but not serialization.
    /// </summary>
    public sealed class Parser
    {
        internal const string COMPONENT_HEADER = "--- !u!";

        private IUPropertyCollection _curObject;
        private UProperty _curProperty;
        private Stack<IUPropertyCollection> _objects;

        private int _spt;
        private int _indentLevel;
        private int _prevIndentLevel;
        private int _charPos = 0;

        private Parser()
        {
            _spt = 0;
            _objects = new Stack<IUPropertyCollection>(10);
            Reset();
        }

        /// <summary>Resets the state of the parser</summary>
        private void Reset()
        {
            _charPos = 0;
            _curObject = null;
            _curProperty = null;
            _objects.Clear();
            _indentLevel = 0;
            _prevIndentLevel = 0;
        }

        /// <summary>Parses the given UYAML content</summary>
        public static List<UComponent> Parse(string content)
        {
            int offset;
            int nextOffset;
            string block;

            // Get to first chunk
            offset = content.IndexOf(COMPONENT_HEADER);
            if (offset == -1)
                throw new System.InvalidOperationException("There was no blocks found");

            List<UComponent> components = new List<UComponent>();
            Parser parser = new Parser();

            do
            {
                nextOffset = content.IndexOf(COMPONENT_HEADER, offset + COMPONENT_HEADER.Length);
                if (nextOffset == -1)
                    block = content.Substring(offset);
                else
                    block = content.Substring(offset, nextOffset - offset - 1);

                var component = parser.ParseComponent(block);
                components.Add(component);

                offset = nextOffset;
            } while (offset >= 0);

            return components;
        }

        private UComponent ParseComponent(string content)
        {
            Reset();
            int offset;
            int nextOffset;
            int objLevel = 0;
            int arrLevel = 0;
            string line;

            // Get to the first line
            offset = content.IndexOf(COMPONENT_HEADER);
            if (offset == -1)
                throw new System.InvalidOperationException("The block is missing the content header");

            do
            {
                nextOffset = -1;
                for (int i = offset; i < content.Length; i++)
                {
                    char c = content[i];
                    if (c == '{')
                    {
                        objLevel++;
                    }
                    else if (c == '}')
                    {
                        objLevel--;
                    }
                    else if (c == '[')
                    {
                        arrLevel++;
                    }
                    else if (c == ']')
                    {
                        arrLevel--;
                    }
                    else if (c == '\n' && objLevel == 0 && arrLevel == 0)
                    {
                        nextOffset = i + 1;
                        break;
                    }

                    // Ensure we havnt closed more than we opened
#if UNITY_5_3_OR_NEWER
                    UnityEngine.Debug.Assert(objLevel >= 0 && arrLevel >= 0);
#else
                    System.Diagnostics.Debug.Assert(objLevel >= 0 && arrLevel >= 0);
#endif
                }

                if (nextOffset <= 0) line = content.Substring(offset);
                else line = content.Substring(offset, nextOffset - offset - 1);

                ParseLine(line);

                _charPos = offset = nextOffset;
            } while (offset > 0);

            // Ensure all our objects closed
#if UNITY_5_3_OR_NEWER
            UnityEngine.Debug.Assert(objLevel == 0 && arrLevel == 0);
#else
            System.Diagnostics.Debug.Assert(objLevel == 0 && arrLevel == 0);
#endif

            while (_objects.TryPop(out var node))
            {
                if (node is UComponent comp)
                    return comp;
            }
            return null;
        }

        private void ParseLine(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return;

            string line;
            bool isArrayEntry = false;

            _prevIndentLevel = _indentLevel;
            _indentLevel = Tabulate(content, out line);
            if (_spt < 1)  // Update the indentation level based of the first item we meet
            {
                _spt = _indentLevel;
                _indentLevel = Tabulate(content, out line);
            }


            // No block yet, expecting a new def
            if (_curObject == null)
            {
                if (!line.StartsWith(COMPONENT_HEADER))
                    throw new ParseException($"Expecting a new block, but got '{line}'", _charPos);

                string[] segs = line.Split(' ');
                if (segs.Length != 3)
                    throw new ParseException($"Expecting 3 parts in the block header", _charPos);

                var identifier = (ClassIdentifier)int.Parse(segs[1].Substring(3));
                _curObject = new UComponent()
                {
                    classID = identifier,
                    fileID = long.Parse(segs[2].Substring(1)),
                    level = _indentLevel,
                };
                return;
            }

            // Start of Array
            if (line[0] == '-')
            {
                line = line.Trim('-', '\t', ' ');
                _indentLevel++;
                isArrayEntry = true;
            }

            // Start of Object
            int indentDiff = _indentLevel - _prevIndentLevel;
            if (indentDiff > 0)    // Something wrong with this logic when adding item,s to array
            {
                if (isArrayEntry)
                {
                    // We have increased our indentation and we have started with a -,
                    // that means we starting an array and the parent property needs to be converted 
                    // into an array.

                    // We need to convert the current property to an array
                    var arr = new UArray() { level = _indentLevel };
                    _objects.Push(_curObject);
                    _curProperty.value = arr;
                    _curObject = arr;
                }
                else
                {
                    // TODO: Check if the previous PROPERTY (_curProperty) value is in a position to create a sub object.
                    // This is only ever allowed in an array when its filled
                    if (!(_curObject is UComponent) && _curProperty.value is UValue val && val.value.Length > 0)
                    {
                        // We are appending to the previosu value.
                        // Once added we stop all processing of this node
                        // FIXME: The fix for multiple levels is lower down. This is a pretty messy way of doing this.
                        val.value += ' ' + line.Trim();
                        return;
                    }
                    else
                    {
                        // FIXME: The below statement is wrong. We only create a new object when incrementing when the previous property has no value.

                        // Our indentation level has increased so we are making a new object
                        var obj = new UObject() { level = _indentLevel };
                        _objects.Push(_curObject);
                        _curProperty.value = obj;
                        _curObject = obj;
                    }
                }
            }
            else if (indentDiff < 0)
            {

                // FIXME: This can cause issue when we pop the last item off the stack.
                // FIXME: The stack is a collection of property storage, it doesn't have a concept of "level" so this is bad design.
                bool popped = false;
                while (_curObject is UNode node && node.level > _indentLevel)
                {
                    _curObject = _objects.Pop();
                    popped = true;
                }

                if (!popped) // We only pop down to the next level if we failed to pop anything above us.
                {
                    while (_objects.Peek() is UNode nNode && nNode.level == _indentLevel)
                        _curObject = _objects.Pop();
                }
            }
            else if (indentDiff == 0)
            {
                // Handle the edge case described earlier for when multiple lines are given
                // FIXME: this is some pretty jank logic. Not entirely sure if it will hold
                if (_curProperty != null && _curProperty.value is UValue val && val.value.Length > 0 && val.level == _indentLevel - 1)
                {
                    val.value += ' ' + line.Trim();
                    return;
                }
            }
            else if (indentDiff != 0)
            {
                throw new ParseException($"Indentation grew/shrunk too rapidly. Changed by {indentDiff}", _charPos);
            }

            // Seperate the parts
            if (!IsKeyValue(line))
            {
                if (_curObject is UArray arr)
                {
                    var value = ParseValue(line);
                    value.level = _indentLevel;
                    arr.Add(value);
                }
                else
                {
                    throw new ParseException($"Cannot add key-less values to an object", _charPos);
                }
            }
            else
            {
                string[] parts = line.Split(':', 2);
                if (parts.Length != 2)
                    throw new ParseException($"Cannot find property name in '{line}'", _charPos);

                _curProperty = new UProperty(parts[0].Trim(), ParseValue(parts[1]));
                _curProperty.value.level = _indentLevel;
                if (isArrayEntry)
                {
                    if (!(_curObject is UArray) && _objects.Peek() is UArray)
                        _curObject = _objects.Pop();

                    if (_curObject is UArray arr)
                    {
                        // Create a new object to put this property into
                        var obj = new UObject() { level = _indentLevel };
                        _objects.Push(_curObject);
                        _curObject = obj;
                        arr.Add(obj);
                    }
                    else
                    {
                        throw new ParseException($"Adding a new array item, but could not get an array to put it in", _charPos);
                    }
                }

                // Attempt to push the current property into the current object
                //if (_curObject is UArray) // The items need to be added to the previous array item
                //    throw new ParseException($"Cannot add the named property '{_curProperty.name}' to an array", _charPos);
                if (_curProperty != null && !_curObject.Add(_curProperty))
                    throw new ParseException($"Failed to add the property '{_curProperty.name}'", _charPos);
            }
        }

        private UNode ParseValue(string value)
        {
            string[] parts;
            string content = value.Trim();

            if (content.Length == 0)
                return new UValue();

            if (content[0] == '{' && content[content.Length - 1] == '}')
            {
                UObject objValue = new UObject();
                if (content.Length > 2)
                {
                    parts = content.Split(',');
                    foreach (var part in parts)
                    {
                        string[] sParts = part.Trim('{', '}', ' ').Split(':', 2);
                        if (sParts.Length != 2)
                            throw new ParseException($"Cannot parse non-key values inside a inline object", _charPos);
                        objValue.Add(new UProperty(sParts[0].Trim(), ParseValue(sParts[1])));
                    }
                }
                return objValue;
            }

            if (content[0] == '[' && content[content.Length - 1] == ']')
            {
                UArray arrayValue = new UArray();
                if (content.Length > 2)
                {
                    parts = content.Split(',');
                    foreach (var part in parts)
                        arrayValue.Add(new UValue(part.Trim('[', ']', ' ')));
                }
                return arrayValue;
            }

            if (content[0] == '"' && content[content.Length - 1] == '"')
                content = content.Trim('"').Replace("\\\"", "\"");

            return new UValue(content);
        }

        private bool IsKeyValue(string line)
            => line.IndexOf(':') >= 0 && line[0] != '{' && line[0] != '[';

        private int Tabulate(string content, out string line)
        {
            int spaces;
            for (spaces = 0; spaces < content.Length; spaces++)
            {
                if (content[spaces] != ' ' && content[spaces] != '\t')
                    break;
            }

            line = content.TrimStart(' ', '\t', '\n', '\r');
#if UNITY_5_3_OR_NEWER
            return spaces / UnityEngine.Mathf.Max(_spt, 1);
#else
            return spaces / Math.Max(_spt, 1);
#endif
        }
    }

    /// <summary>Represents errors that occure during parsing</summary>
    public sealed class ParseException : System.Exception
    {
        public int Position { get; }
        public ParseException(string message, int position)
            : base($"{message}. (char @{position})")
        {
            Position = position;
        }
    }

}
