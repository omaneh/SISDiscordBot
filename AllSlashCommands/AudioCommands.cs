using System;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.Lavalink;
namespace SISDiscordBot.AllSlashCommands;

public class AudioCommands : ApplicationCommandModule
{
	[SlashCommand("play", "Play audio in voice channel")]
	public async Task Play(InteractionContext ctx, [Option("title", "provide the title of the audio you would like to play")] string title)
	{
		await ctx.DeferAsync();
		var userVC = ctx.Member.VoiceState.Channel; // if user is in a voice channel assign the channel

		if(ctx.Member.VoiceState == null || userVC == null)
		{
			await ctx.Channel.SendMessageAsync("You are not currently in a valid voice channel");
			return;
		}

		var lavalinkInstance = ctx.Client.GetLavalink(); // gets lava link from the current client

		if(!lavalinkInstance.ConnectedNodes.Any())
		{
            await ctx.Channel.SendMessageAsync("Server connection is currently down.");
			return;
        }

		if(userVC.Type != ChannelType.Voice)
		{
            await ctx.Channel.SendMessageAsync("You are not currently in a valid voice channel");
			return;
        }

		var node = lavalinkInstance.ConnectedNodes.Values.First();
		await node.ConnectAsync(userVC);

		var con = node.GetGuildConnection(ctx.Member.VoiceState.Guild); // guild associated with this voice state

		if(con == null)
        {
            await ctx.Channel.SendMessageAsync("Server connection is currently down.");
            return;
        }

		var searchQuery = await node.Rest.GetTracksAsync(title);

		if(searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
		{
            await ctx.Channel.SendMessageAsync($"Failed to find title: {title}. Please try again");
            return;
        }

		var musicTrack = searchQuery.Tracks.First();

		await con.PlayAsync(musicTrack);

		var musicDescription = "Now playing: " + musicTrack.Title + "\nLength: " + musicTrack.Length  + "\nURL: " + musicTrack.Uri;

		var nowPlayingEmbed = new DiscordEmbedBuilder()
		{
			Color = DiscordColor.Purple,
			Title = "Now playing audio",
			Description = musicDescription
		};

		await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(nowPlayingEmbed));

		if(musicTrack.Position == musicTrack.Length)
		{
			await node.StopAsync();
		}
    }

}

