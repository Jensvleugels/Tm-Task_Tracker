namespace TaskTrackerDal
{
    public abstract class BaseRepository
    {
        protected string ConnectionString { get; set; }
        public BaseRepository()
        {
            ConnectionString = DatabaseConnection.ConnectionString("TasktrackerConnectionString");
        }
    }
}
