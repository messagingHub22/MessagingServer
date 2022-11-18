using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessagingServer.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagingServer.Data;

namespace MessagingServer.Controllers.Tests
{
    [TestClass()]
    public class MessageDataControllerTests
    {
        [TestMethod()]
        public void GetMessagesTest()
        {
            MessageDataController controller = new MessageDataController(null);
            IEnumerable<MessageData> messages;

            // Task.Delay(5000).ContinueWith(t => Assert.Fail("Did not respond in 5 seconds. Database might be connecting."));

            try
            {
                messages = controller.GetMessages();
            } catch
            {
                Assert.Fail("Failed to initialize database. Check connection string.");
            }

        }


    }
}