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
namespace Tosca.Core.Tests.ControllerTests
{
    using System.Web.Mvc;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Settings;
    using Web.Controllers;

    [TestFixture]
    public class Calling_actions_on_the_home_controller
    {
        [Test]
        public void Should_return_a_view_for_the_index()
        {
            var controller = new HomeController(MockRepository.GenerateMock<ISharedSettings>());

            var result = controller.Index();

            Assert.IsInstanceOfType(typeof (ViewResult), result);
        }

        [Test]
        public void Should_return_a_view_for_the_about()
        {
            var controller = new HomeController(MockRepository.GenerateMock<ISharedSettings>());

            var result = controller.About();

            Assert.IsInstanceOfType(typeof (ViewResult), result);
        }
    }
}