using System;
using System.IO;
using DSharpPlus;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext;
using System.Text;
using Newtonsoft.Json;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SISDiscordBot.AllCommands;
using SISDiscordBot.AllSlashCommands;


namespace SISDiscordBot;

public class Bot
{

	//props
	public DiscordClient _client { get; private set; }
	public InteractivityExtension _interactivity { get; private set; }
	public CommandsNextExtension _commands { get; private set; }
	public SlashCommandsExtension _slashCommands { get; private set; }

	public async Task RunBotAsync()
	{
		var json = string.Empty;

		using (var file = File.OpenRead("config.json"))
		{
			using (var stream = new StreamReader(file, new UTF8Encoding()))
			{
				json = await stream.ReadToEndAsync();
			}

		}

		var configJson = JsonConvert.DeserializeObject<JSON>(json);

		// config 
		var config = new DiscordConfiguration()
		{
			Intents = DiscordIntents.All,
			Token = configJson.Token,
			TokenType = TokenType.Bot,
			AutoReconnect = true
		};

		//setting client up

		_client = new DiscordClient(config);
		_client.UseInteractivity(new InteractivityConfiguration()
		{
			Timeout = TimeSpan.FromHours(2)
		});

		// setting up commands 
		var commandConfig = new CommandsNextConfiguration()
		{
			StringPrefixes = new string[] { configJson.Prefix },
			EnableMentionPrefix = true,
			EnableDefaultHelp = false,
			EnableDms = true
		};

		_commands = _client.UseCommandsNext(commandConfig);
		_slashCommands = _client.UseSlashCommands();
		_commands.RegisterCommands<Commands>();
		_slashCommands.RegisterCommands<SlashCommands>();
		_commands.CommandErrored += OnCommandError;

		await _client.ConnectAsync();

		//prevents timing issues and crashing 
		await Task.Delay(-1);

	}

	private async Task OnCommandError(CommandsNextExtension sender, CommandErrorEventArgs e)
	{
		if(e.Exception is ChecksFailedException)
		{
			var castedException = (ChecksFailedException)e.Exception;
			string cooldownTimer = string.Empty;

			foreach(var check in castedException.FailedChecks)
			{
				var cooldown = (CooldownAttribute)check;
				TimeSpan time = cooldown.GetRemainingCooldown(e.Context);
				cooldownTimer = time.ToString();
			}

			var cooldownMessage = new DiscordEmbedBuilder()
			{
				Title = "Wait for cooldown to finish",
				Description = "Time remaining " + cooldownTimer,
				Color = DiscordColor.DarkRed
			};

			await e.Context.Channel.SendMessageAsync(embed: cooldownMessage);


		}

	}

    private Task OnClientReady(ReadyEventArgs e)
    {
        return Task.CompletedTask;
    }

}
