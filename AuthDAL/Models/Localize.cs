namespace AuthDAL.Models;

public static class Localize
{
    public enum Error
    {
        JsonWebTokenExpired,
        JsonWebTokenNotFound,
        JsonWebTokenNotProvided,
        JsonWebTokenIdRetrievalFailed,
        AccessTokenNotProvided,
        DependencyInjectionFailed,
        HandledExceptionHttpResponseException,
        HandledExceptionCustomException,
        HandledExceptionContactSystemAdministrator,
        UserNotFoundOrHttpContextMissingClaims,
        SignalRConnectionExpired,
        UserNotFoundOrWrongCredentials,
        RequestAbortedOrCancelled
    }

    public static class Text
    {
        #region SignalRHub

        public static string SignalRHubGroupKey => "#GroupKey:UserId<{0}>";
        // public static string SignalRHubContextKey => "#ContextKey:ConnectionId<{0}>:ExpiresAt<{1}>";

        #endregion
    }

    public enum Warning
    {
        XssVulnerable,
        UserNotFoundSkipped,
        DbContextNoTransactionInProgress,
        DbContextAnotherTransactionInProgress
    }
}