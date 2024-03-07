using Discord;
using Discord.WebSocket;

namespace discordbot.Service;

public class UserService
{
    /**
     * Returns true if second user has higher or equal authority than first user based on roles position.
     * Server ownership gives highest authority, 0 roles gives lowes authority. 
     */
    public bool IsAuthorityEqualOrHigher(SocketGuildUser first, SocketGuildUser second)
    {
        var firstUserRoles = first.Roles.ToList();
        firstUserRoles.Sort((it, other) => it.Position.CompareTo(other.Position));
        var firstTopRolePosition = firstUserRoles.Count == 0 ? int.MinValue : firstUserRoles.Last().Position;

        var secondUserRoles = second.Roles.ToList();
        secondUserRoles.Sort((it, other) => it.Position.CompareTo(other.Position));
        var secondTopRolePosition = secondUserRoles.Count == 0 ? int.MinValue : secondUserRoles.Last().Position;
        
        if (first.Guild.Owner.Equals(first))
        {
            firstTopRolePosition = int.MaxValue;
        }
        
        if (second.Guild.Owner.Equals(second))
        {
            secondTopRolePosition = int.MaxValue;
        }

        return firstTopRolePosition <= secondTopRolePosition;
    }

    public async Task MoveUserAsync(SocketGuildUser user, IVoiceChannel newChannel)
    {
        if (user.VoiceChannel != null)
        {
            await user.ModifyAsync(properties =>
            {
                properties.ChannelId = newChannel.Id;
            });
        }
    }
}