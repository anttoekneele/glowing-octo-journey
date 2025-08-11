namespace Capstone.Api.Contracts
{
    public class CommunicationEvent
    {
        public Guid CommunicationId { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
