using System;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
namespace SISDiscordBot.AllSlashCommands;

public class ModerationCommands : ApplicationCommandModule
{
	[SlashCommand("Ban", "Ban a user from server")]
	public async Task Ban(InteractionContext ctx, [Option("User", "User you want to ban")] DiscordUser user,
												  [Option("Reason", "Why we are banning this user")] string reason = null)
	{
		// defer message allows you to create response later on
		await ctx.DeferAsync();

		if(ctx.Member.Permissions == DSharpPlus.Permissions.BanMembers)
		{
			var member = (DiscordMember)user;
			await ctx.Guild.BanMemberAsync(member, 0, reason);

			var banMessage = new DiscordEmbedBuilder
			{
				Title = "User: " + member.Username + " Banned.",
				Description = reason,
				Color = DiscordColor.Red
			};

			await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(banMessage);
		}
		else
		{
			var warning = new DiscordEmbedBuilder()
			{
				Title = "Access denied",
				Description = "Do not have proper credentials to execute the command",
				Color = DiscordColor.Red
			};

			await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(warning));
		}

	}

}

