using System.Text.Json;
using System.Text.Json.Nodes;
using Common;

var node = new Node();
var ids = new List<int>();
var neighbors = new List<string>();

node.Handle("broadcast", async message =>
{
    var id = message.Body["message"].GetValue<int>();
    if (ids.Contains(id) == false)
    {
        ids.Add(id);
    }

    foreach (var neighbor in neighbors.Where(neighbor => neighbor != message.Src))
    {
        await node.SendAsync(neighbor, message.Body);
    }

    await node.ReplyAsync(message, new JsonObject() { ["type"] = "broadcast_ok" });
});

node.Handle("topology", async message =>
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
    body["messages"] = JsonSerializer.SerializeToNode(ids);
    await node.ReplyAsync(message, body);
});

await node.RunAsync();

