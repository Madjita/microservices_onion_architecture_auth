namespace AuthDAL.Enums;

public enum ErrorType
{
    None = 0,
    Unknown,
    Generic,
    Unhandled,
    ModelState,
    HttpContext,
    Request,
    HttpClient,

    Auth = 1024,
    Permission
}