using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PeopleInSpaceMaui.Models;

using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class CrewModel : ReactiveObject
    {
        [JsonProperty("name")]
        [Reactive]
        public string Name { get; set; }

        [JsonProperty("agency")]
        [Reactive]
        public string Agency { get; set; }

        [JsonProperty("image")]
        [Reactive]
        public Uri Image { get; set; }

        [JsonProperty("wikipedia")]
        [Reactive]
        public Uri Wikipedia { get; set; }

        [JsonProperty("launches")]
        [Reactive]
        public string[] Launches { get; set; }

        [JsonProperty("status")]
        [Reactive]
        public Status Status { get; set; }

        [JsonProperty("id")]
        [Reactive]
        public string Id { get; set; }
    }

    public enum Status { Active, Inactive, Retired, Unknown };

    public partial class CrewModel
    {
        public static CrewModel[] FromJson(string json) => JsonConvert.DeserializeObject<CrewModel[]>(json, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                StatusConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class StatusConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Status) || t == typeof(Status?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "active")
            {
                return Status.Active;
            }
            throw new Exception("Cannot unmarshal type Status");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Status)untypedValue;
            if (value == Status.Active)
            {
                serializer.Serialize(writer, "active");
                return;
            }
            throw new Exception("Cannot marshal type Status");
        }

        public static readonly StatusConverter Singleton = new StatusConverter();
    }