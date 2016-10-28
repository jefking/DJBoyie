namespace shujaaz.djboyie.Data
{
    using System.Configuration;
    using King.Azure.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;

    public class Storage
    {
            #region Members
            //Table Storage Connection
            private readonly string tableConnection = ConfigurationManager.AppSettings["StoryDialogueStore"];

            private static bool msgTableCreated;
            private static bool storyTableCreated;
            #endregion

            #region Methods
            public async Task Save(Message msg)
            {
                var table = new TableStorage("messages", tableConnection);
                if (!msgTableCreated)
                {
                    msgTableCreated = await table.CreateIfNotExists();
                }

                await table.InsertOrReplace(msg);
            }

            public async Task Save(PersonalStoryEntity entity)
            {
                var table = new TableStorage("userprofile", tableConnection);
                if (!storyTableCreated)
                {
                    storyTableCreated = await table.CreateIfNotExists();
                }

                await table.CreateIfNotExists();
                await table.InsertOrReplace(entity);
            }

            public async Task<int> MessagesSinceDone(string partition)
            {
                var table = new TableStorage("messages", tableConnection);
                var msgs = await table.QueryByPartition<Message>(partition);
                return (from m in msgs
                        where m.Task == (int)PersonalStoryTask.Done
                        select m).Count();
            }
            #endregion
    }
}