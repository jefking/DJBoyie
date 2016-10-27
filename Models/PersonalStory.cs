namespace shujaaz.djboyie.Models
{
    using System;

    [Serializable]
    public class PersonalStory
    {
        public string[] Images;
        public string Content;
        public string Theme;
        public PersonalStoryTask Task;
    }
}