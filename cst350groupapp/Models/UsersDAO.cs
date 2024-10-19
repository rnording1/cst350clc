using Microsoft.Data.SqlClient;

namespace cst350groupapp.Models
{
    public class UsersDAO : IUserManager
    {
        private readonly string connectionString = "Server=tcp:cst350dbserver.database.windows.net,1433;Initial Catalog=cst250db;Persist Security Info=False;User ID=CST350Student;Password=AzureRocks!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public int AddUser(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO Users (FirstName, LastName, Sex, Age, Email, Username, PasswordHash, Salt) VALUES (@FirstName, @LastName, @Sex, @Age, @Email, @Username, @PasswordHash, @Salt); SELECT SCOPE_IDENTITY();", connection))
                {
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Sex", user.Sex);
                    command.Parameters.AddWithValue("@Age", user.Age);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    command.Parameters.AddWithValue("@Salt", user.Salt);
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

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

        public void DeleteUser(UserModel user)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM Users WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", user.Id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<UserModel> GetAllUsers()
        {
            List<UserModel> users = new List<UserModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Users", connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        users.Add(new UserModel
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Username = reader.GetString(reader.GetOrdinal("Username")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            Salt = reader.GetFieldValue<byte[]>(reader.GetOrdinal("Salt")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age")),
                            Sex = reader.GetByte(reader.GetOrdinal("Sex"))
                        });
                    }
                }
                return users;
            }
        }
                            

        public UserModel GetUserById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserModel
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Username = reader.GetString(reader.GetOrdinal("Username")),
                                PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                                Salt = reader.GetFieldValue<byte[]>(reader.GetOrdinal("Salt")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Age = reader.GetInt32(reader.GetOrdinal("Age")),
                                Sex = reader.GetByte(reader.GetOrdinal("Sex"))
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public void UpdateUser(UserModel user)
        {
            int id = user.Id;
            UserModel found = GetUserById(id);
            if (found != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE Users SET FirstName = @FirstName, LastName = @LastName, Email = @Email, Username = @Username, PasswordHash = @PasswordHash, Salt = @Salt, Age = @Age, Sex = @Sex WHERE Id = @Id", connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@FirstName", user.FirstName);
                        command.Parameters.AddWithValue("@LastName", user.LastName);
                        command.Parameters.AddWithValue("@Email", user.Email);
                        command.Parameters.AddWithValue("@Username", user.Username);
                        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                        command.Parameters.AddWithValue("@Salt", user.Salt);
                        command.Parameters.AddWithValue("@Age", user.Age);
                        command.Parameters.AddWithValue("@Sex", user.Sex);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
