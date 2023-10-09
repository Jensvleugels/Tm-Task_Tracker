namespace TaskTrackerModels
{
    public class Task : BaseClass
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public List<Comment> Comments { get; set; }
        public enum TaskStatus
        {
            Open,
            InProgress,
            Postponed,
            Done
        }

        public Task()
        {
            Id = Guid.NewGuid();
            Name = "";
            Description = "";
            Status = TaskStatus.Open;
            CreatedAt = DateTime.Now;
            LastUpdatedAt = DateTime.Now;
            Comments = new List<Comment>();
        }
        public Task(string name, string desc) : this()
        {
            this.Name = name;
            Description = desc;            
        }

        public override string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Name) && string.IsNullOrWhiteSpace(Name))
                {
                    return "Task name cannot be empty";
                }
                else if (columnName == nameof(Name) && Name.Length < 3)
                {
                    return "Task name has to be 3 characters long or more";
                }
                else if (columnName == nameof(Name) && Name.Length > 50)
                {
                    return "Task name has to be 50 characters long or less";
                }
                else if (columnName == nameof(Description) && Description.Length > 120)
                {
                    return "Task description can't be longer than 120 characters";
                }
                return "";
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Task task &&
                   Id.Equals(task.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string? ToString()
        {
            return $"{this.Name} - {this.Status}";
        }

        public bool IsUpdated(Task toCompare)
        {
            return this.Name != toCompare.Name ||
                   this.Description != toCompare.Description ||
                   this.Status != toCompare.Status ||
                   this.StartedAt != toCompare.StartedAt ||
                   this.FinishedAt != toCompare.FinishedAt;
        }
    }
    
}