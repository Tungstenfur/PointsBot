//Made by lord_sinister (tungsten) under PolyForm Noncommercial License 1.0.0
using NetCord;
using NetCord.JsonModels;
using NetCord.Services.ApplicationCommands;
using NetCord.Rest;
namespace silly_kronos;

public class pointsModule:ApplicationCommandModule<ApplicationCommandContext> //Allows commands registration from modules
{
    [SlashCommand("init", "Initialize points system for this server", DefaultGuildPermissions = Permissions.Administrator)] //Command to initialize points system, requires Administrator permission
    public Task InitAsync()
    {
        ulong guildId = Context.Interaction.GuildId ?? throw new Exception("This command can only be used in a server."); //Getting the guild and error handling
        if (!db.addTable(guildId)) return Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = "Failed to initialize points system for this server.",
            Flags = MessageFlags.Ephemeral
        }));
        if (!db.addColumn(guildId)) return Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties //Adding points columns to the database
        {
            Content = "Failed to add points column to the database.",
            Flags = MessageFlags.Ephemeral
        }));
        var props = new InteractionMessageProperties //Responding to the command
        {
            Content = "Points system initialized successfully for this server!",
            Flags = MessageFlags.Ephemeral
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
    
    [SlashCommand("addpoints", "Add points to a user", DefaultGuildPermissions = Permissions.Administrator)]
    public Task AddPointsAsync(
        [SlashCommandParameter(Description = "The user to add points to")] User user,
        [SlashCommandParameter(Description = "The amount of points to add")] int points)
    {
        ulong guildId = Context.Interaction.GuildId ?? throw new Exception("This command can only be used in a server.");
        ulong userId = user.Id;
        if (!db.addPoints(guildId, userId, points)) return Context.Interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties
        {
            Content = "Failed to add points to the user.",
            Flags = MessageFlags.Ephemeral
        }));
        var props = new InteractionMessageProperties
        {
            Content = $"Added {points} points to {user}.",
            Flags = MessageFlags.Ephemeral
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
    [SlashCommand("mypoints", "Check your points")]
    public Task MyPointsAsync()
    {
        ulong guildId = Context.Interaction.GuildId ?? throw new Exception("This command can only be used in a server.");
        ulong userId = Context.Interaction.User.Id;
        string points = db.getPoints(guildId, userId);
        var props = new InteractionMessageProperties
        {
            Content = $"You have {points} points.",
            Flags = MessageFlags.Ephemeral
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
}