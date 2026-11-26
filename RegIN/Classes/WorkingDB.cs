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
    }
}
