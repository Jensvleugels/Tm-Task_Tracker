using System.ComponentModel;

namespace TaskTrackerModels
{
    public abstract class BaseClass : IDataErrorInfo
    {
        public abstract string this[string columnName] { get; }
        public string Error
        {
            get
            {
                string error = string.Empty;
                foreach (var property in this.GetType().GetProperties())
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        string propertyError = this[property.Name];
                        if (!string.IsNullOrWhiteSpace(propertyError))
                        {
                            error += propertyError + Environment.NewLine;
                        }
                    }
                }
                return error;
            }
        }

        public bool IsValid()
        {
            return string.IsNullOrWhiteSpace(Error);
        }

    }
}
