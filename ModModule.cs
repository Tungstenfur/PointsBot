using NetCord.Services.ApplicationCommands;
using NetCord;
using NetCord.Rest;
namespace silly_kronos;
public class ModModule:ApplicationCommandModule<ApplicationCommandContext> //Allows commands registration from modules
{
    [SlashCommand("ban", "Ban a user from the server", DefaultGuildPermissions = Permissions.BanUsers)] //Command to ban a user, requires BanUsers permission
    public async Task BanAsync(
        //Parameters
        [SlashCommandParameter(Description = "The user to ban")] User user,
        [SlashCommandParameter(Description = "The reason for the ban")] string reason = "No reason provided",
        [SlashCommandParameter(Description = "Number of days to delete messages for (0-7)")] int deleteMessageDays = 0)
    {
        var guild=Context.Interaction.Guild ?? throw new Exception("This command can only be used in a server."); //Getting the guild and error handling
        GuildUser member = await guild.GetUserAsync(user.Id); //Geting the user to ban
        await guild.BanUserAsync(member.Id, deleteMessageDays*60*60*24,new RestRequestProperties().WithAuditLogReason(reason)); //Banning the user, multiplying days by seconds in a day to get seconds
        //Responding to the command
        var props = new InteractionMessageProperties
        {
            Content = $"Banned {user} from the server.",
        };
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
    [SlashCommand("kick", "Kick a user from the server", DefaultGuildPermissions = Permissions.KickUsers)] //Command to kick a user, requires KickUsers permission
    public async Task KickAsync(
        //Parameters
        [SlashCommandParameter(Description = "The user to kick")] User user,
        [SlashCommandParameter(Description = "The reason for the kick")] string reason = "No reason provided")
    {
        var guild=Context.Interaction.Guild ?? throw new Exception("This command can only be used in a server."); //Getting the guild and error handling
        GuildUser member = await guild.GetUserAsync(user.Id); //Getting the user to kick
        await member.KickAsync(new RestRequestProperties().WithAuditLogReason(reason)); //Kicking the user
        //Responding to the command
        var props = new InteractionMessageProperties
        {
            Content = $"Kicked {user} from the server.",
        };
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(props));
    }
    [SlashCommand("timeout", "Timeout a user in the server",DefaultGuildPermissions = Permissions.ModerateUsers)] //Command to timeout a user, requires ModerateUsers permission
    public async Task TimeoutAsync(
        //Parameters
        [SlashCommandParameter(Description = "The user to timeout")] User user,
        [SlashCommandParameter(Description = "Duration of the timeout in minutes")] int durationMinutes,
        [SlashCommandParameter(Description = "The reason for the timeout")] string reason = "No reason provided")
    {
        var guild=Context.Interaction.Guild ?? throw new Exception("This command can only be used in a server."); //Getting the guild and error handling
        GuildUser member = await guild.GetUserAsync(user.Id); //Getting the user to timeout
        var time = DateTimeOffset.UtcNow.AddMinutes(durationMinutes); //Calculating the time to set the timeout until
        var props = new RestRequestProperties().WithAuditLogReason(reason); 
        await member.TimeOutAsync(time, props); //Timing out the user
        //Responding to the command
        var responseProps = new InteractionMessageProperties
        {
            Content = $"Timed out {user} for {durationMinutes} minutes.",
        };
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(responseProps));
    }
    [SlashCommand("moth","Returns a moth" )] //Command to return a random moth image from the moths folder (very important)
    public Task MothAsync()
    {
        //Prepare the variables
        string[] files;
        FileStream fs;
        //Get a random file from the moths folder, different pathing for linux and windows
        if (OperatingSystem.IsLinux())
        {
            files= Directory.GetFiles(Directory.GetCurrentDirectory() + "/moths/"); //getting all files in the moths folder
            fs = File.OpenRead(files[Random.Shared.Next(files.Length)]); //opening a random file from the moths folder using Shared random
        }
        else if (OperatingSystem.IsWindows())
        {
            files= Directory.GetFiles(Directory.GetCurrentDirectory() + "\\moths\\"); //Same as above but for windows
            fs = File.OpenRead(files[Random.Shared.Next(files.Length)]);
        }
        else
            throw new Exception("why"); //Are you hosting on samsung smart refrigerator? So sad
        var attachment = new AttachmentProperties("moth.png", fs) //Creating the attachment
            .WithTitle("Moth!"); //Title
        var embed = new EmbedProperties() //Creating the embed
        {
            Title = "Moth!",
            Image = "attachment://moth.png"
        };
        var props = new InteractionMessageProperties()
            .AddEmbeds(embed).AddAttachments(attachment); //Adding the embed and attachment to the message properties
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(props)); //Returns moth
    }
}