using System;

namespace DayzServerManager
{
    public class Context
    {
        private MySqlWrapper _wrapper;
        public event EventHandler DBConnectionOpen;

        protected virtual void OnDbConnectionOpen()
        {
            if (DBConnectionOpen != null)
                DBConnectionOpen(this, EventArgs.Empty);
        }

        public static bool ConnectToDB(string server, int port, string database, string user, string password)
        {
            Instance.DbWrapper.UpdateConnection(server, port, database, user, password);
            bool result = Instance.DbWrapper.TryOpenConnection();
            if(result)
                Instance.OnDbConnectionOpen();
            return result;
        }

        public MySqlWrapper DbWrapper { get { return (_wrapper = _wrapper ?? new MySqlWrapper()); } }



        private static Context _instance;

        public static Context Instance
        {
            get { return (_instance = _instance ?? new Context()); }
        }

        private Context()
        {
        }
    }
}
