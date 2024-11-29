namespace CommonAssets
{
    public record PublishEventRequest(string PubsubName, string TopicName, object Data);
}
