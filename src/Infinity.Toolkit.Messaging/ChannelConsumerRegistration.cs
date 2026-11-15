namespace Infinity.Toolkit.Messaging;

[DebuggerDisplay("BrokerName = {BrokerName}, EventType = {EventType.FullName,nq}")]
public class ChannelConsumerRegistration
{
    public string BrokerName { get; set; } = string.Empty;

    [JsonIgnore]
    public Type EventType { get; init; }

    public object? Key { get; set; }

    public ChannelConsumerOptions Options { get; set; }
}
