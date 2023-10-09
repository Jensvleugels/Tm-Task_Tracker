using Task = TaskTrackerModels.Task;

namespace TaskTrackerDal
{
    public interface ITaskRepository
    {
        public List<Task> GetAllTasks();
        public Task GetTaskById(Guid id);
        public bool AddTask(Task task);
        public bool UpdateTask(Task task);
        public bool DeleteTask(Guid id);
    }
}
