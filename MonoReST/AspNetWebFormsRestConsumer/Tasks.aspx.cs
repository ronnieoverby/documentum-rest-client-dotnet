using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.DataModel.D2;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace AspNetWebFormsRestConsumer
{
    public partial class Tasks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Global.GetRepository() == null) Response.Redirect("Default.aspx");
            else PopulateTaskGrid(Global.GetRepository());
        }

        protected void PopulateTaskGrid(D2Repository repository)
        {
            if (!repository.isD2Rest())
            {
                NoteLabel.ForeColor = Color.OrangeRed;
                NoteLabel.Text = String.Format("The target repository '{0}' is NOT a D2 repository.", repository.Name);
                return;
            }

            NoteLabel.ForeColor = Color.LightGreen;
            NoteLabel.Text = String.Format("The target repository '{0}' is a D2 repository.", repository.Name);
            Feed<D2Task> taskFeed = repository.GetD2TaskList();
            List<D2Task> tasks = ObjectUtil.getFeedAsList(taskFeed);
            StringBuilder taskStatus = new StringBuilder();
            List<TaskProperties> taskProperties = new List<TaskProperties>();
            foreach (D2Task task in tasks)
            {
                TaskProperties tp = new TaskProperties();
                tp.Subject = task.TaskSubject;
                tp.Instructions = task.TaskInstructions;
                StringBuilder forwardTasks = new StringBuilder();
                bool firstPass = true;
                foreach (String key in task.TaskRequirements.ForwardTasks.Keys)
                {
                    if (firstPass)
                    {
                        firstPass = false;
                        forwardTasks.Append(key);
                    }
                    else forwardTasks.Append(", " + key);
                }
                tp.forwardTasks = forwardTasks.ToString();
                taskProperties.Add(tp);
            }
            taskGrid.DataSource = taskProperties;
            taskGrid.DataBind();
        }
    }
    class TaskProperties
    {
        public String Subject { get; set; }
        public String Instructions { get; set; }
        public String forwardTasks { get; set; }
    }
}