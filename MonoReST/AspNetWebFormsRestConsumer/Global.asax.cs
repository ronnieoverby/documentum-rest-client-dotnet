using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using AspNetWebFormsRestConsumer;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.DataModel.D2;


namespace AspNetWebFormsRestConsumer
{
    public class Global : HttpApplication
    {
        public static String UserName { get; set; }
        public static String Password { get; set; }
        public static String Docbase { get; set; }

        public static D2Repository Repository { get; set; }
        public static bool isAuthenticated { get; set; }

        public static String RestServiceURL { get; set; }

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterOpenAuth();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        public static D2Repository GetRepository()
        {
            if (Repository != null) return Repository;
            if(UserName == null || Password == null || Docbase == null || RestServiceURL == null)
            {
                return null;
            }
            RestController client;
            HomeDocument home;

            if (RestServiceURL == null) RestServiceURL = "http://localhost:8080/dctm-rest/services";
            client = new RestController(UserName, Password);
            home = client.Get<HomeDocument>(RestServiceURL, null);
            home.SetClient(client);
            Repository = home.GetRepository<D2Repository>(Docbase);
            return Repository;
        }
    }
}
