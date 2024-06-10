namespace PeopleInSpaceMaui.Models;

using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class CrewModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("agency")]
        public Agency Agency { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("wikipedia")]
        public Uri Wikipedia { get; set; }

        [JsonProperty("launches")]
        public Launch[] Launches { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public string AgencyString => Agency.ToString();
    }

    public enum Agency { Esa, Jaxa, Nasa, SpaceX, AxiomSpace, Roscosmos };

    public enum Launch { The5Eb87D46Ffd86E000604B388,
        The5Eb87D4Dffd86E000604B38E,
        The5Fe3Af58B3467846B324215F,
        The607a37565a906a44023e0866,
        The5fe3b15eb3467846b324216d,
        The61eefaa89eb1064137a1bd73,
        The6243ade2af52800c6e919255,
        The62dd70D5202306255024D139
    };

    public enum Status { Active, Inactive, Retired, Unknown };

    public partial class CrewModel
    {
        public static CrewModel[] FromJson(string json) => JsonConvert.DeserializeObject<CrewModel[]>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CrewModel[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                AgencyConverter.Singleton,
                LaunchConverter.Singleton,
                StatusConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class AgencyConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Agency) || t == typeof(Agency?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "ESA":
                    return Agency.Esa;
                case "JAXA":
                    return Agency.Jaxa;
                case "NASA":
                    return Agency.Nasa;
                case "SpaceX":
                    return Agency.SpaceX;
                case "Axiom Space":
                    return Agency.AxiomSpace;
                case "Roscosmos":
                    return Agency.Roscosmos;
                        
            }
            throw new Exception("Cannot unmarshal type Agency");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Agency)untypedValue;
            switch (value)
            {
                case Agency.Esa:
                    serializer.Serialize(writer, "ESA");
                    return;
                case Agency.Jaxa:
                    serializer.Serialize(writer, "JAXA");
                    return;
                case Agency.Nasa:
                    serializer.Serialize(writer, "NASA");
                    return;
                case Agency.SpaceX:
                    serializer.Serialize(writer, "SpaceX");
                    return;
            }
            throw new Exception("Cannot marshal type Agency");
        }

        public static readonly AgencyConverter Singleton = new AgencyConverter();
    }

    internal class LaunchConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Launch) || t == typeof(Launch?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "5eb87d46ffd86e000604b388":
                    return Launch.The5Eb87D46Ffd86E000604B388;
                case "5eb87d4dffd86e000604b38e":
                    return Launch.The5Eb87D4Dffd86E000604B38E;
                case "5fe3af58b3467846b324215f":
                    return Launch.The5Fe3Af58B3467846B324215F;
                case "607a37565a906a44023e0866":
                    return Launch.The607a37565a906a44023e0866;
                case "5fe3b15eb3467846b324216d":
                    return Launch.The5fe3b15eb3467846b324216d;
                case "61eefaa89eb1064137a1bd73":
                    return Launch.The61eefaa89eb1064137a1bd73;
                case "6243ade2af52800c6e919255":
                    return Launch.The6243ade2af52800c6e919255;
                case "62dd70d5202306255024d139":
                    return Launch.The62dd70D5202306255024D139;
            }
            throw new Exception("Cannot unmarshal type Launch");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Launch)untypedValue;
            switch (value)
            {
                case Launch.The5Eb87D46Ffd86E000604B388:
                    serializer.Serialize(writer, "5eb87d46ffd86e000604b388");
                    return;
                case Launch.The5Eb87D4Dffd86E000604B38E:
                    serializer.Serialize(writer, "5eb87d4dffd86e000604b38e");
                    return;
                case Launch.The5Fe3Af58B3467846B324215F:
                    serializer.Serialize(writer, "5fe3af58b3467846b324215f");
                    return;
                case Launch.The607a37565a906a44023e0866:
                    serializer.Serialize(writer, "607a37565a906a44023e0866");
                    return;
            }
            throw new Exception("Cannot marshal type Launch");
        }

        public static readonly LaunchConverter Singleton = new LaunchConverter();
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