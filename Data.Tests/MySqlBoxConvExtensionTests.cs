using Data.Helpers;
using Domain;
using FluentAssertions;
using Xunit;

namespace Data.Tests
{
    public class MySqlBoxConvExtensionTests
    {
        [Fact]
        public void UpdateOneLayoutInDatabase_CallingFuctionWithNoValues_ReturnsError()
        {
            var error = MySqlExtension.UpdateOneLayoutInDatabase("test", null,"NULL");

            error.IsError.Should().BeTrue();
            error.Message.Should().Contain("There are no values to set");
        }


        [Fact]
        public void UpdateOneLayoutInDatabase_CallingFuctionWithEmptyConString_ReturnsError()
        {
            var dto = new LayoutValueNotifDTO("test", "false");
            var error = MySqlExtension.UpdateOneLayoutInDatabase(null, dto, "NULL");

            error.IsError.Should().BeTrue();
            error.Message.Should().Contain("Connection string not set");
        }

        [Fact]
        public void UpdateOneLayoutInDatabase_CallingFuctionWithNoLayoutName_ReturnsError()
        {
            var dto = new LayoutValueNotifDTO(null, "false");
            var error = MySqlExtension.UpdateOneLayoutInDatabase("test", dto, "NULL");

            error.IsError.Should().BeTrue();
            error.Message.Should().Contain("LayoutName is empty");
        }

    }
}
