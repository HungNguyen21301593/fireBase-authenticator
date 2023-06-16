using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireBaseAuthenticator.KijijiHelperServices;
using FireBaseAuthenticator.Model;

namespace FireBaseAuthenticator.Extensions
{
    public  static class V3Extensions
    {
        public static bool IsNotExpired(this V3 deviceInfo)
        {
            return deviceInfo.ExpiredDate > DateTime.UtcNow;
        }

        public static bool IsVipDevice(this V3 deviceInfo, int? vipAccountPostNumber = null)
        {
            vipAccountPostNumber ??= new FireBaseSettingService().VipAccountPostNumber;
            return deviceInfo.RemainingPostLimit == vipAccountPostNumber;
        }

        public static bool IsRePostLimitIsValid(this V3 deviceInfo)
        {
            return deviceInfo.RemainingPostLimit > 0;
        }

        public static bool IsVerified(this V3 deviceInfo)
        {
            return deviceInfo.IsVipDevice() || (deviceInfo.IsNotExpired() && deviceInfo.IsRePostLimitIsValid());
        }
    }
}
