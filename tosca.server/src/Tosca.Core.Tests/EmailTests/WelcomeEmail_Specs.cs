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
namespace Tosca.Core.Tests.EmailTests
{
    using NUnit.Framework;
    using Web.Controllers;
    using Web.ViewModels;

    [TestFixture]
    public class WelcomeEmail_Specs
    {
        [Test, Ignore("No context")]
        public void Should_properly_format_an_email_using_the_view_engine()
        {
            var controller = new EmailController();

            var viewModel = new WelcomeEmailViewModel();

            controller.GenerateEmail(viewModel);
        }
    }

    public interface IEmailRenderer
    {
        string Render(WelcomeEmailViewModel model);
    }
}
