using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

namespace RegIN.Classes
{
    public class WorkingDB
    {
        private readonly static string _connectionString = @$"server=localhost;database=regin;user=root;pwd=1234;";
        /// <summary>
        /// Создание и открытие подключения к бд
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection? OpenConnection()
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                connection.Open();
                return connection;
            }
            catch(Exception e)
            {
                Debug.WriteLine("Ошибка: " + e.Message);
                return null;
            }
        }
        /// <summary>
        /// Функция выполнения SQL-запросов
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static MySqlDataReader Query(string sql, MySqlConnection connection) => 
            new MySqlCommand(sql, connection).ExecuteReader();
        /// <summary>
        /// Функция закрытие соединения с бд 
        /// </summary>
        /// <param name="connection"></param>
        public static void CloseConnection(MySqlConnection connection)
        {
            connection.Close();
           MySqlConnection.ClearPool(connection);
        }
        /// <summary>
        /// Функция проверки соединения на работоспособность
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool IsOpenConnection(MySqlConnection connection) => 
            connection != null && connection.State == ConnectionState.Open;

        public static void SavePin(string login, string pin)
        {
            using (var connection = OpenConnection())
            {
                if (connection == null)
                    throw new Exception("Нет подключения к БД.");

                using (var command = new MySqlCommand(
                    "UPDATE users SET PinCode = @pin WHERE Login = @login",
                    connection))
                {
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@pin", PinHelper.Hash(pin));

                    if (command.ExecuteNonQuery() != 1)
                        throw new Exception("Не удалось сохранить PIN-код.");
                }
            }
        }

        public static string? GetPin(string login)
        {
            using (var connection = OpenConnection())
            {
                if (connection == null)
                    throw new Exception("Нет подключения к БД.");

                using (var command = new MySqlCommand(
                    "SELECT PinCode FROM users WHERE Login = @login",
                    connection))
                {
                    command.Parameters.AddWithValue("@login", login);

                    object result = command.ExecuteScalar();

                    return result == null || result == DBNull.Value
                        ? null
                        : result.ToString();
                }
            }
        }

        public static bool CheckPin(
            string login, string pin, out string message)
        {
            message = "";

            if (!PinHelper.IsValid(pin))
            {
                message = "PIN must contain 4 digits";
                return false;
            }

            using (var connection = OpenConnection())
            {
                if (connection == null)
                    throw new Exception("Database connection failed");

                using (var transaction = connection.BeginTransaction())
                {
                    int id;
                    int attempts;
                    string savedHash;
                    DateTime? blockedUntil;

                    using (var command = new MySqlCommand(
                        @"SELECT Id, PinCode, PinAttempts, PinBlockedUntil
                  FROM users
                  WHERE Login = @login
                  FOR UPDATE",
                        connection, transaction))
                    {
                        command.Parameters.AddWithValue("@login", login);

                        using (var reader = command.ExecuteReader())
                        {
                            if (!reader.Read() || reader.IsDBNull(1))
                            {
                                message = "Login or PIN is incorrect";
                                return false;
                            }

                            id = reader.GetInt32(0);
                            savedHash = reader.GetString(1);
                            attempts = reader.GetInt32(2);

                            blockedUntil = reader.IsDBNull(3)
                                ? null
                                : reader.GetDateTime(3);
                        }
                    }

                    if (blockedUntil.HasValue &&
                        blockedUntil.Value > DateTime.Now)
                    {
                        message = "PIN login is temporarily blocked";
                        return false;
                    }

                    bool correct = PinHelper.Verify(pin, savedHash);

                    if (correct)
                    {
                        attempts = 0;
                        blockedUntil = null;
                    }
                    else
                    {
                        attempts++;

                        if (attempts >= 3)
                        {
                            attempts = 0;
                            blockedUntil = DateTime.Now.AddMinutes(3);
                            message = "Too many attempts. Wait 3 minutes";
                        }
                        else
                        {
                            blockedUntil = null;
                            message = $"Incorrect PIN. {3 - attempts} attempts left";
                        }
                    }

                    using (var command = new MySqlCommand(
                        @"UPDATE users
                  SET PinAttempts = @attempts,
                      PinBlockedUntil = @blocked
                  WHERE Id = @id",
                        connection, transaction))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@attempts", attempts);
                        command.Parameters.AddWithValue(
                            "@blocked",
                            (object?)blockedUntil ?? DBNull.Value);

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();

                    return correct;
                }
            }
        }

        public static void InitializeDatabase()
        {
            var builder = new MySqlConnectionStringBuilder(_connectionString);
            builder.Database = "";

            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                connection.Open();

                string sql = @"
            CREATE DATABASE IF NOT EXISTS regin
            CHARACTER SET utf8mb4;

            USE regin;

            CREATE TABLE IF NOT EXISTS users (
                Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
                Login VARCHAR(50) NOT NULL,
                Password VARCHAR(50) NOT NULL,
                Name VARCHAR(50) NOT NULL,
                Image BLOB DEFAULT NULL,
                DateUpdate DATETIME NOT NULL,
                DateCreate DATETIME NOT NULL,
                PinCode VARCHAR(128) DEFAULT NULL,
                PinAttempts INT NOT NULL DEFAULT 0,
                PinBlockedUntil DATETIME DEFAULT NULL
            );";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
