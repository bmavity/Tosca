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
    using System.Linq.Expressions;
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
    }
}