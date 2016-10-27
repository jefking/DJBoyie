namespace shujaaz.djboyie.Models
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;

    [Serializable]
    public class PersonalStoryEntity : TableEntity
    {
        public string Images;
        public string Content;
        public string Theme;
        public int Task;
    }
}