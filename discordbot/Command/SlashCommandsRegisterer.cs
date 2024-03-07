using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace discordbot.Command;

public class SlashCommandsRegisterer(DiscordSocketClient client)
{
    public async Task RegisterSlashCommandsAsync()
    {
        var guild = client.GetGuild(838892001537163345);
        
        var imprisonCommand = new SlashCommandBuilder();

        imprisonCommand.WithName("imprison")
            .WithDescription("Садит подонка в клетку")
            .AddOption("user", ApplicationCommandOptionType.User, "кого", true)
            .AddOption("time", ApplicationCommandOptionType.Integer, "на сколько, минута по дефолту", false);

        var statsCommand = new SlashCommandBuilder();

        statsCommand.WithName("stats")
            .WithDescription("Как там в тюрьме");
        
        var releaseCommand = new SlashCommandBuilder();

        releaseCommand.WithName("release")
            .WithDescription("Отпустить")
            .AddOption("user", ApplicationCommandOptionType.User, "кого, всех по дефолту", false)
            .AddOption("time", ApplicationCommandOptionType.Integer, "на сколько минут уменьшить бан, выпустить по дефолту", false);
        
        var repairCommand = new SlashCommandBuilder();
        
        repairCommand.WithName("repair")
            .WithDescription("Как там в тюрьме");

        try
        {
            var deleteCommandsTask = Task.Run(() => guild.DeleteApplicationCommandsAsync());
            var createImprisonCommandTask = Task.Run(() => guild.CreateApplicationCommandAsync(imprisonCommand.Build()));
            var createStatsCommandTask = Task.Run(() => guild.CreateApplicationCommandAsync(statsCommand.Build()));
            var createReleaseCommandTask = Task.Run(() => guild.CreateApplicationCommandAsync(releaseCommand.Build()));
            var createRepairCommandTask = Task.Run(() => guild.CreateApplicationCommandAsync(repairCommand.Build()));

            await deleteCommandsTask;
            await createImprisonCommandTask;
            await createStatsCommandTask;
            await createReleaseCommandTask;
            await createRepairCommandTask;
        }
        catch(ApplicationCommandException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            
            Console.WriteLine(json);
        }
    }
}