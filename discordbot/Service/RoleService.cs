using Discord;
using Discord.WebSocket;

namespace discordbot.Service;

public class RoleService
{
    public async Task<IRole> CreateRoleIfNotExistsAsync(SocketGuild server)
    {
        var roleList = server.Roles.Where(it => it.Name.ToLower().Trim().Equals("в клетке")).ToList();
        IRole role;

        if (roleList.Count == 0)
        {
            role = await server.CreateRoleAsync("в клетке", color: new Color());
        }
        else
        {
            role = roleList.First();
        }

        return role;
    }
}