using NetCord;
using NetCord.JsonModels;
using NetCord.Services.ApplicationCommands;
using NetCord.Rest;
namespace silly_kronos;

public class Schedule:ApplicationCommandModule<ApplicationCommandContext>
{

    [SlashCommand("pong", "Pong!")]
    public static string Pong() => "Ping!";
    
    [SlashCommand("schedule", "Schedule an event")]
    public Task ScheduleAsync(
        [SlashCommandParameter(Description = "The channel to send to")] TextChannel channel,
        [SlashCommandParameter(Description = "Type of the event")] EventType type,
        [SlashCommandParameter(Description = "Description")] string desc,
        [SlashCommandParameter(Description = "Start time of the event (UTC unix time)")] ulong startTime,
        [SlashCommandParameter(Description = "Duration (minutes)")] ushort duration,
        [SlashCommandParameter(Description = "Host of the event")] User host,
        [SlashCommandParameter(Description = "Color of the event")] Colors color = Colors.Blue)
    {
        var embed = new EmbedProperties()
            .WithTitle("New Event Scheduled")
            .WithDescription($"**Type:** {type}\n**Description:** {desc}\n**Start Time:** <t:{startTime}:F> (<t:{startTime}:R>)\n**Duration:** {duration} minutes\n**Host:** {host}")
            .WithColor(new((int)color));
        var message = new MessageProperties().AddEmbeds(embed);
        channel.SendMessageAsync(message);
        var props = new InteractionMessageProperties
        {
            Content = "Event scheduled successfully!",
            Flags = MessageFlags.Ephemeral
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
}
