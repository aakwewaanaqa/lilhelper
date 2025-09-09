namespace Lilhelper.Parsing {
    public enum ErrorCode : uint {
        Ok                  = 200,
        ExpectationFail     = 400,
        Meaningless         = 401,
        NoChannelValue      = 402,
        ValueFormatMismatch = 403,
        ExceptionOccured    = 500,
    }
}
