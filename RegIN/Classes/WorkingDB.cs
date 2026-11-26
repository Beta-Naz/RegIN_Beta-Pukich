using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;

namespace RegIN.Classes
{
    public class WorkingDB
    {
        private readonly static string _connectionString = @$"server=localhost;database=regin;user=root;pwd=;";
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
    }
}
