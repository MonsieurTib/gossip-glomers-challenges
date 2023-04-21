using System.Collections.Concurrent;
using System.Text.Json.Nodes;
using Common;

var node = new Node();
var kv = new ConcurrentDictionary<string, int>();

node.OnInit += async () =>
{
    while (true)
    {
        if(kv.TryGetValue(node.NodeId, out var count))
        {
            foreach (var dest in node.Nodes.Where(n => n != node.NodeId))
            {
                await node.Rpc(dest, new JsonObject()
                {
                    ["type"] = "replicate",
                    ["node_id"] = node.NodeId,
                    ["count"] = count

                });
            }
        }
        await Task.Delay(2000);
    }
};

node.Handle("add", async message =>
{
    var body = message.Body;
    var delta = body["delta"]!.GetValue<int>();
    if (kv.TryGetValue(node.NodeId, out var total))
    {
        kv[node.NodeId] = total + delta;
    }
    else
    {
        kv.TryAdd(node.NodeId, delta);
    }
    await node.ReplyAsync(message, new JsonObject() { ["type"] = "add_ok" });
});

node.Handle("read", async message =>
{
    var body = message.Body;
    body["type"] = "read_ok";
    body["value"] = kv.Sum( kvp =>  kvp.Value );
    await node.ReplyAsync(message, body);
});

node.Handle("replicate", async message =>
{
    var body = message.Body;
    var src = body["node_id"].GetValue<string>();
    var count = body["count"].GetValue<int>();
    if (kv.TryGetValue(src, out var total))
    {
        kv[src] = count ;
    }
    else
    {
        kv.TryAdd(src, count);
    }

    await node.ReplyAsync(message, new JsonObject() { ["type"] = "replicate_ok" });
});

await node.RunAsync();
