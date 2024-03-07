using Discord;
using Discord.WebSocket;
using discordbot.Command;
using discordbot.Model;
using discordbot.Service;
using Microsoft.Extensions.DependencyInjection;

namespace discordbot;

public class Program
{
    private const string Token = "placeholder";
    private static DiscordSocketClient _client;
    
    private static IServiceProvider _serviceProvider;
    
    static IServiceProvider CreateProvider()
    {
        
        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged
        };
        
        var collection = new ServiceCollection();

        collection.AddSingleton(config);
        collection.AddSingleton<DiscordSocketClient>();
        collection.AddSingleton<SlashCommandHandlers>();
        collection.AddSingleton<VoiceStateUpdateHandlers>();
        collection.AddSingleton<SlashCommandsRegisterer>();
        collection.AddSingleton<Prison>();
        collection.AddSingleton<VoiceService>();
        collection.AddSingleton<RoleService>();
        collection.AddSingleton<UserService>();
        
        return collection.BuildServiceProvider();
    }
    
    public static async Task Main()
    {
        _serviceProvider = CreateProvider();

        _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        _client.Log += Log;

        await _client.LoginAsync(TokenType.Bot, Token);
        await _client.StartAsync();

        var slashCommandHandlers = _serviceProvider.GetRequiredService<SlashCommandHandlers>();
        var voiceStateUpdateHandlers = _serviceProvider.GetRequiredService<VoiceStateUpdateHandlers>();
        var slashCommandsRegisterer = _serviceProvider.GetRequiredService<SlashCommandsRegisterer>();
        
        _client.Ready += slashCommandsRegisterer.RegisterSlashCommandsAsync;
        _client.SlashCommandExecuted += slashCommandHandlers.SlashCommandHandlerAsync;
        _client.UserVoiceStateUpdated += voiceStateUpdateHandlers.OnVoiceStateUpdatedAsync;
        
        await Task.Delay(-1);
    }

    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}

