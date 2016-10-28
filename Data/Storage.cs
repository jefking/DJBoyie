public class Storage
{
        #region Members
        //Table Storage Connection
        private readonly string tableConnection = ConfigurationManager.AppSettings["StoryDialogueStore"];
        #endregion

        public async Task Save(Message msg)
        {
            var msgTable = new TableStorage("message", tableConnection);
            await msgTable.CreateIfNotExists();
            await msgTable.Save(msg);
        }
        public async Task Save(PersonalStory story)
        {

                var table = new TableStorage("userprofile", tableConnection);
                await table.CreateIfNotExists();
                await table.InsertOrReplace(entity);
        }
}