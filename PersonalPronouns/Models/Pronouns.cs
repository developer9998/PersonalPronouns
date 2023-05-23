namespace PersonalPronouns.Models
{
    public class Pronouns
    {
        public string Subject;
        public string Object;
        public string Possessive;

        public Pronouns(string subjectPronoun, string objectPronoun, string possessivePronoun)
        {
            Subject = subjectPronoun;
            Object = objectPronoun;
            Possessive = possessivePronoun;
        }

        public Pronouns(string subjectPronoun, string objectPronoun)
        {
            Subject = subjectPronoun;
            Object = objectPronoun;
        }
    }
}
