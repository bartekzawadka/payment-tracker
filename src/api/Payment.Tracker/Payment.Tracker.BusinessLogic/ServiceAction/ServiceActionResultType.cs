namespace Payment.Tracker.BusinessLogic.ServiceAction
{
    public enum ServiceActionResultType
    {
        None = 0,
        Success = 1,
        Created = 2,
        InvalidDataOrOperation = 100,
        UnauthorizedAccess = 101,
        ForbiddenAccess = 102,
        ObjectNotFound = 103
    }
}