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
                    var message = activity.CreateReply("hustle deleted");
                    var sc = activity.GetStateClient();
                    var userData = sc.BotState.GetPrivateConversationData(message.ChannelId, message.Conversation.Id, message.From.Id);
                    // Set BotUserData
                    userData.SetProperty<PersonalStory>(StoryDialogue.key, new PersonalStory());

                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    await connector.Conversations.ReplyToActivityAsync(message);
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