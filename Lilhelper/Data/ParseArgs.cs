namespace Lilhelper.Data {
    public struct ParseArgs {
        public readonly string columnDelimiter;
        public readonly string rowDelimiter;

        public ParseArgs(string columnDelimiter, string rowDelimiter) {
            this.columnDelimiter = columnDelimiter;
            this.rowDelimiter    = rowDelimiter;
        }
    }
}
