using System;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
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

		if(ctx.Member.Permissions.HasPermission(Permissions.BanMembers))
		{
			var member = (DiscordMember)user;
			await ctx.Guild.BanMemberAsync(member, 0, reason);

			var banMessage = new DiscordEmbedBuilder
			{
				Title = "User: " + member.Username + " Banned.",
				Description = reason,
				Color = DiscordColor.Red
			};

			await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(banMessage));
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

	[SlashCommand("Kick", "Remove user from server")]
	public async Task Kick(InteractionContext ctx, [Option("User", "User that is wanted to be removed")] DiscordUser user, [Option("Reason", "Short description as to why user was banned")] string reason = null)
	{
		await ctx.DeferAsync();

		if(ctx.Member.Permissions.HasPermission(Permissions.Administrator))
		{
			var kickedUser = (DiscordMember)user;
			await kickedUser.RemoveAsync();

			var kickedMessage = new DiscordEmbedBuilder()
			{
				Title = kickedUser + " kicked from server",
				Description = reason,
				Color = DiscordColor.Red
			};

			await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(kickedMessage));
		}
		else
		{
            var errorMessage = new DiscordEmbedBuilder()
            {
                Title = "Error",
                Description = "You do not have the credentials to execute this action",
                Color = DiscordColor.Red
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(errorMessage));
        }

	}

	[SlashCommand("Timeout", "Prevent a user for speaking for a certain duration")]
	public async Task TimeOut(InteractionContext ctx, [Option("User", "User that you want to time out")] DiscordUser user, [Option("Timeout", "Duration in minutes")] long duration)
	{
		await ctx.DeferAsync();

		if(ctx.Member.Permissions.HasPermission(Permissions.BanMembers))
		{
			DateTime timeout = DateTime.Now.AddMinutes(duration); 
			DiscordMember member = (DiscordMember)user;
			await member.TimeoutAsync(timeout);

            var timeoutMessage = new DiscordEmbedBuilder()
            {
				Title = "Timeout",
                Description = member + " timed out for " + duration + " minutes",
                Color = DiscordColor.Red
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(timeoutMessage));
        }
		else
		{
            var errorMessage = new DiscordEmbedBuilder()
            {
                Title = "Error",
                Description = "You do not have the credentials to execute this action",
                Color = DiscordColor.Red
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(errorMessage));
        }


	}

	[SlashCommand("add_Role", "Create a new role- no permissions by default")]
	public async Task CreateRole(InteractionContext ctx, [Option("Name", "What is the name of the role?")] string name, [Option("Color", "What color would you like the tag to be?")] string color, [Option("Emoji", "Please include an emoji for the role")] DiscordEmoji emoji)
	{
		await ctx.DeferAsync();

		if(ctx.Member.Permissions.HasPermission(Permissions.ManageRoles))
		{
			var chosenColor = color.ToLower() switch
			{
				"red" => DiscordColor.Red,
				"yellow" => DiscordColor.Yellow,
				"blue" => DiscordColor.Blue,
				"purple" => DiscordColor.Purple,
				"pink" => DiscordColor.HotPink,
				"brown" => DiscordColor.Brown,
				"black" => DiscordColor.Black,
				"green" => DiscordColor.Green,
				_ => DiscordColor.Orange
			};


            var role = await ctx.Guild.CreateRoleAsync(name, Permissions.None, chosenColor);
            // Associate the emoji with the role
            await role.ModifyAsync(properties => properties.Emoji = emoji);

            var roleCreatedEmbed = new DiscordEmbedBuilder()
			{
				Title = $"Role {name} successfully created",
				Color = DiscordColor.Blurple
			};

			await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(roleCreatedEmbed));
		}
		else
		{
            var errorMessage = new DiscordEmbedBuilder()
            {
                Title = "Error",
                Description = "You do not have the credentials to execute this action",
                Color = DiscordColor.Red
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(errorMessage));
        }


	}

    [SlashCommand("remove_role", "Remove user role- no permissions by default")]
    public async Task RemoveRole(InteractionContext ctx, [Option("User", "Whose role is being removed?")] DiscordUser user, [Option("Role", "Designated role to be removed")] DiscordRole intendedRole)
    {
        await ctx.DeferAsync();

        if (ctx.Member.Permissions.HasPermission(Permissions.ManageRoles))
        {
			DiscordMember member = (DiscordMember)user;

			foreach(var role in member.Roles)
			{
				if(role == intendedRole)
				{
					await member.RevokeRoleAsync(role);


                    var roleRemoved = new DiscordEmbedBuilder()
                    {
                        Title = $"Role: {intendedRole} successfully removed",
                        Color = DiscordColor.DarkButNotBlack
					};

                    await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(roleRemoved));

                }

            }
        }
        else
        {
            var errorMessage = new DiscordEmbedBuilder()
            {
                Title = "Error",
                Description = "You do not have the credentials to execute this action",
                Color = DiscordColor.Red
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(errorMessage));
        }


    }

    [SlashCommand("assignment", "assign a role to a user")]

	public async Task AssignRole(InteractionContext ctx, [Option("User", "What user would you like to assign the role to?")] DiscordUser user, [Option("Role", "What role would you like to assign to the user?")] DiscordRole intendedRole)
	{
		await ctx.DeferAsync();

		if(ctx.Member.Permissions.HasPermission(Permissions.ManageRoles))
		{
			foreach(var role in ctx.Guild.Roles)
			{
				if(role.Value == intendedRole)
				{
					await ((DiscordMember)user).GrantRoleAsync(role.Value);
					var roleMessage = new DiscordEmbedBuilder()
					{
						Title = "Role added",
						Description = user + "was successfully added the role: " + intendedRole,
						Color = intendedRole.Color
					};

					await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(roleMessage));

				}
			}

		}
		else
		{
            var errorMessage = new DiscordEmbedBuilder()
            {
                Title = "Error",
                Description = "You do not have the credentials to execute this action",
                Color = DiscordColor.Red
            };

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(errorMessage));
        }

	}

	[SlashCommand("Welcome_board", "Make a poll to allow users to assign themselves roles")]
	public async Task WelcomeBoard(InteractionContext ctx, [Option("Message", "Provide a short title to welcome others into your discord")] string message)
	{
		await ctx.DeferAsync();

        try
        {
			var interactivity = ctx.Client.GetInteractivity();
			DiscordRole[] allRoles = new DiscordRole[ctx.Guild.Roles.Count];
			int pos = 0;
			string body = string.Empty;
			foreach(var role in ctx.Guild.Roles)
			{
				body += $"{role.Value.Emoji} {role.Value.Name}\n";
				allRoles[pos] = role.Value;
				pos++;
			}


            var pollMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"{message}")
                .WithDescription("React to this message to get your roles!\n" + body)
                .WithColor(DiscordColor.CornflowerBlue));

            var reactionIncoming = await ctx.Channel.SendMessageAsync(pollMessage);

            foreach (var role in allRoles)
            {
                await reactionIncoming.CreateReactionAsync(role.Emoji);
            }

            var result = await interactivity.CollectReactionsAsync(reactionIncoming);

            foreach (var rxn in result)
            {
                foreach(var user in rxn.Users)
				{
					var member = (DiscordMember)user;
					var targetRole = allRoles.Where(x => x.Emoji == rxn.Emoji).FirstOrDefault(); // I want this to return the role associated with the emoji
					if (targetRole != null) { await member.GrantRoleAsync(targetRole); }
				}

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }


}

