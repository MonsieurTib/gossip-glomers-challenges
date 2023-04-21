using System.Text.Json;
using System.Text.Json.Nodes;
using Common;

var node = new Node();
var ids = new List<int>();
var neighbors = new List<string>();
var lockObj = new object();
node.Handle("broadcast", async message =>
{
    var id = message.Body["message"].GetValue<int>();
    lock (lockObj)
    {
        if (ids.Contains(id) == false)
        {
            ids.Add(id);
        }
    }
    await node.ReplyAsync(message, new JsonObject() { ["type"] = "broadcast_ok" });
    
    var tasks = neighbors.Where(neighbor => neighbor != message.Src)
        .Select(neighbor => node.Rpc(neighbor, message.Body))
        .ToList();
    
    await Task.WhenAll(tasks);
});

node.Handle("topology", async message =>
{
    if (neighbors.Count == 0)
    {
        var topology = message.Body["topology"].Deserialize<Dictionary<string, List<string>>>();
        if (topology.TryGetValue(node.NodeId, out var nodeIds))
        {
            neighbors = nodeIds;
        }
        else
        {
            throw new Exception("topology mismatch");
        }
    }
   
    var body = new JsonObject
    {
        ["type"] = "topology_ok"
    };
    await node.ReplyAsync(message, body);
});

node.Handle("read", async message =>
{
    var body = message.Body;
    
    body["type"] = "read_ok";
    lock (lockObj)
    {
        body["messages"] = JsonSerializer.SerializeToNode(ids);
    }
    await node.ReplyAsync(message, body);
});

await node.RunAsync();