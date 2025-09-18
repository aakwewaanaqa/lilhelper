using System;

namespace Lilhelper.Async {
    public class ChannelNoValueException : System.Exception {
        public ChannelNoValueException(string msg, Exception inner) : base(msg, inner) {
        }
    }
}
