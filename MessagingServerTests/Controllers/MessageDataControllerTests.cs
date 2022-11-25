using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessagingServer.Data;
using System.Data;
using Moq;

namespace MessagingServer.Controllers.Tests
{
    [TestClass()]
    public class MessageDataControllerTests
    {
        [TestMethod()]
        public void GetMessagesTest()
        {
            MessageDataController controller = new MessageDataController(null);
            controller.SetCustomReader(MockMessageData());

            IEnumerable<MessageData> messages = controller.GetMessages();

            Assert.IsNotNull(messages);
            Assert.AreEqual(messages.ElementAt(0).Id, 0);
            Assert.AreEqual(messages.ElementAt(0).MessageRead, false);
            Assert.AreEqual(messages.ElementAt(0).Content, "A");
            Assert.AreEqual(messages.ElementAt(0).MessageCategory, "B");
            Assert.AreEqual(messages.ElementAt(0).MessageUser, "C");
        }

        private IDataReader MockMessageData()
        {
            var moq = new Mock<IDataReader>();

            moq.SetupSequence(m => m.Read())
                .Returns(true)
                .Returns(false);

            moq.SetupGet<object>(x => x["Id"]).Returns((UInt32)0);
            moq.SetupGet<object>(x => x["SentTime"]).Returns(DateTime.Now);
            moq.SetupGet<object>(x => x["MessageRead"]).Returns((UInt64)0);
            moq.SetupGet<object>(x => x["Content"]).Returns("A");
            moq.SetupGet<object>(x => x["MessageCategory"]).Returns("B");
            moq.SetupGet<object>(x => x["MessageUser"]).Returns("C");

            return moq.Object;
        }
    }
}