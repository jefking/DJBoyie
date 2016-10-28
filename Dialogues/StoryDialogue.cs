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
        //Story Data for User
        public const string key = "personalstory";

        //Storage
        private readonly Storage storage = new Storage();
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

            var msg = new Message()
            {
                    PartitionKey = a.Recipient.Id,
                    RowKey = $"{a.From.Id}_{DateTime.UtcNow}",
                    Timestamp = DateTime.UtcNow,
                    Content = message.Text,
                    Task = story.Task
            };
            await storage.Save(msg);

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
                    Images = string.Join(", ", story.Images),
                    Timestamp = DateTime.UtcNow,
                };
                await storage.Save(entity);

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
                    var count = await storage.MessagesSinceDone();
                    if (0 >= count)
                    {
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
                    }
                    else
                    {
                        reply.Text = "I am thinking about your hustle... more coming soon. Please feel free to add more photos.";
                    }
                    break;
            }

            return reply;
        }
        #endregion
    }
}