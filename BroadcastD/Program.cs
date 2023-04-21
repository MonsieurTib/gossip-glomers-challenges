using System.Text.Json;
using System.Text.Json.Nodes;
using Common;

var node = new Node();
var ids = new List<int>();

var lockObj = new object();

List<string[]>? buckets = default;
var masters = Enumerable.Empty<string>();

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
    
    var neighbors = new List<string>();
    foreach (var nodes in buckets)
    {
        var index = Array.IndexOf(nodes, node.NodeId);
        if (index >= 0)
        {
            if (index == 0)
            {
                var t = buckets.SelectMany(b => b.First()).ToList();
                neighbors.AddRange(nodes.Except(new[] { node.NodeId }));
                neighbors.AddRange( masters.Except(new[] { node.NodeId }) );
            }
            else
            {
                neighbors.Add(nodes[0]);
            }
            break;
        }
    }

    var tasks = neighbors.Where(neighbor => neighbor != message.Src)
        .Select(neighbor => node.Rpc(neighbor, message.Body))
        .ToList();
    
    await Task.WhenAll(tasks);
});

node.Handle("topology", async message =>
{
    if (buckets == null)
    {
        var nodeCount = node.Nodes.Count;
        buckets = node.Nodes
            .Chunk(Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(nodeCount) / 2m)))
            .ToList();
        masters = buckets.Select(s => s.First());
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