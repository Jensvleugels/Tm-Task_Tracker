using System.Data.SqlClient;
using System.Data;
using Dapper;
using TaskTrackerModels;

namespace TaskTrackerDal
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        public List<Comment> GetAllComments()
        {
            string sql = @"SELECT * FROM Tasktracker.Comments";

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Comment>(sql).ToList();
            }
        }

        public List<Comment> GetAllCommentsByTaskId(Guid id)
        {
            string sql = @"SELECT * FROM Tasktracker.Comments WHERE taskId = @taskId";

            var parameters = new { @taskId = id.ToString() };
            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Query<Comment>(sql, parameters).ToList();
            }
        }

        public Comment GetCommentById(Guid id)
        {
            string sql = @"SELECT * FROM Tasktracker.Comments WHERE id = @id";

            var parameters = new { @id = id.ToString() };

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.QuerySingleOrDefault<Comment>(sql);
            }
        }

        public bool AddComment(Comment comment)
        {
            string sql = @"INSERT INTO Tasktracker.Comments (id, text, createdAt, taskIsPostponed, taskId)
                            VALUES (@id, @text, @createdAt, @isPostponed, @taskId)";

            var parameters = new
            {
                @id = comment.Id.ToString(),
                @text= comment.Text,
                @createdAt= comment.CreatedAt,
                @isPostponed = comment.TaskIsPostponed,
                @taskId = comment.TaskId.ToString()
            };

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Execute(sql, parameters) == 1;
            }
        }

        public bool UpdateComment(Comment comment)
        {
            string sql = @"UPDATE Tasktracker.Comments SET
                            text = @text
                            WHERE id = @id";

            var parameters = new
            {
                @id = comment.Id.ToString(),
                @text = comment.Text
            };

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Execute(sql, parameters) == 1;
            }
        }

        public bool DeleteComment(Guid id)
        {
            string sql = @"DELETE FROM Tasktracker.Comments WHERE id = @id";

            var parameters = new { @id = id.ToString() };

            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                return db.Execute(sql, parameters) == 1;
            }
        }
    }
}
