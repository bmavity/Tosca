// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace Tosca.Web
{
    using System.IO;
    using System.Web.Mvc;
    using System.Web.Routing;
    using log4net;
    using log4net.Config;

    public class Bootstrapper
    {
        public static void Bootstrap(MvcApplication application)
        {
            BootstrapLogger(application);

            Core.Web.Bootstrapper.Bootstrap();

            RegisterRoutes(RouteTable.Routes);
        }

        private static void BootstrapLogger(MvcApplication application)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(application.Server.MapPath("/log4net.config")));

            LogManager.GetLogger(typeof (Bootstrapper)).Info("AppDomain Started");
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{id}", new
                {
                    controller = "Home", action = "Index", id = ""
                });
        }
    }
}