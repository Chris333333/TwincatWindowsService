using Application.Helpers;
using Domain;
using FluentAssertions;
using System;
using System.Collections.Generic;
using TwinCAT.Ads;
using Xunit;

namespace Application.Tests
{
    /// <summary>
    /// Some tests can be broken if you are not properly connected to PLC, Check if you are logged in to the PLC
    /// </summary>
    public class TwincatConnectorExtensionsTests
    {
        [Fact]
        public void ConnectTwinCatClient_SettingUpCorrectConnectionToPLC_ShouldConnectToTwincatAndReadStateRunning()
        {
            var twincatClient = new TcAdsClient();

            var error = TwincatClientExtensions.ConnectTwinCatClient(twincatClient, Secrets.ExampleAmsNetID, Secrets.ExamplePortNumber);
            var twincatState = twincatClient.ReadState();

            twincatClient.IsConnected.Should().BeTrue();
            twincatState.AdsState.Should().Be(AdsState.Run);
            error.IsError.Should().BeFalse();
            error.Message.Should().BeNull();
        }

        [Fact]
        public void ConnectTwinCatClient_SettingUpWrongConnectionToPLC_ShouldNotConnectShouldReturnErrorAndMessage()
        {
            var twincatClient = new TcAdsClient();

            var error = TwincatClientExtensions.ConnectTwinCatClient(twincatClient, "0", Secrets.ExamplePortNumber);

            twincatClient.IsConnected.Should().BeFalse();
            error.IsError.Should().BeTrue();
            error.Message.Should().Contain("Error while connecting to Twincat");
        }

        [Fact]
        public void ConnectTwinCatClient_SettingUpWithNullClient_ShouldNotConnectShouldReturnErrorAndMessage()
        {
            TcAdsClient twincatClient = null;

            var error = TwincatClientExtensions.ConnectTwinCatClient(twincatClient, Secrets.ExampleAmsNetID, Secrets.ExamplePortNumber);

            error.IsError.Should().BeTrue();
            error.Message.Should().Contain("TwinCat client isn't declared");
        }




        [Theory]
        [InlineData("Global_Variables.iErrorQty")]
        public void SetupTwinCatNotifications_SetingUpCorrectlyTwinCatNotificationWithList_ReturnsNoErrors(string key)
        {
            var twincatClient = new TcAdsClient();
            var error = TwincatClientExtensions.ConnectTwinCatClient(twincatClient, Secrets.ExampleAmsNetID, Secrets.ExamplePortNumber);
            Dictionary<string, TwincatLayoutsDictionaryValues> ExampleLayoutDictionary = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {key, new TwincatLayoutsDictionaryValues(key,TwincatDataTypes.INT) }
            };
            var twincatReadStream = new AdsStream(sizeof(UInt32));


            var errorSetupNotif = TwincatClientExtensions.SetupTwinCatNotifications(ExampleLayoutDictionary, twincatClient, twincatReadStream);

            errorSetupNotif.IsError.Should().BeFalse();
        }

        [Fact]
        public void SetupTwinCatNotifications_SetingUpWithNotConnectedPLC_ReturnsErrors()
        {
            var twincatClient = new TcAdsClient();
            Dictionary<string, TwincatLayoutsDictionaryValues> ExampleLayoutDictionary = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {"Global_Variables.iErrorQty", new TwincatLayoutsDictionaryValues("Global_Variables.iErrorQty",TwincatDataTypes.INT) }
            };
            var twincatReadStream = new AdsStream(sizeof(UInt32));


            var errorSetupNotif = TwincatClientExtensions.SetupTwinCatNotifications(ExampleLayoutDictionary, twincatClient, twincatReadStream);

