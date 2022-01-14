namespace Common
{
    public class Payload
    {
        public string Content { get; set; }

        public string SenderId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public Guid Id { get; set; } = new Guid();
    }
}
