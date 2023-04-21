using Common;

var node = new Node();
var id = 0;

node.Handle("generate", async message =>
{
    var body = message.Body;
    body["type"] = "generate_ok";
    body["id"] = $"{node.NodeId}-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Interlocked.Increment(ref id)}";
    await node.ReplyAsync(message, body);
});

await node.RunAsync();
