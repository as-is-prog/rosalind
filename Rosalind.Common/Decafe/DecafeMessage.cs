namespace Shiorose.Decafe
{
    public enum DecafeMessageType
    {
        Speak,
        AskQuestion,
        ThinkStart,
        ThinkEnd,
        Error
    }

    public class DecafeMessage
    {
        public DecafeMessageType Type { get; set; }
        public string Content { get; set; }
        public int Surface { get; set; }
        public string[] Choices { get; set; }
        public string QuestionId { get; set; }
        public long Timestamp { get; set; }
    }
}
