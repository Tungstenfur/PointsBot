# WORK IN PROGRESS
## Unfinished, things may changellll
# The point bot
This is a bot used to manage a point system in a Discord server. It allows users to give and take points from each other, and keeps track of the points in a SQLite database.
## Features
- Give points to other users (admin only)
- Take points from other users (admin only)
- Check your own points
- Check another user's points (menage events only)
- Suggest user point (menage events only)
- View the leaderboard (soon)
- Schedule events
## Usage
### Hosting your own
#### Requirements
- **No .NET runtime required** - Self-contained executables are provided for Windows and Linux

#### Setup
1. **Download the latest release** from the [Releases page](https://github.com/Tungstenfur/PointsBot/releases)
   - For **Windows**: Download `PointsBot-windows-x64.zip`
   - For **Linux**: Download `PointsBot-linux-x64.tar.gz`

2. **Extract the downloaded file** to your desired directory

3. **Configure the bot token**:
   - Rename `appsettings.template.json` to `appsettings.json`
   - Replace `YOUR_BOT_TOKEN_HERE` with your actual Discord bot token

4. **Create a Discord bot**:
   - Go to the [Discord Developer Portal](https://discord.com/developers/applications)
   - Create a new application and add a bot to it
   - Copy the bot token and paste it into your `appsettings.json` file
   - Invite the bot to your server with the appropriate permissions

5. **Run the bot**:
   - **Windows**: Double-click `PointsBot.exe` or run it from command line
   - **Linux**: Run `./PointsBot` from terminal (you may need to make it executable with `chmod +x PointsBot`)

#### Building from source
If you prefer to build from source, you'll need:
- .NET 9.0 SDK
- Run `dotnet build` or `dotnet publish` for your target platform

