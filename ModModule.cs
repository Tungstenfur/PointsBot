using NetCord.Services.ApplicationCommands;
using NetCord;

using NetCord.Rest;

namespace silly_kronos;

public class ModModule:ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("Ban", "Ban a user from the server", DefaultGuildPermissions = Permissions.BanUsers)]
    public async Task BanAsync(
        [SlashCommandParameter(Description = "The user to ban")] User user,
        [SlashCommandParameter(Description = "The reason for the ban")] string reason = "No reason provided",
        [SlashCommandParameter(Description = "Number of days to delete messages for (0-7)")] int deleteMessageDays = 0)
    {
        var guild=Context.Interaction.Guild ?? throw new Exception("This command can only be used in a server.");
        GuildUser member = await guild.GetUserAsync(user.Id);
        await guild.BanUserAsync(member.Id, deleteMessageDays*60*60*24,new RestRequestProperties().WithAuditLogReason(reason));
    }
    [SlashCommand("Kick", "Kick a user from the server", DefaultGuildPermissions = Permissions.KickUsers)]
    public async Task KickAsync(
        [SlashCommandParameter(Description = "The user to kick")] User user,
        [SlashCommandParameter(Description = "The reason for the kick")] string reason = "No reason provided")
    {
        var guild=Context.Interaction.Guild ?? throw new Exception("This command can only be used in a server.");
        GuildUser member = await guild.GetUserAsync(user.Id);
        await member.KickAsync(new RestRequestProperties().WithAuditLogReason(reason));
    }
    [SlashCommand("Timeout", "Timeout a user in the server",DefaultGuildPermissions = Permissions.ModerateUsers)]
    public async Task TimeoutAsync(
        [SlashCommandParameter(Description = "The user to timeout")] User user,
        [SlashCommandParameter(Description = "Duration of the timeout in minutes")] int durationMinutes,
        [SlashCommandParameter(Description = "The reason for the timeout")] string reason = "No reason provided")
    {
        var guild=Context.Interaction.Guild ?? throw new Exception("This command can only be used in a server.");
        GuildUser member = await guild.GetUserAsync(user.Id);
        var time = DateTimeOffset.UtcNow.AddMinutes(durationMinutes);
        var props = new RestRequestProperties().WithAuditLogReason(reason);
        await member.TimeOutAsync(time, props);
    }
}