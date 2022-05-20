using System;
using UnityEngine;

namespace Lachee.Utilities
{
    public class Logger : ILogHandler
    {
        public Logger Parent { get; }
        public string Tag { get; }

        public Logger(string tag)
        {
            Tag = $"[{tag}]";
        }

        public Logger(Logger parent, string tag) : this(parent.Tag + tag)
        {
            Parent = parent;
        }

        public void Info(string format, params object[] args)
            => Info(format, null, args);
        public void Info(string format, UnityEngine.Object context, params object[] args)
            => LogFormat(LogType.Log, context, format, args);
        public void Info(object message)
            => Info(message, null);
        public void Info(object message, UnityEngine.Object context)
            => LogFormat(LogType.Log, context, message.ToString());

        public void Warning(string format, params object[] args)
            => Warning(format, null, args);
        public void Warning(string format, UnityEngine.Object context, params object[] args)
            => LogFormat(LogType.Warning, context, format, args);
        public void Warning(object message)
            => Warning(message, null);
        public void Warning(object message, UnityEngine.Object context)
            => LogFormat(LogType.Warning, context, message.ToString());

        public void Error(string format, params object[] args)
            => Error(format, null, args);
        public void Error(string format, UnityEngine.Object context, params object[] args)
            => LogFormat(LogType.Error, context, format, args);
        public void Error(object message)
            => Error(message, null);
        public void Error(object message, UnityEngine.Object context)
            => LogFormat(LogType.Error, context, message.ToString());

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
#if !SILENT
            Debug.unityLogger.LogFormat(logType, context, Tag + " " + format, args);
#endif
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
#if !SILENT
            Debug.unityLogger.LogException(exception, context);
#endif
        }


    }
}
