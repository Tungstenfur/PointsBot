using Microsoft.Data.Sqlite;

namespace silly_kronos;

internal static class db
{
    private static SqliteConnection _conn;

    public static bool dbOpen()
    {
        try
        {
            _conn = new SqliteConnection("Data Source=pointsbot.db");
            _conn.Open();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening database: {ex.Message}");
            return false;
        }
    }
    public static bool addTable(ulong guildId)
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS guild_{guildId} (userId TEXT PRIMARY KEY)";
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating table for guild {guildId}: {ex.Message}");
            return false;
        }
    }
    public static bool addColumn(ulong guildId, string columnName)
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = $"ALTER TABLE guild_{guildId} ADD COLUMN {columnName} INTEGER DEFAULT 0";
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding column {columnName} to table for guild {guildId}: {ex.Message}");
            return false;
        }
    }
    public static bool addPoints(ulong guildId, ulong userId, int points, string table = "points")
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = $"INSERT INTO guild_{guildId} (userId, {table}) VALUES (@userId, @points) ON CONFLICT(userId) DO UPDATE SET {table} = {table} + @points";
            cmd.Parameters.AddWithValue("@userId", userId.ToString());
            cmd.Parameters.AddWithValue("@points", points);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding points to user {userId} in guild {guildId}: {ex.Message}");
            return false;
        }
    }
    public static int[] getPoints(ulong guildId, ulong userId)
    {
        try
        {
            using var cmd = _conn.CreateCommand();
            cmd.CommandText = $"PRAGMA table_info(guild_{guildId})";
            using var reader = cmd.ExecuteReader();

            var columns = new List<string>();
            while (reader.Read())
            {
                string columnName = reader.GetString(1);
                if (columnName != "userId")
                    columns.Add(columnName);
            }

            var points = new int[columns.Count];
            for (int i = 0; i < columns.Count; i++)
            {
                cmd.CommandText = $"SELECT {columns[i]} FROM guild_{guildId} WHERE userId = @userId";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@userId", userId.ToString());

                object result = cmd.ExecuteScalar();
                points[i] = result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }

            return points;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving points for user {userId} in guild {guildId}: {ex.Message}");
            return Array.Empty<int>();
        }
    }
}