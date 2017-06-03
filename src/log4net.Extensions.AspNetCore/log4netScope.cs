using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace log4net.Extensions.AspNetCore
{
    public class log4netScope
    {
        private readonly string _name;
        private readonly object _state;

        public log4netScope(string name, object state)
        {
            _name = name;
            _state = state;
        }

        public log4netScope Parent { get; private set; }

        private static AsyncLocal<log4netScope> _value = new AsyncLocal<log4netScope>();

        public static log4netScope Current
        {
            get { return _value.Value; }
            set { _value.Value = value; }
        }

        public static IDisposable Push(string name, object state)
        {
            var temp = Current;
            Current = new log4netScope(name, state);
            Current.Parent = temp;

            return new DisposableScope();
        }

        private sealed class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }

        public override string ToString()
        {
            return _state?.ToString();
        }
    }
}
