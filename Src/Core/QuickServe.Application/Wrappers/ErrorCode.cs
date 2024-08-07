namespace QuickServe.Application.Wrappers
{
    public enum ErrorCode : short
    {
        ModelStateNotValid = 0,
        ModelInvariantInvalid = 1,
        FieldDataInvalid = 2,
        MandatoryField = 3,
        InconsistentData = 4,
        RedundantData = 5,
        EmptyData = 6,
        LongData = 7,
        ShortData = 8,
        DataLengthInvalid = 9,
        BirthdateIsAfterNow = 10,
        RequestedDataNotExist = 11,
        DuplicateData = 12,
        DatabaseCommitException = 13,
        DatabaseCommitNotAffected = 14,
        NotFound = 15,
        ModelIsNull = 16,
        NotHaveAnyChangeInData = 17,
        InvalidOperation = 18,
        ThisDataAlreadyExist = 19,
        TamperedData = 20,
        NotInRange = 21,
        ErrorInApiIdentity = 22,
        AccessDenied = 23,
        ErrorInIdentity = 24,
        Exception = 25,
        LicenseException = 26,
        Unauthorized = 27,
        Duplicate = 28,
        
        TokenExpired = 29,
        InvalidToken = 30,
        AccountLocked = 31,
       
        Forbidden = 32,
        RoleNotAssigned = 33,
       
        ForeignKeyViolation = 34,
        ConstraintViolation = 35,
       
        FileNotFound = 36,
        FileUploadFailed = 37,
        FileFormatInvalid = 38,

        NetworkError = 39,
        Timeout = 40,

        ServiceUnavailable = 41,
        ExternalServiceError = 42,

        BusinessRuleViolation = 43,
        OperationNotAllowed = 44,

        ConcurrencyConflict = 45,
        OptimisticConcurrencyFailure = 46,
    }
}
