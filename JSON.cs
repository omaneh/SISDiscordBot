using System;
using Newtonsoft.Json;

namespace SISDiscordBot;

internal class JSON
{
	[JsonProperty("token")]
	internal string Token { get; private set; }
	[JsonProperty("prefix")]
	internal string Prefix { get; private set; }
}

