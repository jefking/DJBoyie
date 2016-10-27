namespace shujaaz.djboyie
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Models;
    using shujaaz.djboyie.Dialogues;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    await Conversation.SendAsync(activity, () => new StoryDialogue());
                    break;
                case ActivityTypes.DeleteUserData:
                    //Delete profile data
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Handle conversation state changes, like members being added and removed
                    // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                    // Not available in all channels
                    break;
                case ActivityTypes.ContactRelationUpdate:
                    // Handle add/remove from contact lists
                    // Activity.From + Activity.Action represent what happened
                    break;
                case ActivityTypes.Typing:
                    // Handle knowing that the user is typing
                    break;
                case ActivityTypes.Ping:
                    break;
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}