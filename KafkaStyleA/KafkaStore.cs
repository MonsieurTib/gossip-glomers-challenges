namespace Maelstrom.KafkaStyleA;

public class KafkaStore
{
    private readonly Dictionary<string, List<KafkaTopicValue>> _topics = new();
    private readonly object _lockObj = new();
    public int Add(string key, int value)
    {
        lock (_lockObj)
        {
            var offset = 0;
            if (_topics.TryGetValue(key, out var topic))
            {
                offset = topic.Count ;
                topic.Add( new(offset, value));
            }
            else
            {
                _topics.TryAdd(key, new List<KafkaTopicValue>(){ new (offset, value) });
            }
            
            return offset;
        }
    }

    public List<KafkaTopicValue>? Get(string key, int offset)
    {
        lock (_lockObj)
        {
            if (_topics.TryGetValue(key, out var topicValues))
            {
                return topicValues.Skip(offset).ToList();
            }
        }
        
        return null;
    }
}

public record KafkaTopicValue(int offset, int Value);