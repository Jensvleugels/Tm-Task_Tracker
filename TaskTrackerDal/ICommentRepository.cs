using TaskTrackerModels;

namespace TaskTrackerDal
{
    public interface ICommentRepository
    {
        public List<Comment> GetAllComments();
        public List<Comment> GetAllCommentsByTaskId(Guid id);
        public Comment GetCommentById(Guid id);
        public bool AddComment(Comment comment);
        public bool UpdateComment(Comment comment);
        public bool DeleteComment(Guid id);
    }
}
