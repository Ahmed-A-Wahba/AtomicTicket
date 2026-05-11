namespace AtomicTicket.SharedKernel.Results;

public enum ErrorType
{
    Failure,
    Unexpected,
    Validation,
    Conflict,
    NotFound,
    Unauthorized,
    Forbidden,
    InvalidCredentials
}
