using Common;

var node = new Node();

node.Handle("echo", async message =>
{
    var body = message.Body;
    body["type"] = "echo_ok";
    await node.ReplyAsync(message, body);
});

await node.RunAsync();


