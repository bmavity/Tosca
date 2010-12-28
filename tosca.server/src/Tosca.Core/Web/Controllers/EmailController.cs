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
namespace Tosca.Core.Web.Controllers
{
    using System.Diagnostics;
    using System.Net.Mail;
    using System.Web.Mvc;
    using ViewModels;

    public class EmailController :
        Controller
    {
        [NonAction]
        public void GenerateEmail(WelcomeEmailViewModel model)
        {
            string subject = "Welcome " + model.FirstName;

            string body = this.CaptureActionHtml(c => c.Welcome(model));

            Trace.WriteLine("Subject: " + subject);
            Trace.WriteLine("Body:");
            Trace.WriteLine(body);
        }


        public ViewResult Welcome(WelcomeEmailViewModel model)
        {
            return View("Welcome", model);
        }

        public ViewResult Render()
        {
            var model = new WelcomeEmailViewModel();
            model.FirstName = "Chris";

            GenerateEmail(model);

            return View(model);
        }
    }
}