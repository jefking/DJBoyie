namespace shujaaz.djboyie.Dialogues
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Models;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    public class StoryDialogue : IDialog<object>
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

            switch (story.Task)
            {
                case PersonalStoryTask.Theme:
                    story.Theme = message.Text;
                    story.Task = PersonalStoryTask.Description;
                    break;
                case PersonalStoryTask.Description:
                    story.Content = message.Text;
                    story.Task = PersonalStoryTask.Images;
                    break;
                case PersonalStoryTask.Images:
                    if (null != message.Attachments && 0 < message.Attachments.Count)
                    {
                        story.Images = new string[message.Attachments.Count];
                        for (var i = 0; i < message.Attachments.Count; i++)
                        {
                            story.Images[i] = message.Attachments[i].ContentUrl;
                        }
                        story.Task = PersonalStoryTask.Theme;
                    }
                    break;
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

            switch (story.Task)
            {
                case PersonalStoryTask.Theme:
                    message.Text = "plase add theme";
                    break;
                case PersonalStoryTask.Description:
                    message.Text = "please add description";
                    break;
                case PersonalStoryTask.Images:
                    message.Text = "please add images";
                    break;
            }

            return message;
        }
    }
}