namespace shujaaz.djboyie.Models
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;

    [Serializable]
    public class PersonalStoryEntity : TableEntity
    {
        public string Images { get; set; }
        public string Content { get; set; }
        public string Theme { get; set; }
        public int Task { get; set; }
    }
}