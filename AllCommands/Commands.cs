using System;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace SISDiscordBot.AllCommands;

public class Commands : BaseCommandModule
{

    [Command("hello")]

    public async Task Respond(CommandContext ctx)
    {
        await ctx.Channel.SendMessageAsync("Hello!");
    }

}

