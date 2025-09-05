namespace Lilhelper.Parsing.Tokens {
    public ref struct TokenResult {
        public Token token;
        public Error err;

        public static TokenResult Err(Error outer, Error inner = null) {
            return new TokenResult {
                err = outer.SetInner(inner)
            };
        }

        public bool IsErr => err is not null;

        public bool IsOk => err is null;

        public bool IsNone => token.Length == 0;

        public static implicit operator TokenResult(Token token) => new() {
            token = token
        };
        
        public static implicit operator TokenResult(Error err) => Err(err);
    }
}
