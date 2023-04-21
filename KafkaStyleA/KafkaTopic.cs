using System.Text.Json.Nodes;

namespace Maelstrom.KafkaStyleA;

/*
public class KafkaTopic
{
    private readonly List<Message> _messages  = new();
    private readonly object _msgLock = new();
    private int _offset = 0;

    public KafkaTopic(int message)
    {
        AddMessage(message, out _);
    }

    public List<Message> GetMessages(int offset)
    {
        lock (_msgLock)
        {
            return _messages.Skip(offset).ToList();
        }
    }
    public void AddMessage(int msg, out int offset)
    {
        //offset = 0;
        lock (_msgLock)
        {
            _messages.Add(new Message(_offset, msg));
            offset = _offset;
            _offset += 1;
        }
    }

    public int GetOffset()
    {
        lock (_msgLock)
        {
            return _offset;
        }
    }
}

public record struct Message(int Offset, int Msg);
*/