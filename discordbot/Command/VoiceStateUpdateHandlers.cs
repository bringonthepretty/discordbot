using Discord.WebSocket;
using discordbot.Model;
using discordbot.Service;

namespace discordbot.Command;

public class VoiceStateUpdateHandlers(Prison prison, VoiceService voiceService, RoleService roleService, UserService userService)
{
    public async Task OnVoiceStateUpdatedAsync(SocketUser user, SocketVoiceState state1, SocketVoiceState state2)
    {
        var targetUser = (SocketGuildUser)user;
        
        if (targetUser.IsBot)
            return;
        
        prison.RemoveUserIfSentenceEnded();

        var role = await roleService.CreateRoleIfNotExistsAsync(targetUser.Guild);
        
        if (state2.VoiceChannel != null && prison.UserIdToSentences.ContainsKey(targetUser.Id))
        {
            var channel = await voiceService.CreatePrisonChannelIfNotExistsAsync(targetUser.Guild, role, false);
            if (prison.PrisonChannel!.Id != state2.VoiceChannel?.Id)
            {
                await userService.MoveUserAsync(targetUser, channel);
            }
        }
    }
}