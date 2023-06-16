namespace FireBaseAuthenticator.Exceptions
{
    public class GetDeviceInfoException : Exception
    {
        public GetDeviceInfoException(string message, Exception? exception = null) : base(message, exception)
        {

        }
    }
}
