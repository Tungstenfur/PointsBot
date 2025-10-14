//Made by lord_sinister (tungsten) under PolyForm Noncommercial License 1.0.0
using NetCord;
using NetCord.JsonModels;
using NetCord.Services.ApplicationCommands;
using NetCord.Rest;
namespace silly_kronos;
public class Schedule:ApplicationCommandModule<ApplicationCommandContext> //Allows commands registration from modules
{
    [SlashCommand("pong", "Pong!")] // Command for testing
    public static string Pong() => "Ping!";
    [SlashCommand("schedule", "Schedule an event", DefaultGuildPermissions = Permissions.ManageEvents)] // Command to schedule an event, requires ManageEvents permission
    public Task ScheduleAsync(
        //Parameters
        [SlashCommandParameter(Description = "The channel to send to")] TextChannel channel,
        [SlashCommandParameter(Description = "Type of the event")] EventType type,
        [SlashCommandParameter(Description = "Description")] string desc,
        [SlashCommandParameter(Description = "Start time of the event (UTC unix time)")] ulong startTime,
        [SlashCommandParameter(Description = "Duration (minutes)")] ushort duration,
        [SlashCommandParameter(Description = "Host of the event")] User host,
        [SlashCommandParameter(Description = "Color of the event")] Colors color = Colors.Blue)
    {
        //Create an embed
        var embed = new EmbedProperties()
            .WithTitle("New Event Scheduled")
            .WithDescription($"**Type:** {type}\n**Description:** {desc}\n**Start Time:** <t:{startTime}:F> (<t:{startTime}:R>)\n**Duration:** {duration} minutes\n**Host:** {host}")
            .WithColor(new((int)color));
        //Send the embed to the specified channel
        var message = new MessageProperties().AddEmbeds(embed);
        channel.SendMessageAsync(message);
        //Respond to the command
        var props = new InteractionMessageProperties
        {
            Content = "Event scheduled successfully!",
            Flags = MessageFlags.Ephemeral //Visible only to the command user
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
    [SlashCommand("cancel", "Sends a cancellation notice for an event")] // Command to cancel an event, requires ManageEvents permission
    public Task CancelAsync(
        //Parameters
        [SlashCommandParameter(Description = "The channel to send to")] TextChannel channel,
        [SlashCommandParameter(Description = "Type of the event")] EventType type,
        [SlashCommandParameter(Description = "Description")] string desc,
        [SlashCommandParameter(Description = "Start time of the event (UTC unix time)")] ulong startTime,
        [SlashCommandParameter(Description = "Duration (minutes)")] ushort duration,
        [SlashCommandParameter(Description = "Host of the event")] User host,
        [SlashCommandParameter(Description = "Color of the event")] Colors color = Colors.Red)
    {
        //Create an embed
        var embed = new EmbedProperties()
            .WithTitle("Event Cancelled")
            .WithDescription($"**Type:** {type}\n**Description:** {desc}\n**Start Time:** <t:{startTime}:F> (<t:{startTime}:R>)\n**Duration:** {duration} minutes\n**Host:** {host}")
            .WithColor(new((int)color));
        //Send the embed to the specified channel
        var message = new MessageProperties().AddEmbeds(embed);
        channel.SendMessageAsync(message);
        //Respond to the command
        var props = new InteractionMessageProperties
        {
            Content = "Event cancellation sent successfully!",
            Flags = MessageFlags.Ephemeral
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
}
