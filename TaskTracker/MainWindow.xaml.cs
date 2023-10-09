using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Task = TaskTrackerModels.Task;
using TaskTrackerDal;
using TaskTrackerModels;

namespace TaskTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskRepository taskRepository = new TaskRepository();
        CommentRepository commentRepository = new CommentRepository();
        List<Task> tasks;

        public MainWindow()
        {
            InitializeComponent();
            tasks = taskRepository.GetAllTasks();
            //Fill all components
            RefreshTasks();
            cmbOrderby.ItemsSource = new List<string>() { "CreatedAt", "FinishedAt", "Name" };
            cmbOrderby.SelectedIndex = 0;
            List<string> filterBy = new List<string>() { "All" };
            foreach (var item in Enum.GetValues(typeof(Task.TaskStatus)))
            {
                filterBy.Add(item.ToString());
            };
            cmbFilterBy.ItemsSource = filterBy;
            cmbFilterBy.SelectedIndex = 0;
        }

        //Deselect current task and clear fields for new task
        private void btnTaskAdd_Click(object sender, RoutedEventArgs e)
        {
            lstTasks.SelectedIndex = -1;
            UpdateButtons();
        }

        //Save new task or update existing one
        private void btnTaskSave_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedIndex == -1)
            {
                //Create new task
                Task newTask = new Task(txtTaskName.Text, txtTaskDesc.Text);
                if (newTask.IsValid())
                {
                    if (taskRepository.AddTask(newTask))
                    {
                        //if storing task to db is successful, add it to the list and update listbox
                        tasks.Add(newTask);
                        cmbFilterBy.SelectedIndex = 0;
                        RefreshTasks(newTask);
                    }
                }
                else
                {
                    MessageBox.Show(newTask.Error);
                    ClearForm();
                }
            }
            else
            {
                //Update selected task
                Task task = new Task()
                {
                    Id = (lstTasks.SelectedItem as Task).Id,
                    Name = txtTaskName.Text,
                    Description = txtTaskDesc.Text,
                    Status = (Task.TaskStatus)Enum.Parse(typeof(Task.TaskStatus), txtTaskState.Text),
                    CreatedAt = String.IsNullOrWhiteSpace(txtTaskCreated.Text) ? DateTime.Now : DateTime.Parse(txtTaskCreated.Text),
                    StartedAt = String.IsNullOrWhiteSpace(txtTaskStarted.Text) ? null : DateTime.Parse(txtTaskStarted.Text),
                    FinishedAt = String.IsNullOrWhiteSpace(txtTaskFinished.Text) ? null : DateTime.Parse(txtTaskFinished.Text),
                    LastUpdatedAt = DateTime.Now                    
                };

                //check if task was changed and it's status is not Done
                if (task.IsUpdated(lstTasks.SelectedItem as Task) && task.Status != Task.TaskStatus.Done)
                {
                    if (!task.IsValid())
                    {
                        MessageBox.Show(task.Error);
                        FillForm(lstTasks.SelectedItem as Task);
                        return;
                    }
                    if (taskRepository.UpdateTask(task))
                    {
                        //On successful update, update the list and listbox
                        tasks[tasks.FindIndex(x => x.Id == task.Id)] = task;
                        RefreshTasks(task);
                    }
                }
                else MessageBox.Show("No changes were made");
            }            
        }

        private void btnTaskRemove_Click(object sender, RoutedEventArgs e)
        {
            //In progress tasks can't be removed
            if (lstTasks.SelectedIndex != -1 && ((Task)lstTasks.SelectedItem).Status != Task.TaskStatus.InProgress)
            {
                if (taskRepository.DeleteTask((lstTasks.SelectedItem as Task).Id))
                {
                    //On successful delete, remove task from list and update listbox
                    tasks.Remove(lstTasks.SelectedItem as Task);
                    RefreshTasks();
                }
            }

        }

        private void btnTaskStart_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedIndex != -1)
            {
                Task task = lstTasks.SelectedItem as Task;
                if (task.StartedAt == null) task.StartedAt = DateTime.Now;
                if (task.Status == Task.TaskStatus.Open || task.Status == Task.TaskStatus.Postponed) task.Status = Task.TaskStatus.InProgress;
                if (taskRepository.UpdateTask(task))
                {
                    tasks[tasks.FindIndex(x => x.Id == task.Id)] = task;
                    cmbFilterBy.SelectedIndex = 0;
                    RefreshTasks(task);
                }
            }

        }

        private void btnTaskPostpone_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedIndex == -1) return;
            
            Task task = lstTasks.SelectedItem as Task;
            if (task.Status == Task.TaskStatus.InProgress)
            {
                //show an inputbox to enter a reason for postponing and put it in a string
                string reason = Microsoft.VisualBasic.Interaction.InputBox("Enter a reason for postponing", "Postpone reason", "");
                if (string.IsNullOrWhiteSpace(reason))
                {
                    MessageBox.Show("A reason for postponing is mandatory.", "Postpone reason", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                task.Status = Task.TaskStatus.Postponed;
                task.LastUpdatedAt = DateTime.Now;

                if (taskRepository.UpdateTask(task))
                {
                    //On successful update, add a comment with the reason for postponing
                    commentRepository.AddComment(new Comment(reason, true, task.Id));

                    //Update list and listboxes
                    tasks[tasks.FindIndex(x => x.Id == task.Id)] = task;
                    cmbFilterBy.SelectedIndex = 0;
                    RefreshTasks(task);
                    lstComments.ItemsSource = commentRepository.GetAllCommentsByTaskId(task.Id).OrderBy(x => x.CreatedAt).ToList();
                }
            }            
        }

        private void btnTaskFinish_Click(object sender, RoutedEventArgs e)
        {
            if (lstTasks.SelectedIndex == -1) return;

            Task task = lstTasks.SelectedItem as Task;
            //Only in progress tasks can be finished
            if (task.Status == Task.TaskStatus.InProgress)
            {
                task.Status = Task.TaskStatus.Done;
                task.FinishedAt = DateTime.Now;
                task.LastUpdatedAt = DateTime.Now;

                if (taskRepository.UpdateTask(task))
                {
                    //On successful update, update list and listboxes
                    tasks[tasks.FindIndex(x => x.Id == task.Id)] = task;
                    cmbFilterBy.SelectedIndex = 0;
                    RefreshTasks(task);
                }
            }
        }

        //Apply chosen filter and order by to task listbox and fill task fields with selected task
        private void RefreshTasks(Task? task = null)
        {
            //filter
            if (cmbFilterBy.SelectedIndex != -1 && cmbFilterBy.SelectedIndex != 0)
              lstTasks.ItemsSource = tasks.Where(x => x.Status == (Task.TaskStatus)(cmbFilterBy.SelectedIndex - 1));
            else
                lstTasks.ItemsSource = tasks;
            lstTasks.Items.Refresh();
            //order by - CreatedAt is default
            string orderBy = cmbOrderby.SelectedItem == null ? "CreatedAt" : cmbOrderby.SelectedItem.ToString();
            lstTasks.Items.SortDescriptions.Clear();
            System.ComponentModel.ListSortDirection ascdesc = cmbOrderby.SelectedIndex == 1 ? System.ComponentModel.ListSortDirection.Descending : System.ComponentModel.ListSortDirection.Ascending;
            lstTasks.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription(orderBy, ascdesc));
            if (task != null)
            {
                lstTasks.SelectedItem = task;
                FillForm(task);
            }
            else
            {
                lstTasks.SelectedIndex = -1;
                ClearForm();
            }
            UpdateButtons();
        }

        public void btnCommentSave_Click(object sender, RoutedEventArgs e)
        {
            //A task has to be selected to add a comment
            if (lstTasks.SelectedIndex == -1) return;
            if (lstComments.SelectedIndex == -1)
            {
                //Create new comment
                Comment newComment = new Comment(txtCommentText.Text, ((Task)lstTasks.SelectedItem).Id);
                if (newComment.IsValid())
                {
                    if (commentRepository.AddComment(newComment))
                    {
                        //On successful add, update comment listbox
                        lstComments.ItemsSource = commentRepository.GetAllCommentsByTaskId(newComment.TaskId.GetValueOrDefault()).OrderBy(x => x.CreatedAt);
                        lstComments.SelectedItem = newComment;
                    }
                }
                else
                {
                    MessageBox.Show(newComment.Error);
                    ClearCommentForm();
                }
            }
            //Can't update comments
            //else
            //{
            //    //Update selected comment
            //    Comment crntComment = lstComments.SelectedItem as Comment;
            //    Comment newComment = new Comment()
            //    {
            //        Id = crntComment.Id,
            //        Text = txtCommentText.Text
            //    };

            //    if (newComment.IsUpdated(crntComment))
            //    {
            //        if (commentRepository.UpdateComment(newComment))
            //        {
            //            lstComments.ItemsSource = commentRepository.GetAllCommentsByTaskId(crntComment.TaskId.GetValueOrDefault()).OrderBy(x => x.CreatedAt);
            //            lstComments.SelectedItem = newComment;
            //        }
            //    }
            //}
            UpdateButtons();
        }

        private void btnCommentAdd_Click(object sender, RoutedEventArgs e)
        {
            lstComments.SelectedIndex = -1;
            UpdateButtons();
        }

        //Check if a task is selected and fill task fields and comment listbox
        private void lstTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstTasks.SelectedItem != null)
            {
                FillForm(lstTasks.SelectedItem as Task);
                lstComments.ItemsSource = commentRepository.GetAllCommentsByTaskId((lstTasks.SelectedItem as Task).Id).OrderBy(x => x.CreatedAt);
            }
            else
            {
                //If no task is selected, clear task fields and comment listbox
                ClearForm();
                ClearCommentForm();
                lstComments.ItemsSource = null;
            }
            UpdateButtons();
        }

        //Check if a comment is selected and fill comment fields
        private void lstComments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstComments.SelectedItem != null)
            {
                FillCommentForm(lstComments.SelectedItem as Comment);
            }
            else ClearCommentForm();
            UpdateButtons();
        }

        //Apply new order by to task listbox
        private void cmbOrderby_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstTasks.SelectedIndex != -1) RefreshTasks((Task)lstTasks.SelectedItem);
            else RefreshTasks();
        }

        //Apply new filter to task listbox
        private void cmbFilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshTasks();
        }

        public void ClearForm()
        {
            txtTaskId.Text = "";
            txtTaskName.Text = "";
            txtTaskDesc.Text = "";
            txtTaskState.Text = "";
            txtTaskStarted.Text = "";
            txtTaskFinished.Text = "";
            txtTaskCreated.Text = "";
            txtTaskLastUpdated.Text = "";

        }
        public void ClearCommentForm()
        {
            txtCommentText.Text = "";
            txtCommentId.Text = "";
            txtCommentCreated.Text = "";
            chkCommentPostponed.IsChecked = false;
        }
        public void FillForm(Task task)
        {
            txtTaskId.Text = task.Id.ToString();
            txtTaskName.Text = task.Name;
            txtTaskDesc.Text = task.Description;
            txtTaskState.Text = task.Status.ToString();
            txtTaskStarted.Text = task.StartedAt.ToString();
            txtTaskFinished.Text = task.FinishedAt.ToString();
            txtTaskCreated.Text = task.CreatedAt.ToString();
            txtTaskLastUpdated.Text = task.LastUpdatedAt.ToString();
        }
        public void FillCommentForm(Comment comment)
        {
            txtCommentId.Text = comment.Id.ToString();
            txtCommentText.Text = comment.Text;
            txtCommentCreated.Text = comment.CreatedAt.ToString();
            chkCommentPostponed.IsChecked = comment.TaskIsPostponed;
        }

        //Determine what buttons should be visible based on selected task and comment
        public void UpdateButtons()
        {
            if (lstTasks.SelectedIndex == -1)
            {
                btnTaskSave.Visibility = Visibility.Visible;
                btnTaskRemove.Visibility = Visibility.Hidden;
                btnTaskStart.Visibility = Visibility.Hidden;
                btnTaskPostpone.Visibility = Visibility.Hidden;
                btnTaskFinish.Visibility = Visibility.Hidden;

                btnCommentSave.Visibility = Visibility.Hidden;
                btnCommentAdd.Visibility = Visibility.Hidden;
            }
            else
            {
                Task.TaskStatus status = (lstTasks.SelectedItem as Task).Status;
                btnTaskAdd.Visibility = Visibility.Visible;
                btnTaskRemove.Visibility = status == Task.TaskStatus.InProgress ? Visibility.Hidden : Visibility.Visible;
                btnTaskStart.Visibility = (status == Task.TaskStatus.Open || status == Task.TaskStatus.Postponed) ? Visibility.Visible : Visibility.Hidden;
                btnTaskPostpone.Visibility = status == Task.TaskStatus.InProgress ? Visibility.Visible : Visibility.Hidden;
                btnTaskFinish.Visibility = status == Task.TaskStatus.InProgress ? Visibility.Visible : Visibility.Hidden;
                btnTaskSave.Visibility = status == Task.TaskStatus.Done ? Visibility.Hidden : Visibility.Visible;

                //comment buttons visibility
                btnCommentSave.Visibility = Visibility.Hidden;
                btnCommentAdd.Visibility = Visibility.Hidden;
                if (status != Task.TaskStatus.Done)
                {
                    if (lstComments.SelectedIndex == -1) btnCommentSave.Visibility = Visibility.Visible;
                    else btnCommentAdd.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
