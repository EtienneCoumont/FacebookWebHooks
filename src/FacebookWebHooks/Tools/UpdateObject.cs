using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacebookWebHooks
{
    /// <summary>
    /// Used for json deserialisation
    /// </summary>
    public class UpdateObject
    {
        public ObjectEnum Object { get; set; }
        public Entry[] Entry { get; set; }
    }

    public class Entry
    {
        public string Id { get; set; }

        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime Time { get; set; }

        public Change[] Changes { get; set;  }
    }

    public class Change
    {
        public string Field { get; set; }
        public Value Value { get; set ; }

    }

    public class Value
    {
        public string Item { get; set; }
        public string Verb { get; set; }

        [JsonProperty("photo_id")]
        public Int64 PhotoId { get; set; }
        [JsonProperty("share_id")]
        public Int64 ShareId { get; set; }
        [JsonProperty("post_id")]
        public string PostId { get; set; }
        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("sender_id")]
        public string SenderId { get; set; }
        [JsonProperty("created_time")]
        [JsonConverter(typeof(UnixTimestampConverter))]
        public DateTime CreatedTime { get; set; }
        public int Published { get; set; }
        public string Message { get; set; }
        public string Link { get; set; }
        [JsonProperty("sender_name")]
        public string SenderName { get; set; }
        [JsonProperty("user_id")]
        public Int64 UserId { get; set; }

    }

    public enum ObjectEnum
    {
        Unknown,
        User,
        Page,
        Permissions,
        Payments
    }

    public class UnixTimestampConverter : DateTimeConverterBase
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTime)value - _epoch).TotalSeconds.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return _epoch.AddSeconds((long)reader.Value);
        }
    }
}
