using Microsoft.Data.Sqlite;

namespace silly_kronos;

internal static class db
{
    private static SqliteConnection _conn; //Database connection

    internal static bool dbOpen() //Open connection to the database
    {
        try
        {
            _conn = new SqliteConnection("Data Source=pointsbot.db"); //Database file
            _conn.Open(); //Open
            return true; //It worked!
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening database: {ex.Message}"); 
            return false; //Throw scary error
        }
    }
    internal static bool addTable(ulong guildId) //Create table for guild if it doesn't exist
    {
        try
        {
            using var cmd = _conn.CreateCommand(); 
            //TODO: IMPORTANT: ALTER CURRENT TABLES TO USE VARCHAR(20)
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS guild_{guildId} (userId VARCHAR(20) PRIMARY KEY NOT NULL)"; //Create table with userId as primary key (I hate null and discord ids) (maybe i shoulve used varchar?), changed. IMPORTANT: ALTER CURRENT TABLES TO USE VARCHAR(20)
            cmd.ExecuteNonQuery(); 
            return true; 
        }
        catch (Exception ex) //Catch any errors
        {
            Console.WriteLine($"Error creating table for guild {guildId}: {ex.Message}");
            return false;
        }
    }

    internal static bool addColumn(ulong guildId)
    {
        try
        {
            using var cmd = _conn.CreateCommand();

            // Check if the column 'points' exists
            cmd.CommandText = $"PRAGMA table_info(guild_{guildId})";
            using (var reader = cmd.ExecuteReader())
            {
                bool pointsColumnExists = false;
                bool pointsPendingColumnExists = false;

                while (reader.Read())
                {
                    string columnName = reader.GetString(1); // Column name is in the second column of PRAGMA table_info
                    if (columnName == "points")
                        pointsColumnExists = true;
                    if (columnName == "points_pending")
                        pointsPendingColumnExists = true;
                }
            } // Reader is disposed here

            // Add 'points' column if it doesn't exist
            cmd.CommandText = $"ALTER TABLE guild_{guildId} ADD COLUMN points INTEGER DEFAULT 0";
            cmd.ExecuteNonQuery();

            // Add 'points_pending' column if it doesn't exist
            cmd.CommandText = $"ALTER TABLE guild_{guildId} ADD COLUMN points_pending INTEGER DEFAULT 0";
            cmd.ExecuteNonQuery();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding columns to table for guild {guildId}: {ex.Message}");
            return false;
        }
    }
    internal static bool addPoints(ulong guildId, ulong userId, int points,string table="points") //Add points to a user
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = $"INSERT INTO guild_{guildId} (userId, {table}) VALUES (@userId, @points) ON CONFLICT(userId) DO UPDATE SET {table} = {table} + @points"; //Insert user if they don't exist, otherwise update their points
            cmd.Parameters.AddWithValue("@userId", userId.ToString()); //Using string for userId to avoid issues with large numbers
            cmd.Parameters.AddWithValue("@points", points); //Points to add
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex) //Catch any errors
        {
            Console.WriteLine($"Error adding points to user {userId} in guild {guildId}: {ex.Message}");
            return false;
        }
    }
    internal static string getPoints(ulong guildId, ulong userId) //Get points for a user
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = $"SELECT points, points_pending FROM guild_{guildId} WHERE userId = @userId"; //Select points and pending points for user
            cmd.Parameters.AddWithValue("@userId", userId.ToString()); //Using string for userId to avoid issues with large numbers
            using var reader = cmd.ExecuteReader(); //
            if (reader.Read()) //Return points if user exists
            {
                int points, pendingPoints;
                points = reader.GetInt32(0);
                pendingPoints = reader.GetInt32(1);
                return points.ToString() +" points and "+ pendingPoints.ToString() +" pending points";
            }
            return "You dont have any points!"; //User doesn't exist
            
        }
        catch (Exception ex) //error handling
        {
            Console.WriteLine($"Error retrieving points for user {userId} in guild {guildId}: {ex.Message}");
            return "Error retrieving points.";
        }
    }

    internal static bool syncPendingToMain(ulong guildId,ulong userId)
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = $"UPDATE guild_{guildId} SET points = points + points_pending, points_pending = 0 WHERE userId = @userId"; //Add pending points to main points and reset pending points to 0
            cmd.Parameters.AddWithValue("@userId", userId.ToString()); //Using string for userId to avoid issues with large numbers
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error syncing pending points for user {userId} in guild {guildId}: {ex.Message}");
            return false;
        }
    }
}