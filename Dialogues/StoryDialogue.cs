namespace shujaaz.djboyie.Dialogues
{
    using King.Azure.Data;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading.Tasks;

    [Serializable]
    public class StoryDialogue : IDialog<object>
    {
        #region Members
        public const string key = "personalstory";
        private readonly string tableConnection = ConfigurationManager.AppSettings["StoryDialogueStore"];
        #endregion

        #region Methods
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            PersonalStory story;
            context.PrivateConversationData.TryGetValue<PersonalStory>(key, out story);
            story = story ?? new PersonalStory();

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
            }

            if (null != message.Attachments && 0 < message.Attachments.Count)
            {
                story.Images = new string[message.Attachments.Count];
                for (var i = 0; i < message.Attachments.Count; i++)
                {
                    story.Images[i] = message.Attachments[i].ContentUrl;
                }
            }

            if (!string.IsNullOrWhiteSpace(story.Theme) && !string.IsNullOrWhiteSpace(story.Content) && null != story.Images && 0 < story.Images.Length)
            {
                var a = await argument;
                var entity = new PersonalStoryEntity()
                {
                    PartitionKey = a.Recipient.Id,
                    RowKey = a.From.Id,
                    Content = story.Content,
                    Theme = story.Theme,
                    Images = string.Join(",", story.Images),
                    Timestamp = DateTime.UtcNow,
                };

                var table = new TableStorage("userprofile", tableConnection);
                await table.CreateIfNotExists();
                await table.InsertOrReplace(entity);

                story.Task = PersonalStoryTask.Done;
            }

            var replyToConversation = CreateResponse(context, story);

            await context.PostAsync(replyToConversation);
            context.PrivateConversationData.SetValue<PersonalStory>(key, story);

            context.Wait(MessageReceivedAsync);
        }

        private IMessageActivity CreateResponse(IDialogContext context, PersonalStory story)
        {
            var reply = context.MakeMessage();
            reply.Recipient = reply.From;
            reply.Type = "message";
            
            switch (story.Task)
            {
                case PersonalStoryTask.Theme:
                    reply.Text = "please add theme";
                    break;
                case PersonalStoryTask.Description:
                    reply.Text = "please add description";
                    break;
                case PersonalStoryTask.Images:
                    reply.Text = "please add images";
                    break;
                case PersonalStoryTask.Done:
                    reply.Attachments = new List<Attachment>();
                    var cardImages = new List<CardImage>();
                    cardImages.AddRange(story.Images.Select(i => new CardImage(url: i)));
                
                    var card = new HeroCard()
                    {
                        Title = $"Your hustle: {story.Theme}",
                        Subtitle = story.Content,
                        Images = cardImages,
                        Text = "Thanks, come back soon. There will be more!"
                    };
                    reply.Attachments.Add(card.ToAttachment());

                    break;
            }

            return reply;
        }
        #endregion
    }
}