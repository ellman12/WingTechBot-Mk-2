namespace WingTechBot.Extensions.Json;

///Converts json string values to ulong on read, and back to string on write.
public sealed class StringUInt64Converter : JsonConverter<ulong>
{
	public override ulong Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String && ulong.TryParse(reader.GetString(), out ulong value))
		{
			return value;
		}

		if (reader.TokenType == JsonTokenType.Number)
		{
			return reader.GetUInt64();
		}

		throw new JsonException("Invalid format for ulong value.");
	}

	public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToString());
	}
}