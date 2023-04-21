using System.Text.Json.Nodes;

namespace Common;

public class MaelstromMessage
{
    public int Id { get; set; }
    public string Src { get; set; }
    public string Dest { get; set; }
    public JsonObject Body { get; set; }
}