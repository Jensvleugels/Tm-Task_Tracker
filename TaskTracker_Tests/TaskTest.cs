using TaskTrackerModels;
using Task = TaskTrackerModels.Task;


namespace TaskTracker_Tests
{
    [TestFixture]
    public class TaskTest
    {
        Task task = new Task();
        [Test]
        public void Set_TaskName_TooShort()
        {
            //Arrange
            task.Name = "12";

            //Act

            //Assert
            Assert.That(task.IsValid(), Is.False);
        }
        [Test]
        public void Set_TaskName_TooLong()
        {
            //Arrange
            task.Name = "123456789012345678901234567890123456789012345678901";

            //Act

            //Assert
            Assert.That(task.IsValid(), Is.False);
        }
        [Test]
        public void Set_TaskDescription_TooLong()
        {
            //Arrange
            task.Name = "1234";
            task.Description = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901";

            //Act

            //Assert
            Assert.That(task.IsValid(), Is.False);
        }

        Comment comment = new Comment();
        [Test]
        public void Set_CommentText_Empty()
        {
            //Arrange
            comment.Text = "";

            //Act

            //Assert
            Assert.That(comment.IsValid(), Is.False);
        }
    }
}