            errorSetupNotif.IsError.Should().BeTrue();
            errorSetupNotif.Message.Should().Contain("TwinCat client isn't connected");
        }

        [Theory]
        [InlineData("test")]
        [InlineData("a")]
        [InlineData("b")]
        public void SetupTwinCatNotifications_SetingUpWithWrongNotifValues_ReturnsError(string value)
        {
            var twincatClient = new TcAdsClient();
            var error = TwincatClientExtensions.ConnectTwinCatClient(twincatClient, Secrets.ExampleAmsNetID, Secrets.ExamplePortNumber);
            Dictionary<string, TwincatLayoutsDictionaryValues> ExampleLayoutDictionary = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {value, new TwincatLayoutsDictionaryValues(value,TwincatDataTypes.BOOL) }
            };
            var twincatReadStream = new AdsStream(sizeof(UInt32));


            var errorSetupNotif = TwincatClientExtensions.SetupTwinCatNotifications(ExampleLayoutDictionary, twincatClient, twincatReadStream);

            errorSetupNotif.IsError.Should().BeTrue();
            errorSetupNotif.Message.Should().Contain("Error while setting up notifications.");
        }

        [Fact]
        public void SetupTwinCatNotifications_SetingUpWithNotInicializedClient_ReturnsErrors()
        {
            TcAdsClient twincatClient = null;
            Dictionary<string, TwincatLayoutsDictionaryValues> ExampleLayoutDictionary = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {"Global_Variables.iErrorQty", new TwincatLayoutsDictionaryValues("Global_Variables.iErrorQty",TwincatDataTypes.INT) }
            };
            var twincatReadStream = new AdsStream(sizeof(UInt32));


            var errorSetupNotif = TwincatClientExtensions.SetupTwinCatNotifications(ExampleLayoutDictionary, twincatClient, twincatReadStream);

            errorSetupNotif.IsError.Should().BeTrue();
            errorSetupNotif.Message.Should().Contain("TwinCat client isn't declared");
        }

        [Fact]
        public void SetupTwinCatNotifications_SetingUpWithNotInitReadStream_ReturnsErrors()
        {
            TcAdsClient twincatClient = new TcAdsClient();
            var error = TwincatClientExtensions.ConnectTwinCatClient(twincatClient, Secrets.ExampleAmsNetID, Secrets.ExamplePortNumber);
            Dictionary<string, TwincatLayoutsDictionaryValues> ExampleLayoutDictionary = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {"Global_Variables.iErrorQty", new TwincatLayoutsDictionaryValues("Global_Variables.iErrorQty",TwincatDataTypes.BOOL) }
            };
            AdsStream twincatReadStream = null;


            var errorSetupNotif = TwincatClientExtensions.SetupTwinCatNotifications(ExampleLayoutDictionary, twincatClient, twincatReadStream);

            errorSetupNotif.IsError.Should().BeTrue();
            errorSetupNotif.Message.Should().Contain("TwinCat AdsReadStream isn't declared");
        }

        //TwincatReadstreamSwitchTypes

        [Fact]
        public void TwincatReadstreamSwitchTypes_ThereAreNoArguments_ReturnsErrorWithMessage()
        {
            AdsNotificationEventArgs args = null;
            AdsBinaryReader reader = new AdsBinaryReader(new AdsStream());
            Dictionary<string, TwincatLayoutsDictionaryValues> LayoutDirectory = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {"test", new TwincatLayoutsDictionaryValues("testlayout",TwincatDataTypes.BOOL) }
            };

            var error = TwincatClientExtensions.TwincatReadstreamSwitchTypes(args,reader,LayoutDirectory);

            error.ErrorLogDTO.IsError.Should().BeTrue();
            error.ErrorLogDTO.Message.Should().Contain("AdsNotificationsArguments are empty");
        }

        [Fact]
        public void TwincatReadstreamSwitchTypes_ThereAreIsNoReader_ReturnsErrorWithMessage()
        {
            AdsNotificationEventArgs args = new AdsNotificationEventArgs(
                timeStamp: 0,
                userData: null,
                notificationHandle: 0,
                length: 0,
                offset: 0,
                dataStream: new AdsStream()
                );

            AdsBinaryReader reader = null;
            Dictionary<string, TwincatLayoutsDictionaryValues> LayoutDirectory = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {"test", new TwincatLayoutsDictionaryValues("testlayout",TwincatDataTypes.BOOL) }
            };

            var error = TwincatClientExtensions.TwincatReadstreamSwitchTypes(args, reader, LayoutDirectory);

            error.ErrorLogDTO.IsError.Should().BeTrue();
            error.ErrorLogDTO.Message.Should().Contain("AdsBinary Reader isn't setup");
        }

        [Fact]
        public void TwincatReadstreamSwitchTypes_ThereAreIsNoDirectory_ReturnsErrorWithMessage()
        {
            AdsNotificationEventArgs args = new AdsNotificationEventArgs(
                timeStamp: 0,
                userData: null,
                notificationHandle: 0,
                length: 0,
                offset: 0,
                dataStream: new AdsStream()
                );

            AdsBinaryReader reader = new AdsBinaryReader (new AdsStream());
            Dictionary<string, TwincatLayoutsDictionaryValues> LayoutDirectory = null;

            var error = TwincatClientExtensions.TwincatReadstreamSwitchTypes(args, reader, LayoutDirectory);

            error.ErrorLogDTO.IsError.Should().BeTrue();
            error.ErrorLogDTO.Message.Should().Contain("LayoutDirectory is empty");
        }

        [Fact]
        public void TwincatReadstreamSwitchTypes_DirectoryDoesntContainsShearchedValue_ReturnsErrorWithMessage()
        {
            AdsNotificationEventArgs args = new AdsNotificationEventArgs(
                timeStamp: 0,
                userData: "test1",
                notificationHandle: 0,
                length: 0,
                offset: 0,
                dataStream: new AdsStream()
                );

            AdsBinaryReader reader = new AdsBinaryReader(new AdsStream());
            Dictionary<string, TwincatLayoutsDictionaryValues> LayoutDirectory = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {"test", new TwincatLayoutsDictionaryValues("testlayout",TwincatDataTypes.BOOL) }
            };

            var error = TwincatClientExtensions.TwincatReadstreamSwitchTypes(args, reader, LayoutDirectory);

            error.ErrorLogDTO.IsError.Should().BeTrue();
            error.ErrorLogDTO.Message.Should().Contain("Dictionary doesn't contains layout");
        }

        [Fact]
        public void TwincatReadstreamSwitchTypes_NotSupportedDataType_ReturnsErrorWithMessage()
        {
            AdsNotificationEventArgs args = new AdsNotificationEventArgs(
                timeStamp: 0,
                userData: "test",
                notificationHandle: 0,
                length: 0,
                offset: 0,
                dataStream: new AdsStream()
                );

            AdsBinaryReader reader = new AdsBinaryReader(new AdsStream());
            Dictionary<string, TwincatLayoutsDictionaryValues> LayoutDirectory = new Dictionary<string, TwincatLayoutsDictionaryValues>
            {
                {"test", new TwincatLayoutsDictionaryValues("testlayout",TwincatDataTypes.__UXINT) }
            };

            var error = TwincatClientExtensions.TwincatReadstreamSwitchTypes(args, reader, LayoutDirectory);

            error.ErrorLogDTO.IsError.Should().BeTrue();
            error.ErrorLogDTO.Message.Should().Contain("Not supported Twincat Data Type");
        }

    }
}
