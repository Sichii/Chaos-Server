using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Core.Data;

namespace Chaos.Core.JsonConverters;

public class UserStatSheetConverter : JsonConverter<UserStatSheet>
{
    public override UserStatSheet? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => default;

    public override void Write(Utf8JsonWriter writer, UserStatSheet value, JsonSerializerOptions options) =>
        throw new NotImplementedException();

    private record SerializableUserStatSheet
    {
        [JsonInclude, JsonPropertyName("ability")]
        protected int _ability;
        [JsonInclude, JsonPropertyName("ac")]
        protected int _ac;
        [JsonInclude, JsonPropertyName("attackSpeed")]
        protected int _attackSpeed;
        [JsonInclude, JsonPropertyName("con")]
        protected int _con;
        [JsonInclude, JsonPropertyName("cooldownReduction")]
        protected int _cooldownReduction;
        [JsonInclude, JsonPropertyName("currentHp")]
        protected int _currentHp;
        [JsonInclude, JsonPropertyName("currentMp")]
        protected int _currentMp;
        protected int _currentWeight;
        [JsonInclude, JsonPropertyName("dex")]
        protected int _dex;
        [JsonInclude, JsonPropertyName("dmg")]
        protected int _dmg;
        [JsonInclude, JsonPropertyName("hit")]
        protected int _hit;
        [JsonInclude, JsonPropertyName("int")]
        protected int _int;
        [JsonInclude, JsonPropertyName("level")]
        protected int _level;
        [JsonInclude, JsonPropertyName("magicResistance")]
        protected int _magicResistance;
        [JsonInclude, JsonPropertyName("maximumHp")]
        protected int _maximumHp;
        [JsonInclude, JsonPropertyName("maximumMp")]
        protected int _maximumMp;
        [JsonInclude, JsonPropertyName("str")]
        protected int _str;
        [JsonInclude, JsonPropertyName("toNextAbility")]
        protected int _toNextAbility;
        [JsonInclude, JsonPropertyName("toNextLevel")]
        protected int _toNextLevel;
        [JsonInclude, JsonPropertyName("totalAbility")]
        protected uint _totalAbility;
        [JsonInclude, JsonPropertyName("totalExp")]
        protected uint _totalExp;
        [JsonInclude, JsonPropertyName("unspentPoints")]
        protected int _unspentPoints;
        [JsonInclude, JsonPropertyName("wis")]
        protected int _wis;
    }
}