using Microsoft.Bot.Connector;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace shujaaz.djboyie
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                // calculate something for us to return
                var length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                var reply = $"Thanks for: {activity.Text}";

                if (null != activity.Attachments)
                {
                    foreach (var attachment in activity.Attachments)
                    {
                        reply += $"Thanks for this: {attachment.ContentUrl}";
                    }
                }

                var acc = activity.CreateReply(reply);

                    //acc.Attachments.Add(new Attachment()
                    //{
                    //    ContentUrl = "https://upload.wikimedia.org/wikipedia/en/a/a6/Bender_Rodriguez.png",
                    //    ContentType = "image/png",
                    //    Name = "Bender_Rodriguez.png"
                    //});

                await connector.Conversations.ReplyToActivityAsync(acc);
            }
            else if (activity.Type == ActionTypes.ImBack)
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var reply = $"Welcome back.";
                var acc = activity.CreateReply(reply);
                await connector.Conversations.ReplyToActivityAsync(acc);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            switch (message.Type)
            {
                case ActivityTypes.DeleteUserData:
                    // Implement user deletion here
                    // If we handle user deletion, return a real message
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                    message.Action
                    break;
                case ActivityTypes.Typing:
                    // Handle knowing that the user is typing
                    break;
                case ActivityTypes.Ping:
                    break;
            }
            
            return null;
        }
    }
}