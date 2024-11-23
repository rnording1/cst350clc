using Microsoft.Data.SqlClient;

namespace cst350groupapp.Models
{
    public class GameDAO : IGameManager
    {
        private readonly string connectionString = "Server=tcp:cst350dbserver.database.windows.net,1433;Initial Catalog=cst250db;Persist Security Info=False;User ID=CST350Student;Password=AzureRocks!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public int CheckCredentials(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT Id, PasswordHash, Salt FROM Users WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserModel user = new UserModel
                            {
                                Id = reader.GetInt32(0),
                                PasswordHash = reader.GetString(1),
                                Salt = reader.GetSqlBinary(2).Value
                            };
                            if (user.VerifyPassword(password))
                            {
                                return user.Id;
                            }
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    return -1;
                }
            }
        }

        public void DeleteGame(GameModel game)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM Games WHERE Id = @Id AND UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@Id", game.Id);
                    command.Parameters.AddWithValue("@UserId", game.PlayerId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<GameModel> GetAllGames(int playerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT Id, UserId, DateSaved, GameData FROM Games WHERE UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", playerId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        List<GameModel> games = new List<GameModel>();
                        while (reader.Read())
                        {
                            GameModel game = new GameModel
                            {
                                Id = reader.GetInt32(0),
                                PlayerId = reader.GetInt32(1),
                                DateSaved = reader.GetDateTime(2),
                                GameData = reader.GetString(3)
                            };
                            games.Add(game);
                        }
                        return games;
                    }
                }
            }
        }

        public GameModel GetGameById(int id, int playerId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT Id, UserId, DateSaved, GameData FROM Games WHERE Id = @Id AND UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@UserId", playerId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            GameModel game = new GameModel
                            {
                                Id = reader.GetInt32(0),
                                PlayerId = reader.GetInt32(1),
                                DateSaved = reader.GetDateTime(2),
                                GameData = reader.GetString(3)
                            };
                            return game;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public int SaveGame(GameModel game)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO Games (UserId, DateSaved, GameData) VALUES (@UserId, @DateSaved, @GameData); SELECT SCOPE_IDENTITY();", connection))
                {
                    command.Parameters.AddWithValue("@UserId", game.PlayerId);
                    command.Parameters.AddWithValue("@DateSaved", DateTime.Now);
                    command.Parameters.AddWithValue("@GameData", game.GameData);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        public void UpdateGame(GameModel game)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("UPDATE Games SET UserId = @UserId, DateSaved = @DateSaved, GameData = @GameData WHERE Id = @Id AND UserId = @UserId", connection))
                {
                    command.Parameters.AddWithValue("@UserId", game.PlayerId);
                    command.Parameters.AddWithValue("@DateSaved", DateTime.Now);
                    command.Parameters.AddWithValue("@GameData", game.GameData);
                    command.Parameters.AddWithValue("@Id", game.Id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
