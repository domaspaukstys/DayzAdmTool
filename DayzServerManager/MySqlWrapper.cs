using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace DayzServerManager
{
    public class MySqlWrapper : IDisposable
    {
        private readonly MySqlConnection _connection;

        public MySqlWrapper()
        {
            _connection = new MySqlConnection();
        }

        public void UpdateConnection(string server, int port, string database, string user, string password)
        {
            _connection.ConnectionString = string.Format(@"Server={0};Port={1};Database={2};Uid={3};Pwd={4};", server,
                port, database, user, password);
        }

        public bool TryOpenConnection()
        {
            bool result = false;
            try
            {
                if(_connection.State != ConnectionState.Open)
                    _connection.Open();
                result = true;
            }
            catch{}
            return result;
        }

        private DataTable GetData(string sql)
        {
            DataTable table = null;
            if (TryOpenConnection())
            {
                MySqlCommand command = _connection.CreateCommand();
                command.CommandText = sql;
                MySqlDataReader mySqlDataReader = command.ExecuteReader();
                table = new DataTable();
                table.Load(mySqlDataReader);
                _connection.Close();
            }
            return table;
        }

        public void Dispose()
        {
            _connection.Dispose();
        }

        public DataTable GetVehicles()
        {
            return GetData("SELECT ObjectUID, Classname, Worldspace, Damage, Fuel, Hitpoints, Inventory FROM object_data");
        }

        public DataTable GetPlayers()
        {
            return GetData("SELECT pd.PlayerUID, pd.playerName, cd.Worldspace, cd.Inventory, cd.Alive FROM character_data cd JOIN player_data pd ON pd.PlayerUID = cd.playerUID;");
        }
    }
}
