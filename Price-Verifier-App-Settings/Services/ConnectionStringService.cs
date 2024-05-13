namespace Price_Verifier_App_Settings.Services
{
    internal class ConnectionStringService
    {
        private static string _connectionString;
        private static DatabaseConfig _config = new DatabaseConfig();

        public static string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        static ConnectionStringService()
        {
            // Set the initial connection string
            _connectionString = $"server={_config.Server};port={_config.Port};uid={_config.Uid};pwd={_config.Pwd};database={_config.Database}";
        }
    }
}
