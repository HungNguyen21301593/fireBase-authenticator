using FireBaseAuthenticator.Exceptions;

namespace FireBaseAuthenticator.KijijiHelperServices
{
    public class MachineInformationService : IMachineInformationService
    {
        public string GetMachineName()
        {
            return Environment.MachineName;
        }

        public string GetUserName()
        {
            try
            {
                return Environment.UserName;
            }
            catch (Exception e)
            {
                throw new GetDeviceInfoException("Could not get device user name", e);
            }
        }
    }
}
