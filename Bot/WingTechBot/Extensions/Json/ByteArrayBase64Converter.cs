namespace WingTechBot.Extensions.Json;

///Used to convert byte[]s to and from base64. Necessary for HTTP GETs.
public sealed class ByteArrayBase64Converter : JsonConverter<byte[]>
{
	public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return Convert.FromBase64String(reader.GetString() ?? throw new NullReferenceException());
	}

	public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(Convert.ToBase64String(value));
	}
}
