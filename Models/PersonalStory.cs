namespace shujaaz.djboyie.Models
{
    using System;

    [Serializable]
    public class PersonalStory
    {
        public string[] Images { get; set; }
        public string Content { get; set; }
        public string Theme { get; set; }
        public PersonalStoryTask Task { get; set; }
    }
}