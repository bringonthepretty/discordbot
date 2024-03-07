using Discord;

namespace discordbot.Model;

public class Prison
{
    public IDictionary<ulong, Sentence> UserIdToSentences { get; } = new Dictionary<ulong, Sentence>();
    public IVoiceChannel? PrisonChannel { get; set; } = null;

    public void RemoveUserIfSentenceEnded()
    {
        foreach (var (key, value) in UserIdToSentences)
        {
            if (value.StartTime.AddMinutes(value.MinutesInPrison) <= DateTime.Now)
            {
                UserIdToSentences.Remove(key);
            }
        }
    }
}