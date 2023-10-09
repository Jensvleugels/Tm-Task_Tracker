using System.Configuration;

namespace TaskTrackerDal
{
    public static class DatabaseConnection
    {
        public static string ConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
