namespace DAL.NH
{
    public class ConnectionStringManager
    {
        public const string DefaultConnectionStringKerberos = @"Data Source={0};User ID=/;Validate Connection=true;Pooling=false;";
        public const string DefaultConnectionStringLoginPswrd = @"Data Source={0};User ID={1};Password={2};Validate Connection=true;Pooling=false;";

        private static string _connectionString;
        private static string _login;
        private static string _password;
        private static string _database;

        public static string GetConnectionString()
        {
            var cs = _connectionString ??
                     (_login == null ? DefaultConnectionStringKerberos : DefaultConnectionStringLoginPswrd);
            return string.Format(cs, _database, _login, _password);
        }

        public static void SetCustomConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static void SetAuthData(string database)
        {
            _database = database;
            _login = null;
            _password = null;
        }

        public static void SetAuthData(string database, string login, string password)
        {
            _database = database;
            _login = login;
            _password = password;
        }
    }
}