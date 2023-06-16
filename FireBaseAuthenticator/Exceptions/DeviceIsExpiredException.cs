namespace FireBaseAuthenticator.Exceptions
{
    public class DeviceIsExpiredException : Exception
    {
        public DeviceIsExpiredException(string message, Exception? exception = null) : base(message, exception)
        {

        }
    }
}
