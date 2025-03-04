namespace IdentityService.Exceptions
{
    public class UserAlreadyExistException : BaseException
    {
        public UserAlreadyExistException() : base("Such a user already exists!") { }
    }
}
