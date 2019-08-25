using Microsoft.Owin;
using Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using Hangfire;

namespace ToDoList
{
    public partial class Startup : System.Web.UI.Page
    {
        public object ClienScript { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //ClientScript.RegisterStartupScript(this.GetType(), "alert", "javascript:alert('script running');", true);

            //ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Record is not updated');", true);

            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Record Inserted Successfully')", true);

            //Page.RegisterStartupScript("Key", "<script type='text/javascript'>window.onload = function(){alert('Please accept Terms and Conditions before submitting.');return false;}</script>");


            //HttpContext.Current.Response.Write("<script>alert('hello')</script>");

            //System.Diagnostics.Debug.WriteLine("asfsafafs");

            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
