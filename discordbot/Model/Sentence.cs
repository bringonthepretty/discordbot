using Discord.WebSocket;

namespace discordbot.Model;

public class Sentence(SocketGuildUser user, DateTime startTime, long minutesInPrison, SocketGuildUser imprisonedByUser)
{
    public SocketGuildUser User { get; set; } = user;
    public DateTime StartTime { get; set; } = startTime;
    public long MinutesInPrison { get; set; } = minutesInPrison;

    public SocketGuildUser ImprisonedByUser { get; set; } = imprisonedByUser;
}