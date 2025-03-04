namespace IdentityService.Exceptions
{
    public class RefreshTokenShouldNotBeExpiredException : BaseException
    {
        public RefreshTokenShouldNotBeExpiredException() : base("Your session has expired. Please log in again.") { }
    }
}
