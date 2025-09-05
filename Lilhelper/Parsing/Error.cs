using System.Text;

namespace Lilhelper.Parsing {
    public class Error {
        private readonly string    msg;
        private readonly IPosition pos;
        private readonly Error     inner;

        public Error(string msg, IPosition pos, Error inner = null) {
            this.msg = msg;
        }

        public override string ToString() {
            return inner is null ? $"{pos} : {msg}" : $"{pos} : {msg}\n{inner}";
        }

        public static implicit operator string(Error error) => error.ToString();
    }
}
