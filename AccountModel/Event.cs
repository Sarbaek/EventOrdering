namespace EventOrdering
{
    using System;
    using System.Text.Json.Serialization;
 
    public class Event
    {
        public EventType EventType { get; set; }
        [JsonPropertyName("Timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonPropertyName("Payload")]
        public Payload? Payload { get; set; }
    }

    public class Payload
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("Currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;
    }
    public enum EventType
    {
        AccountOpened,
        AccountClosed,
        AccountUpdated,
        AccountSettled,
    }

}
