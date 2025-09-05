using System;
using System.Text;

namespace Lilhelper.Parsing {
    public class Error {
        private readonly ErrorCode code;
        private readonly string    msg;
        private readonly IPosition pos;
        private          Error     inner;

        public Error SetInner(Error inner) {
            this.inner = inner;
            return this;
        }
        
        public Error(ErrorCode code, string msg, IPosition pos, Error inner = null) {
            this.code  = code;
            this.msg   = msg;
            this.pos   = pos;
            this.inner = inner;
        }

        public static Error ExpectationFail(string supposed, IPosition pos, Error inner = null) =>
            new(
            ErrorCode.ExpectationFail,
            $"supposed to be {supposed} but not met",
            pos,
            inner
            );

        public static Error Exception(Exception ex, IPosition pos) {
            return new Error(
                ErrorCode.ExceptionOccured,
                ex.Message,
                pos
            );
        }
        
        public override string ToString() {
            return inner is null ? $"{pos} : {msg}" : $"{pos} : {msg}\n{inner}";
        }

        public static implicit operator string(Error error) => error.ToString();
    }
}
