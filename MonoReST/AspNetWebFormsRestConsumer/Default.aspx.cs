using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Http.Utility;
using Emc.Documentum.Rest.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AspNetWebFormsRestConsumer
{
    public partial class _Default : Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Global.Repository = null; // set back to null to validate the new credentials.
            Global.Docbase = txtDocbase.Text;
            Global.UserName = txtUserName.Text;
            Global.Password = txtPassword.Text;
            Global.RestServiceURL = txtRestUrl.Text;
            Repository repository = Global.GetRepository();
            
            if(repository != null)
            {
                SingleGetOptions options = new SingleGetOptions();
                options.Inline = true;
                User currentUser = repository.GetCurrentUser(options);
                lblUserInfo.Visible = true;
                lblUserInfo.ForeColor = System.Drawing.Color.Green;
                lblUserInfo.Text = "User " + currentUser.getAttributeValue("user_name").ToString() + " successfully logged in!";
                lblLoginBanner.CssClass = "message-success";
                lblLoginBanner.Text = "You are logged in currently as " + Global.UserName;
            } else
            {
                lblUserInfo.Visible = true;
                lblUserInfo.ForeColor = System.Drawing.Color.Red;
                lblUserInfo.Text = "Login Failed, try again!";
            }
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Global.GetRepository() != null)
            {
                lblLoginBanner.CssClass = "message-success";
                lblLoginBanner.Text = "You are logged in currently as " + Global.UserName;
            }
        }
    }
}