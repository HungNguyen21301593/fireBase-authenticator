namespace FireBaseAuthenticator.Exceptions
{
    public class RePostLimitExceedException : Exception
    {
        public RePostLimitExceedException(string message, Exception? exception = null) : base(message, exception)
        {

        }
    }
}
