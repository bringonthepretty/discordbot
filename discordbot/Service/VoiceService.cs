using Discord;
using Discord.WebSocket;
using discordbot.Model;

namespace discordbot.Service;

public class VoiceService(Prison prison)
{
    public async Task<IVoiceChannel> CreatePrisonChannelIfNotExistsAsync(SocketGuild server, IRole role, bool ensurePermissions)
    {
        var prisonChannels = server.VoiceChannels
            .Where(it => it.Name.Trim().ToLower().Equals("клетка"))
            .ToList();
        if (prisonChannels.Count == 0)
        {
            prison.PrisonChannel = await server.CreateVoiceChannelAsync("Клетка");
        }
        else
        {
            prison.PrisonChannel = prisonChannels.First();
        }
        
        if (ensurePermissions || prisonChannels.Count == 0)
        {
            var overwriteRolePermissionsTask = Task.Run(() => 
                prison.PrisonChannel.AddPermissionOverwriteAsync(role, DenyAllExceptViewAndConnect()));
            var overwriteEveryonePermissionsTask = Task.Run(() => 
                prison.PrisonChannel.AddPermissionOverwriteAsync(server.EveryoneRole, OverwritePermissions.DenyAll(prison.PrisonChannel)));
        }

        return prison.PrisonChannel;
    }

    public async Task RemovePrisonChannelIfExistsAndEmpty()
    {
        if (prison.PrisonChannel != null && (await prison.PrisonChannel.GetUsersAsync().ToListAsync()).Count == 0)
        {
            var deletePrisonChannelTask = Task.Run(() => prison.PrisonChannel.DeleteAsync());
        }
    }
    
    private OverwritePermissions DenyAllExceptViewAndConnect()
    {
        return new OverwritePermissions(PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Allow,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Allow,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny, 
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny, 
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny, 
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny,
            PermValue.Deny, 
            PermValue.Deny);
    }
}