using Newtonsoft.Json;
using System.Collections.Generic;

public class HubMessage
{
    public string MessageFromUser { get; set; }
    public string MessageToUser   { get; set; }
    public string MessageType     { get; set; }   // ERR / REQ / RES / INF / WAR
    public string MessageRequestCode { get; set; }
    public string MessageSubject  { get; set; }
    public List<MessageParam> MessageParams { get; set; }
    public string MessageBody     { get; set; }

    public string ToJson() => JsonConvert.SerializeObject(this);

    public T GetBodyObject<T>() => JsonConvert.DeserializeObject<T>(MessageBody);

    public List<MessageParam> GetMessageParams() => MessageParams ?? new List<MessageParam>();

    public static HubMessage CreateHubMessage(string json) => JsonConvert.DeserializeObject<HubMessage>(json);

    public class MessageParam
    {
        public string ParamName  { get; set; }
        public string ParamValue { get; set; }
    }
}