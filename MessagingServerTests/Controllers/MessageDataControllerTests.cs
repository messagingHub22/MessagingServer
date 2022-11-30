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
            Assert.AreEqual(messages.Count(), 2);
        }

        [TestMethod()]
        public void GetMessagesForUserTest()
        {
            MessageDataController controller = new MessageDataController(null);
            controller.SetCustomReader(MockMessageData());

            IEnumerable<MessageData> messages = controller.GetMessagesForUser("x");

            Assert.IsNotNull(messages);
            Assert.AreEqual(messages.ElementAt(0).Id, 0);
            Assert.AreEqual(messages.ElementAt(0).MessageRead, false);
            Assert.AreEqual(messages.ElementAt(0).Content, "A");
            Assert.AreEqual(messages.ElementAt(0).MessageCategory, "B");
            Assert.AreEqual(messages.ElementAt(0).MessageUser, "C");
            Assert.AreEqual(messages.Count(), 2);
        }

        [TestMethod()]
        public void GetGroupsTest()
        {
            MessageDataController controller = new MessageDataController(null);
            controller.SetCustomReader(MockStringData("GroupName"));

            IEnumerable<String> groups = controller.GetGroups();

            Assert.IsNotNull(groups);
            Assert.AreEqual(groups.ElementAt(0), "string");
            Assert.AreEqual(groups.Count(), 2);
        }

        [TestMethod()]
        public void GetGroupMembersTest()
        {
            MessageDataController controller = new MessageDataController(null);
            controller.SetCustomReader(MockStringData("MemberName"));

            IEnumerable<String> groupMembers = controller.GetGroupMembers("x");

            Assert.IsNotNull(groupMembers);
            Assert.AreEqual(groupMembers.ElementAt(0), "string");
            Assert.AreEqual(groupMembers.Count(), 2);
        }

        [TestMethod()]
        public void GetUserMessagesTest()
        {
            MessageDataController controller = new MessageDataController(null);
            controller.SetCustomReader(MockUserMessageData());

            IEnumerable<MessageUser> messages = controller.GetUserMessages("x", "y");

            Assert.IsNotNull(messages);
            Assert.AreEqual(messages.ElementAt(0).Id, 0);
            Assert.AreEqual(messages.ElementAt(0).Content, "C");
            Assert.AreEqual(messages.ElementAt(0).MessageFrom, "x");
            Assert.AreEqual(messages.ElementAt(0).MessageTo, "y");
            Assert.AreEqual(messages.Count(), 2);
        }

        [TestMethod()]
        public void GetMessagedUsersTest()
        {
            MessageDataController controller = new MessageDataController(null);
            controller.SetCustomReader(MockStringData("UserName"));

            IEnumerable<String> users = controller.GetMessagedUsers("x");

            Assert.IsNotNull(users);
            Assert.AreEqual(users.ElementAt(0), "string");
            Assert.AreEqual(users.Count(), 2);
        }

        [TestMethod()]
        public void SendMessageTest()
        {
            MessageDataReader.IsTesting = true;

            MessageDataController controller = new MessageDataController(null);
            controller.SendMessage("2000-01-01", "testV2", "Testing", "Tester");

            IEnumerable<MessageData> messages = controller.GetMessagesForUser("Tester");

            MessageDataReader.IsTesting = false;

            Assert.IsNotNull(messages);
            Assert.AreEqual(messages.ElementAt(0).Id, 1);
            Assert.AreEqual(messages.ElementAt(0).Content, "testV2");
            Assert.AreEqual(messages.ElementAt(0).MessageCategory, "Testing");
            Assert.AreEqual(messages.ElementAt(0).MessageUser, "Tester");
            Assert.AreEqual(messages.Count(), 1);
        }

        // Mock IDataReader with message data
        private IDataReader MockMessageData()
        {
            var moq = new Mock<IDataReader>();

            moq.SetupSequence(m => m.Read())
                .Returns(true)
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

        // Mock IDataReader with message user
        private IDataReader MockUserMessageData()
        {
            var moq = new Mock<IDataReader>();

            moq.SetupSequence(m => m.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);

            moq.SetupGet<object>(x => x["Id"]).Returns((UInt32)0);
            moq.SetupGet<object>(x => x["SentTime"]).Returns(DateTime.Now);
            moq.SetupGet<object>(x => x["Content"]).Returns("C");
            moq.SetupGet<object>(x => x["MessageFrom"]).Returns("x");
            moq.SetupGet<object>(x => x["MessageTo"]).Returns("y");

            return moq.Object;
        }

        // Mock IDataReader with column
        private IDataReader MockStringData(String Column)
        {
            var moq = new Mock<IDataReader>();

            moq.SetupSequence(m => m.Read())
                .Returns(true)
                .Returns(true)
                .Returns(false);

            moq.SetupGet<object>(x => x[Column]).Returns("string");

            return moq.Object;
        }
    }
}