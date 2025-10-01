using NetCord;
using NetCord.JsonModels;
using NetCord.Services.ApplicationCommands;
using NetCord.Rest;
namespace silly_kronos;

public class pointsModule:ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("init", "Initialize points system for this server")]
    public Task InitAsync()
    {
        ulong guildId = Context.Interaction.GuildId ?? throw new Exception("This command can only be used in a server.");
        if (!db.addTable(guildId)) return Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = "Failed to initialize points system for this server.",
            Flags = MessageFlags.Ephemeral
        }));
        if (!db.addColumn(guildId, "points")) return Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = "Failed to add points column to the database.",
            Flags = MessageFlags.Ephemeral
        }));
        if (!db.addColumn(guildId, "points_pending")) return Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = "Failed to add points column to the database.",
            Flags = MessageFlags.Ephemeral
        }));
        var props = new InteractionMessageProperties
        {
            Content = "Points system initialized successfully for this server!",
            Flags = MessageFlags.Ephemeral
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
    
    [SlashCommand("addpoints", "Add points to a user")]
    public Task AddPointsAsync(
        [SlashCommandParameter(Description = "The user to add points to")] User user,
        [SlashCommandParameter(Description = "The amount of points to add")] int points,
        [SlashCommandParameter(Description = "Table to add points to (default: points)")] string table = "points")
    {
        ulong guildId = Context.Interaction.GuildId ?? throw new Exception("This command can only be used in a server.");
        if (!db.addPoints(guildId, user.Id, points, table)) return Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = $"Failed to add points to the user in the {table} table.",
            Flags = MessageFlags.Ephemeral
        }));
        var props = new InteractionMessageProperties
        {
            Content = $"Successfully added {points} points to {user}.",
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
}