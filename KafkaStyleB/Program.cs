using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;
using Common;
using Maelstrom.KafkaStyleB;

var node = new Node();
var store = new KafkaStore();
var commitOffsets = new ConcurrentDictionary<string, int>();

node.Handle("send", async message =>
{
    var body = message.Body;
    var key = body["key"]!.GetValue<string>();
    var msg = body["msg"]!.GetValue<int>();
    var offset = 0;
    if (node.Nodes.Contains(node.NodeId))
    {
        if (node.NodeId == node.Nodes.First())
        {
            offset = store.Add(key, msg);
        }
        else
        {
            var result = await node.Rpc(node.Nodes.First(), message.Body);
            offset = result["offset"].GetValue<int>();
        }
    }
    
    await node.ReplyAsync(message, new JsonObject() { ["type"] = "send_ok", ["offset"] = offset });
});

node.Handle("poll", async message =>
{
    var body = message.Body;
    var request = body["offsets"].Deserialize<Dictionary<string, int>>();
    var result = new Dictionary<string, List<int[]>>();
    if (node.Nodes.Contains(message.Src))
    {
        if (node.NodeId != node.Nodes.First())
        {
            var resultFromMaster = await node.Rpc(node.Nodes.First(), message.Body);
            await node.ReplyAsync(message, new JsonObject() { ["type"] = "poll_ok", ["msgs"] = JsonSerializer.SerializeToNode(resultFromMaster) });
            return;
        }
    }
    foreach (var kv in request)
    {
        var values = store.Get(kv.Key, kv.Value);
        if (values != null)
        {
            result.Add(kv.Key, values.Select((val) => new int[] { val.offset ,val.Value }).ToList());
        }
    }
   
    await node.ReplyAsync(message, new JsonObject() { ["type"] = "poll_ok", ["msgs"] = JsonSerializer.SerializeToNode(result) });
});

node.Handle("commit_offsets", async message =>
{
    var body = message.Body;
    var request = body["offsets"].Deserialize<Dictionary<string, int>>();
    
    if (node.Nodes.Contains(message.Src))
    {
        if (node.NodeId != node.Nodes.First())
        {
            await node.Rpc(node.Nodes.First(), message.Body);
            await node.ReplyAsync(message, new JsonObject() { ["type"] = "commit_offsets_ok"});
            return;
        }
    }
    
    foreach (var kv in request)
    {
        if (commitOffsets.TryGetValue(kv.Key, out var offset))
        {
            commitOffsets.TryUpdate(kv.Key, kv.Value, offset);
        }
        else
        {
            commitOffsets.TryAdd(kv.Key, kv.Value);
        }
    }
    
    await node.ReplyAsync(message, new JsonObject() { ["type"] = "commit_offsets_ok"});
    
});

node.Handle("list_committed_offsets", async message =>
{
    if (node.Nodes.Contains(message.Src))
    {
        if (node.NodeId != node.Nodes.First())
        {
            var resultFromMaster = await node.Rpc(node.Nodes.First(), message.Body);
            await node.ReplyAsync(message, new JsonObject() { ["type"] = "list_committed_offsets_ok", ["offsets"] = JsonSerializer.SerializeToNode(resultFromMaster) });
            return;
        }
    }
    await node.ReplyAsync(message, new JsonObject() { ["type"] = "list_committed_offsets_ok", ["offsets"] = JsonSerializer.SerializeToNode(commitOffsets) });
});

await node.RunAsync();



