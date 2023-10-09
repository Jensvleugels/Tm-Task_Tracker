using System.Data;
using System.Data.SqlClient;
using Dapper;
using Task = TaskTrackerModels.Task;

namespace TaskTrackerDal
{
    public class TaskRepository : BaseRepository, ITaskRepository
    {
        public List<Task> GetAllTasks()
        {
            string sql = @"SELECT * FROM Tasktracker.Tasks";

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Task>(sql).ToList();
            }
        }

        public Task GetTaskById(Guid id)
        {
            string sql = @"SELECT * FROM Tasktracker.Tasks WHERE id = @id";

            var parameters = new { @id = id.ToString() };
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.QuerySingleOrDefault<Task>(sql, parameters);
            }
        }

        public bool AddTask(Task task)
        {
            string sql = @"INSERT INTO Tasktracker.Tasks (id, name, description, status, startedAt, finishedAt, createdAt, lastUpdatedAt)
                            VALUES (@id, @name, @description, @taskstatus, @startedAt, @finishedAt, @createdAt, @lastUpdatedAt)";

            var parameters = new
            {
                @id = task.Id.ToString(),
                @name = task.Name,
                @description = task.Description,
                @taskstatus = task.Status,
                @startedAt = task.StartedAt,
                @finishedAt = task.FinishedAt,
                @createdAt = task.CreatedAt,
                @lastUpdatedAt = task.LastUpdatedAt
                //@lastUpdatedAt = DateTime.Now
            };

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Execute(sql, parameters) == 1;
            }
        }

        public bool UpdateTask(Task task)
        {
            string sql = @"UPDATE Tasktracker.Tasks SET
                            name = @name,
                            description = @description,
                            status = @taskstatus,
                            startedAt = @startedAt,
                            finishedAt = @finishedAt,
                            lastUpdatedAt = @lastUpdatedAt
                            WHERE id = @id";

            var parameters = new
            {
                @id = task.Id.ToString(),
                @name = task.Name,
                @description = task.Description,
                @taskstatus = task.Status,
                @startedAt = task.StartedAt,
                @finishedAt = task.FinishedAt,
                @lastUpdatedAt = task.LastUpdatedAt
                //@lastUpdatedAt = DateTime.Now
            };

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Execute(sql, parameters) == 1;
            }
        }

        public bool DeleteTask(Guid id)
        {
            string sql = @"DELETE FROM Tasktracker.Comments WHERE taskId = @id;
                           DELETE FROM Tasktracker.Tasks WHERE id = @id";

            var parameters = new { @id = id.ToString() };

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Execute(sql, parameters) >= 1;
            }
        }
    }
}
