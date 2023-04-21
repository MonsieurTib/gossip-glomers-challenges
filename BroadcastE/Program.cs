using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Common;

var node = new Node();
var ids = new List<int>();

var lockObj = new object();

List<string[]>? buckets = default;
var masters = Enumerable.Empty<string>();

var chanel = Channel.CreateUnbounded<(MaelstromMessage msg,List<string> dests)>();
var dests = new List<string>();
node.OnInit += async () =>
{
    var nodeCount = node.Nodes.Count;
    buckets = node.Nodes
        .Chunk(Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(nodeCount) / 2m)))
        .ToList();
    masters = buckets.Select(s => s.First());
   
    dests.AddRange(node.Nodes.Except(new[] { node.NodeId }));
    dests.AddRange(masters.Except(new[] { node.NodeId }));
    
    if (masters.Contains(node.NodeId))
    {
        while (await chanel.Reader.WaitToReadAsync())
        {
            while (chanel.Reader.TryRead(out var payload))
            {
                payload.msg.Body["messages"] = JsonSerializer.SerializeToNode(ids);
                var tasks = payload.dests.Where(neighbor => neighbor != payload.msg.Src)
                    .Select(neighbor => node.Rpc(neighbor, payload.msg.Body))
                    .ToList();
                await Task.WhenAll(tasks);
                await Task.Delay(200);
            }
        }
    }
};


node.Handle("broadcast", async message =>
{
    var id = message.Body["message"].GetValue<int>();
    var nodeBatchIds = message.Body["messages"];
    lock (lockObj)
    {
        if (ids.Contains(id) == false)
        {
            ids.Add(id);
        }

        if (nodeBatchIds != null)
        {
            var batchIds = nodeBatchIds.Deserialize<List<int>>();
            foreach (var item in batchIds.Where(item => ids.Contains(item) == false))
            {
                ids.Add(item);
            }
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
               
                await chanel.Writer.WriteAsync((message, dests));
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