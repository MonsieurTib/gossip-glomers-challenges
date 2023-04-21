using System.Text.Json;
using System.Text.Json.Nodes;
using Common;

var node = new Node();
var store = new Dictionary<int, int>();

node.Handle("txn", async message =>
{
    var body = message.Body;
    var txn = body["txn"].Deserialize<List<object[]>>();
    var txnResult = new List<object?[]>();

    foreach (var batch in txn)
    {
        var cmd = batch[0].ToString();
        var key = ((JsonElement)batch[1]).GetInt32();
        switch (cmd)
        {
            case "r" when store.TryGetValue(key, out var storeValue):
                txnResult.Add(new object[] { "r", key, storeValue });
                break;
            case "r":
                txnResult.Add(new object?[] { "r", key, null });
                break;
            case "w":
                store[key] = ((JsonElement)batch[2]).GetInt32();
                txnResult.Add(new object?[] { cmd, key, ((JsonElement)batch[2]).GetInt32() });
                break;
        }
    }
    
    await node.ReplyAsync(message, new JsonObject() { ["type"] = "txn_ok", ["txn"] =  JsonSerializer.SerializeToNode(txnResult) });

});

await node.RunAsync();

