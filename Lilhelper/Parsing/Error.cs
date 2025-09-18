using System;

namespace Lilhelper.Parsing {
    public class Error : IError {
        private readonly ErrorCode code;
        private readonly string    msg;
        private readonly IPosition pos;
        private          Error     inner;

        public ErrorCode Code  => code;
        public string    Msg   => msg;
        public IPosition Pos   => pos;
        public Error     Inner => inner;

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

        public static Error NoChannelValue(string supposed, IPosition pos, Error inner = null) =>
            new(
                ErrorCode.NoChannelValue,
                $"supposed have value of a chanel {supposed} but no",
                pos,
                inner
            );

        public static Error ValueFormatMismatch(string supposed, IPosition pos, Error inner = null) =>
            new(
                ErrorCode.ValueFormatMismatch,
                $"value format supposed to be {supposed} not match",
                pos,
                inner
            );

        public static Error Exception(Exception ex, IPosition pos) {
            return new(
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
