using System;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

namespace SISDiscordBot.AllSlashCommands;

public class SlashCommands : ApplicationCommandModule
{

    [SlashCommand("Poll", "Run a quick poll to get a consensus on something!")]
    public async Task PollCommand(InteractionContext ctx, [Option("Question", "Main question of poll")] string question, [Option("Duration", "Length of time poll should be up")] long duration, [Option("Option1", "First")] string opt1, [Option("Option2", "Second")] string opt2, [Option("Option3", "Third")] string opt3, [Option("Option4", "Four")] string opt4)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent(".."));

        try
        {
            var interacitivty = ctx.Client.GetInteractivity();
            TimeSpan time = TimeSpan.FromMinutes(duration);
            DiscordEmoji[] emojis = {DiscordEmoji.FromName(ctx.Client, ":one:", false),
            DiscordEmoji.FromName(ctx.Client, ":two:", false),
            DiscordEmoji.FromName(ctx.Client, ":three:", false),
            DiscordEmoji.FromName(ctx.Client, ":four:", false)};

            string[] options = { opt1, opt2, opt3, opt4 };
            int pos = 0;
            string stringSelect = "";
            foreach (string choice in options)
            {
                stringSelect += $"{emojis[pos]}  | {choice} \n";
                pos++;
            }

            var pollMessage = new DiscordMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder()
                .WithTitle($"{string.Join(" ", question)}")
                .WithDescription($"{stringSelect}")
                .WithColor(DiscordColor.Azure));

            var reactionIncoming = await ctx.Channel.SendMessageAsync(pollMessage);

            foreach (var emoj in emojis)
            {
                await reactionIncoming.CreateReactionAsync(emoj);
            }

            var result = await interacitivty.CollectReactionsAsync(reactionIncoming, time);

            int e1 = 0;
            int e2 = 0;
            int e3 = 0;
            int e4 = 0;

            foreach (var rxn in result)
            {
                if (rxn.Emoji == emojis[0]) { e1++; }
                if (rxn.Emoji == emojis[1]) { e2++; }
                if (rxn.Emoji == emojis[2]) { e3++; }
                if (rxn.Emoji == emojis[3]) { e4++; }
            }

            int total = e1 + e2 + e3 + e3 + e4;

            int[] optionsR = { e1, e2, e3, e4 };
            int posR = 0;
            string stringSelectR = "";
            foreach (int choice in optionsR)
            {
                stringSelectR += $"{emojis[posR]}  | {choice} \n";
                posR++;
            }


            var resultsMessage = new DiscordMessageBuilder()
           .AddEmbed(new DiscordEmbedBuilder()
           .WithTitle($"Results: ")
           .WithDescription($"{stringSelectR}\n Total Votes: {total}")
           .WithColor(DiscordColor.Azure));

            await ctx.Channel.SendMessageAsync(resultsMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }
}

