using System.IO;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Controllers.AnarchyController
{
    [TestFixture]
    public class UpdateActionTests : BaseControllerTests
    {
        const string updatePayload = "fake request body";
        const string actionType = "aFakeAnarchyType";

        [Test]
        public async Task UpdateAction_ReturnsOkResult()
        {
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.AnarchyController(anarchyManager));
            sut.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(updatePayload));

            //Act
            var result = await sut.UpdateAction(actionType);

            //Assert
            result.Should().BeOfType<OkResult>();
        }

        [Test]
        public async Task UpdateAction_PassesPayloadToService()
        {
            //arrange
            var anarchyManager = Substitute.For<IAnarchyManagerNew>();
            var sut = ControllerWithContextBuilder(() => new Anarchy.Controllers.AnarchyController(anarchyManager));
            sut.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(updatePayload));

            //Act
            await sut.UpdateAction(actionType);

            //Assert
            anarchyManager.Received(1).UpdateAction(actionType, updatePayload);
        }
    }
}
