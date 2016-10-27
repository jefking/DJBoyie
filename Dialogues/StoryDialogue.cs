namespace shujaaz.djboyie.Dialogues
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    public class StoryDialogue : IDialog<PersonalStory>
    {
        private const string key = "personalstory";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var story = context.PrivateConversationData.Get<PersonalStory>(key) ?? new PersonalStory();

            var message = await argument;

            if (string.IsNullOrWhiteSpace(story.Content))
            {
                story.Content = message.Text;
            }
            else if (string.IsNullOrWhiteSpace(story.Theme))
            {
                story.Theme = message.Text;
            }
            else if (null == story.Images || 0 == story.Images.Length)
            {

            }

            var replyToConversation = CreateResponse(context, story);

            await context.PostAsync(replyToConversation);
            context.PrivateConversationData.SetValue<PersonalStory>(key, story);

            context.Wait(MessageReceivedAsync);
        }

        private IMessageActivity CreateResponse(IDialogContext context, PersonalStory story)
        {
            var message = context.MakeMessage();
            message.Recipient = message.From;
            message.Type = "message";
            //message.Attachments = new List<Attachment>();

            if (string.IsNullOrWhiteSpace(story.Content))
            {
                message.Text = "please add description";
            }
            else if (string.IsNullOrWhiteSpace(story.Theme))
            {
                message.Text = "plase add theme";
            }
            else if (null == story.Images || 0 == story.Images.Length)
            {
                message.Text = "please add images";
            }

            return message;
        }
    }
}