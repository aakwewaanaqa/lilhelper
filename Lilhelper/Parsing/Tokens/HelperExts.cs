namespace Lilhelper.Parsing.Tokens {
    public static class HelperExts {
        public delegate TokenPos Alterer(TokenPos pos);

        public static TokenDim Advance(this ref TokenPos self, Alterer alterer) {
            var dim = new TokenDim {
                start = self,
                end   = alterer(self)
            };

            self = dim.end;
            return dim;
        }
    }
}
