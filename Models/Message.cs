namespace shujaaz.djboyie.Models
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;

    [Serializable]
    public class Message : TableEntity
    {
        public string Content { get; set; }
        public int Task { get; set; }
    }
}