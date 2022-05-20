using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Lachee.Utilities
{
    public class Lazy<T> where T : Component
    {
        private Component parent;
        private bool _initialized;
        private T _component;

        public Lazy(Component parent) {
            this.parent = parent;
        }

        public T Value
        {
            get
            {
                if (_initialized) return _component;
                _component = parent.GetComponent<T>();
                _initialized = true;
                return _component;
            }
            set
            {
                _component = value;
                _initialized = true;
            }
        }
    }
}
