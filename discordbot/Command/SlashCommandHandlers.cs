using System.Text;
using Discord.WebSocket;
using discordbot.Model;
using discordbot.Service;

namespace discordbot.Command;

public class SlashCommandHandlers(Prison prison, VoiceService voiceService, RoleService roleService, UserService userService)
{
    public async Task SlashCommandHandlerAsync(SocketSlashCommand command)
    {

        switch (command.Data.Name)
        {
            case "imprison":
            {
                await HandleImprisonSlashCommandAsync(command);
                break;
            }
            case "stats":
            {
                await HandleStatsSlashCommandAsync(command);
                break;
            }
            case "release":
            {
                await HandleReleaseSlashCommandAsync(command);
                break;
            }
            case "repair":
            {
                await HandleRepairSlashCommandAsync(command);
                break;
            }
        }
    }
    
    private async Task HandleImprisonSlashCommandAsync(SocketSlashCommand command)
    {
        var targetUser = (SocketGuildUser)command.Data.Options.First(it => it.Name == "user").Value;
        var authorUser = (SocketGuildUser)command.User;
        var minutesInPrison = command.Data.Options
            .Where(it => it.Name == "time")
            .Select(it => (long)it.Value)
            .FirstOrDefault(1);
        
        prison.RemoveUserIfSentenceEnded();

        if (!userService.IsAuthorityEqualOrHigher(targetUser, authorUser))
        {
            await command.RespondAsync("куда лезешь бро");
            return;
        }
        
        var role = await roleService.CreateRoleIfNotExistsAsync(targetUser.Guild);
        var addRoleTask = Task.Run(() => targetUser.AddRoleAsync(role));

        if (!prison.UserIdToSentences.ContainsKey(targetUser.Id))
        {
            prison.UserIdToSentences[targetUser.Id] = 
                new Sentence(targetUser, DateTime.Now, minutesInPrison, authorUser);
        }
        else
        {
            if (userService.IsAuthorityEqualOrHigher(prison.UserIdToSentences[targetUser.Id].ImprisonedByUser, authorUser))
            {
                prison.UserIdToSentences[targetUser.Id].MinutesInPrison += minutesInPrison;
            }
        }

        if (targetUser.VoiceChannel != null)
        {
            var channel = await voiceService.CreatePrisonChannelIfNotExistsAsync(targetUser.Guild, role, false);
            var moveUserTask = Task.Run(() => userService.MoveUserAsync(targetUser, channel));
        }
        
        await command.RespondAsync(" добавил " + minutesInPrison + " минут для " + targetUser.DisplayName + ", всего сидеть в клетке " 
                                   + (prison.UserIdToSentences[targetUser.Id].StartTime.AddMinutes(prison.UserIdToSentences[targetUser.Id].MinutesInPrison) - DateTime.Now));
    }
    
    private async Task HandleStatsSlashCommandAsync(SocketSlashCommand command)
    {
        var stringBuilder = new StringBuilder();
        
        prison.RemoveUserIfSentenceEnded();
        
        foreach (var (key, value) in prison.UserIdToSentences)
        {
            stringBuilder.Append(value.User.GlobalName).Append(": ").Append(value.StartTime.AddMinutes(value.MinutesInPrison) - DateTime.Now).Append('\n');
        }

        if (stringBuilder.Length == 0)
        {
            stringBuilder.Append("Тут пусто");
        }
        
        await command.RespondAsync(stringBuilder.ToString());
    }

    private async Task HandleReleaseSlashCommandAsync(SocketSlashCommand command)
    {
        var authorUser = (SocketGuildUser)command.User;
        var count = command.Data.Options.Count(it => it.Name == "user");
        
        var minutesToRemove = command.Data.Options
            .Where(it => it.Name == "time")
            .Select(it => (long)it.Value)
            .FirstOrDefault(10000);

        if (count == 0)
        {
            foreach (var (key, value) in prison.UserIdToSentences)
            {
                if (userService.IsAuthorityEqualOrHigher(value.ImprisonedByUser, authorUser))
                {
                    value.MinutesInPrison -= minutesToRemove;   
                }
            }
        }
        else
        {
            var targetUser = (SocketGuildUser)command.Data.Options.First(it => it.Name == "user").Value;
            
            if (userService.IsAuthorityEqualOrHigher(prison.UserIdToSentences[targetUser.Id].ImprisonedByUser, authorUser)) 
            { 
                prison.UserIdToSentences[targetUser.Id].MinutesInPrison -= minutesToRemove;
            }
        }
        
        prison.RemoveUserIfSentenceEnded();
        
        await command.RespondAsync("Готово");
    }

    private async Task HandleRepairSlashCommandAsync(SocketSlashCommand command)
    {
        var guild = ((SocketGuildUser)command.User).Guild;
        var role = await roleService.CreateRoleIfNotExistsAsync(guild);
        var createChannelTask = Task.Run(() =>
        {
            var createPrisonChannelTask =voiceService.CreatePrisonChannelIfNotExistsAsync(guild,
                role,
                true);
        });
        await command.RespondAsync("Готово");
    }
}