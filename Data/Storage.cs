public class Storage
{
        #region Members
        //Table Storage Connection
        private readonly string tableConnection = ConfigurationManager.AppSettings["StoryDialogueStore"];

        private static bool msgTableCreated;
        private static bool storyTableCreated;
        #endregion

        public async Task Save(Message msg)
        {
            var table = new TableStorage("message", tableConnection);
            if !(msgTableCreated)
            {
                msgTableCreated = await table.CreateIfNotExists();
            }

            await table.Save(msg);
        }
        public async Task Save(PersonalStory story)
        {
                var table = new TableStorage("userprofile", tableConnection);
            if !(storyTableCreated)
            {
                storyTableCreated = await table.CreateIfNotExists();
            }
                await table.CreateIfNotExists();
                await table.InsertOrReplace(entity);
        }
}