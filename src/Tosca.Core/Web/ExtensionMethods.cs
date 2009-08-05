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
namespace Tosca.Core.Web
{
    using System;
    using System.IO;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.Web.Mvc.Internal;

    public static class ExtensionMethods
    {
        public static string Action<TController>(this UrlHelper url, Expression<Action<TController>> action)
            where TController : Controller
        {
            var routeValueDictionary = ExpressionHelper.GetRouteValuesFromExpression(action);

            return url != null ? url.Action(null, routeValueDictionary) : null;
        }

        public static string BuildPageIdentifier(this ViewMasterPage masterPage, ViewContext context)
        {
            return context.RouteData.Values["controller"] + "-" + context.RouteData.Values["action"];
        }

        public static string BuildPageIdentifier(this ViewPage page, ViewContext context)
        {
            return context.RouteData.Values["controller"] + "-" + context.RouteData.Values["action"];
        }

        /// <summary>
        /// Captures the HTML output by a controller action that returns a ViewResult
        /// </summary>
        /// <typeparam name="TController">The type of controller to execute the action on</typeparam>
        /// <param name="controller">The controller</param>
        /// <param name="action">The action to execute</param>
        /// <returns>The HTML output from the view</returns>
        public static string CaptureActionHtml<TController>(
            this TController controller,
            Func<TController, ViewResult> action)
            where TController : Controller
        {
            return controller.CaptureActionHtml(controller, null, action);
        }

        /// <summary>
        /// Captures the HTML output by a controller action that returns a ViewResult
        /// </summary>
        /// <typeparam name="TController">The type of controller to execute the action on</typeparam>
        /// <param name="controller">The controller</param>
        /// <param name="masterPageName">The master page to use for the view</param>
        /// <param name="action">The action to execute</param>
        /// <returns>The HTML output from the view</returns>
        public static string CaptureActionHtml<TController>(
            this TController controller,
            string masterPageName,
            Func<TController, ViewResult> action)
            where TController : Controller
        {
            return controller.CaptureActionHtml(controller, masterPageName, action);
        }

        /// <summary>
        /// Captures the HTML output by a controller action that returns a ViewResult
        /// </summary>
        /// <typeparam name="TController">The type of controller to execute the action on</typeparam>
        /// <param name="controller">The current controller</param>
        /// <param name="targetController">The controller which has the action to execute</param>
        /// <param name="action">The action to execute</param>
        /// <returns>The HTML output from the view</returns>
        public static string CaptureActionHtml<TController>(
            this Controller controller,
            TController targetController,
            Func<TController, ViewResult> action)
            where TController : Controller
        {
            return controller.CaptureActionHtml(targetController, null, action);
        }

        /// <summary>
        /// Captures the HTML output by a controller action that returns a ViewResult
        /// </summary>
        /// <typeparam name="TController">The type of controller to execute the action on</typeparam>
        /// <param name="controller">The current controller</param>
        /// <param name="targetController">The controller which has the action to execute</param>
        /// <param name="masterPageName">The name of the master page for the view</param>
        /// <param name="action">The action to execute</param>
        /// <returns>The HTML output from the view</returns>
        public static string CaptureActionHtml<TController>(
            this Controller controller,
            TController targetController,
            string masterPageName,
            Func<TController, ViewResult> action)
            where TController : Controller
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }
            if (targetController == null)
            {
                throw new ArgumentNullException("targetController");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            // pass the current controller context to orderController
            var controllerContext = controller.ControllerContext;
            targetController.ControllerContext = controllerContext;

            // replace the current context with a new context that writes to a string writer
            var existingContext = HttpContext.Current;
            var writer = new StringWriter();
            var response = new HttpResponse(writer);
            var context = new HttpContext(existingContext.Request, response) {};
            HttpContext.Current = context;

            // execute the action
            var viewResult = action(targetController);

            // change the master page name
            if (masterPageName != null)
            {
                viewResult.MasterName = masterPageName;
            }

            // we have to set the controller route value to the name of the controller we want to execute
            // because the ViewLocator class uses this to find the correct view
            var oldController = controllerContext.RouteData.Values["controller"];
            controllerContext.RouteData.Values["controller"] = typeof (TController).Name.Replace("Controller", "");

            // execute the result
            viewResult.ExecuteResult(controllerContext);

            // restore the old route data
            controllerContext.RouteData.Values["controller"] = oldController;

            // restore the old context
            HttpContext.Current = existingContext;

            return writer.ToString();
        }
    }
}