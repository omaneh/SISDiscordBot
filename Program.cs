namespace SISDiscordBot;
class Program
{
    static void Main(string[] args)
    {
        Bot bot = new Bot();

        bot.RunBotAsync().GetAwaiter().GetResult();
    }
}

