namespace TaskTrackerModels
{
    public class Comment : BaseClass
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool TaskIsPostponed { get; set; }
        public Guid? TaskId { get; set; }

        public Comment()
        {
            Id = Guid.NewGuid();
            Text = "";
            CreatedAt = DateTime.Now;
            TaskIsPostponed = false;
        }
        public Comment(string desc) : this()
        {
            Text = desc;
        }
        public Comment(string desc, Guid taskId) : this(desc)
        {
            TaskId = taskId;
        }
        public Comment(string desc, bool postponed, Guid taskId) : this(desc, taskId)
        {
            TaskIsPostponed = postponed;            
        }

        public override string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Text) && string.IsNullOrWhiteSpace(Text))
                {
                    return "Comment cannot be empty";
                }
                else if (columnName == nameof(CreatedAt) && (CreatedAt < new DateTime(1900, 01, 01) || CreatedAt > DateTime.Now))
                {
                    return "Invalid creation date";
                }
                return "";
            }
        }

        public override bool Equals(object? obj)
        {
            return obj is Comment comment &&
                   Id.Equals(comment.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string? ToString()
        {
            string postponed = TaskIsPostponed ? " - Postponed" : "";
            return $"{this.CreatedAt}{postponed} - {this.Id}";
        }

        public bool IsUpdated(Comment toCompare)
        {
            return this.Text != toCompare.Text;
        }
    }
}
