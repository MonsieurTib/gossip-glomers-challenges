using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Common;
public class Node
{
    private long _messageId = 0;
    private readonly Dictionary<string, List<Func<MaelstromMessage, Task>>> _subscriptions = new();
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly ConcurrentDictionary<long, TaskCompletionSource<JsonObject>> _replyHandlers = new();
    public string NodeId { get; private set; } = string.Empty;
    public List<string> Nodes { get; private set; } = new();
    public event Func<Task>? OnInit;

    public async Task RunAsync()
    {
        Handle("init", async message =>
        {
            NodeId = message.Body["node_id"].GetValue<string>();
            Nodes = message.Body["node_ids"].Deserialize<List<string>>();
            OnInit?.Invoke();
            var body = new JsonObject
            {
                ["type"] = "init_ok"
            };
            await ReplyAsync(message, body);
        });
        
        var stdIn = Console.OpenStandardInput();
        var reader = new StreamReader(stdIn);
        
        while (reader.EndOfStream == false)
        {
            var line = await reader.ReadLineAsync();
            if (line == null)
            {
                continue;
            }

            var req = JsonSerializer.Deserialize<MaelstromMessage>(line, _jsonSerializerOptions);
            var type = req.Body["type"].GetValue<string>();
            var inReplyTo = req.Body["in_reply_to"];

            if (inReplyTo != null /*&& inReplyTo.GetValue<long>() >= 0*/)
            {
                HandleRpcReply(req, inReplyTo.GetValue<long>());
            }

            if (!_subscriptions.TryGetValue(type, out var callbacks))
            {
                continue;
            }

            foreach (var callback in callbacks)
            {
                Task.Run(async () => await callback(req));
            }
        }
    }

    private void HandleRpcReply(MaelstromMessage req, long id)
    {
        if (_replyHandlers.TryGetValue(id, out var tcs))
        {
            if (req.Body["type"].GetValue<string>() == "error")
            {
                tcs.SetException(new Exception($"{req.Body["code"]}-{req.Body["text"]}"));
            }
            else
            {
                tcs.SetResult(req.Body);
                _replyHandlers.TryRemove(id, out _);
            }
        }
    }

    public void Handle(string type, Func<MaelstromMessage, Task> callback)
    {
        if (_subscriptions.TryGetValue(type, out var actions))
        {
            actions.Add(callback);
        }
        else
        {
            _subscriptions.Add(type, new List<Func<MaelstromMessage, Task>>() { callback});
        }
    }

    public async Task<JsonObject> Rpc(string dest, JsonObject request)
    {
        var messageId = Interlocked.Increment(ref _messageId);
        while (true)
        {
            var result = await RetryRpc(dest, request, messageId);
            if (result != null)
            {
                return result;
            }
        }
    }

    private Task<JsonObject?> RetryRpc(string dest, JsonObject request, long messageId)
    {
        var tcs = new TaskCompletionSource<JsonObject>();
        _replyHandlers.TryAdd(messageId, tcs );
        request["msg_id"] = messageId;
        Send(dest, request);
        return tcs.Task.TimeoutAfter(TimeSpan.FromSeconds(5));
    }

    public async Task ReplyAsync(MaelstromMessage req, JsonObject body)
    {
        body["in_reply_to"] = req.Body["msg_id"]!.GetValue<long>();
        await SendAsync(req.Src, body);
    }

    public async Task SendAsync(string dest, JsonObject body)
    {
        await SendAsync(new MaelstromMessage()
        {
            Src = NodeId,
            Dest = dest,
            Body = body
        });
    }

    private async Task SendAsync(MaelstromMessage msg)
    {
        Console.WriteLine(JsonSerializer.Serialize(msg, _jsonSerializerOptions));
        //Console.Error.WriteLine("debug" + JsonSerializer.Serialize(msg, _jsonSerializerOptions));
        await Console.Out.FlushAsync();
       
    }

    private void Send(string dest, JsonObject body)
    {
        Send(new MaelstromMessage()
        {
            Src = NodeId,
            Dest = dest,
            Body = body
        });
    }

    private void Send(MaelstromMessage msg)
    {
        Console.WriteLine(JsonSerializer.Serialize(msg, _jsonSerializerOptions));
        Console.Out.Flush();
    }
}