using FireBaseAuthenticator.KijijiHelperServices;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using FireBaseAuthenticator.Exceptions;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace FireBaseAuthenticatorTest
{
    public class Tests
    {
        public Mock<IMachineInformationService> machineInformationServiceMock { get; set; }
        public Mock<IFireBaseSettingService> fireBaseSettingServiceMock { get; set; }
        public Mock<IFireBaseLoggingService> fireBaseLoggingServiceMock { get; set; }
        private readonly string undefinedDeviceName = "PCName_unknown";
        private readonly string TestDeviceNameInvalid = "TestPCName_Invalid";
        private readonly string TestDeviceNameValid = "TestPCName_Valid";
        private readonly string TestDeviceNameExpired = "TestDeviceName_Expired";
        private readonly string TestDeviceNamePostLimitExceed = "TestDeviceName_PostLimitExceed";
        private readonly string TestDeviceNameVip = "TestPCName_Vip";
        private readonly string TestDeviceName_ToBeUpdated = "TestDeviceName_ToBeUpdated";

        private readonly string v3TestUrl = "https://subscriptions-e1198-default-rtdb.firebaseio.com/v3-test/v3";
        private readonly string v3TestHistoryUtl = "https://subscriptions-e1198-default-rtdb.firebaseio.com/v3-test/history";

        [SetUp]
        public void Setup()
        {
            machineInformationServiceMock = new Mock<IMachineInformationService>();
            fireBaseSettingServiceMock = new Mock<IFireBaseSettingService>();
            fireBaseLoggingServiceMock = new Mock<IFireBaseLoggingService>();
        }

        [Test]
        public async Task ShouldThrowExceptionIfCouldNotGetMachineName()
        {
            // Arrange
            var target = CreateTarget(machineInformationServiceMock.Object, fireBaseSettingServiceMock.Object, fireBaseLoggingServiceMock.Object);
            machineInformationServiceMock.Setup(m => m.GetMachineName()).Throws(new GetDeviceInfoException("error"));
            // Act
            await Assert.ThrowsExceptionAsync<GetDeviceInfoException>(async () => await target.VerifyDevice());
            fireBaseLoggingServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task ShouldThrowExceptionIfCouldNotGetDeviceInfoFromFireBase()
        {
            // Arrange
            var target = CreateTarget(machineInformationServiceMock.Object, fireBaseSettingServiceMock.Object, fireBaseLoggingServiceMock.Object);
            machineInformationServiceMock.Setup(m => m.GetMachineName()).Returns(undefinedDeviceName);
            fireBaseSettingServiceMock.Setup(m => m.GetV3Url()).Returns(v3TestUrl);
            // Act
            await Assert.ThrowsExceptionAsync<GetDeviceInfoException>(async () => await target.VerifyDevice());
            fireBaseLoggingServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task ShouldThrowExceptionIfDeviceInfoFromFireBaseIsInvalid()
        {
            // Arrange
            var target = CreateTarget(machineInformationServiceMock.Object, fireBaseSettingServiceMock.Object, fireBaseLoggingServiceMock.Object);
            machineInformationServiceMock.Setup(m => m.GetMachineName()).Returns(TestDeviceNameInvalid);
            fireBaseSettingServiceMock.Setup(m => m.GetV3Url()).Returns(v3TestUrl);
            // Act
            await Assert.ThrowsExceptionAsync<GetDeviceInfoException>(async () => await target.VerifyDevice());
            fireBaseLoggingServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Should_ThrowDeviceIsExpiredException_When_DeviceIsANormalAccountAndExpiredDateBeforeToday()
        {
            // Arrange
            var target = CreateTarget(machineInformationServiceMock.Object, fireBaseSettingServiceMock.Object, fireBaseLoggingServiceMock.Object);
            machineInformationServiceMock.Setup(m => m.GetMachineName()).Returns(TestDeviceNameExpired);
            // Act
            await Assert.ThrowsExceptionAsync<DeviceIsExpiredException>(async () => await target.VerifyDevice());
            fireBaseLoggingServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Should_ThrowRePostLimitExceedException_When_DeviceIsANormalAccountAndRePostLimitIsZero()
        {
            // Arrange
            var target = CreateTarget(machineInformationServiceMock.Object, fireBaseSettingServiceMock.Object, fireBaseLoggingServiceMock.Object);
            machineInformationServiceMock.Setup(m => m.GetMachineName()).Returns(TestDeviceNamePostLimitExceed);
            // Act
            await Assert.ThrowsExceptionAsync<RePostLimitExceedException>(async () => await target.VerifyDevice());
            fireBaseLoggingServiceMock.Verify(l => l.LogError(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task ShouldPassNormalIfDeviceIsVip()
        {
            // Arrange
            var target = CreateTarget(machineInformationServiceMock.Object, fireBaseSettingServiceMock.Object, fireBaseLoggingServiceMock.Object);
            machineInformationServiceMock.Setup(m => m.GetMachineName()).Returns(TestDeviceNameVip);
            // Act
            await target.VerifyDevice();
            fireBaseLoggingServiceMock.Verify(l => l.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Should_Pass_When_DeviceIsANormalAccount_And_RePostLimitIsGreaterZero_And_NotExpired()
        {
            // Arrange
            var target = CreateTarget(machineInformationServiceMock.Object, fireBaseSettingServiceMock.Object, fireBaseLoggingServiceMock.Object);
            machineInformationServiceMock.Setup(m => m.GetMachineName()).Returns(TestDeviceNameValid);
            // Act
            await target.VerifyDevice();
            fireBaseLoggingServiceMock.Verify(l => l.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Test]
        public async Task Should_BeAbleToUpdatePostLimit()
        {
            // Arrange
            var target = CreateTarget(machineInformationServiceMock.Object, fireBaseSettingServiceMock.Object, fireBaseLoggingServiceMock.Object);
            machineInformationServiceMock.Setup(m => m.GetMachineName()).Returns(TestDeviceName_ToBeUpdated);
            // Act
            var currentInfo = await target.GetDeviceInformation();
            var newPostLimit = currentInfo.RemainingPostLimit + 1;
            await target.UpdateNumberOfAllowAds(newPostLimit);
            var newInfo = await target.GetDeviceInformation();
            //Assert
            Assert.AreEqual(newInfo.RemainingPostLimit, newPostLimit);
            fireBaseLoggingServiceMock.Verify(l => l.LogInfo(It.IsAny<string>()), Times.AtLeastOnce);
        }

        private IAuthenticatorService CreateTarget(IMachineInformationService informationService, IFireBaseSettingService fireBaseSettingService, IFireBaseLoggingService fireBaseLoggingService)
        {
            fireBaseSettingServiceMock.Setup(m => m.GetV3Url()).Returns(v3TestUrl);
            fireBaseSettingServiceMock.Setup(m => m.GetHistoryUrl()).Returns(v3TestHistoryUtl);
            fireBaseSettingServiceMock.Setup(m => m.GetVipAccountPostNumber()).Returns(99999);
            return new AuthenticatorService(informationService, fireBaseSettingService, fireBaseLoggingService);
        }
    }
}