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
namespace Tosca.Core
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using log4net;
    using StructureMap;

    public class StructureMapControllerFactory :
        DefaultControllerFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (StructureMapControllerFactory));

        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            Type controllerType = base.GetControllerType(controllerName);

            return GetControllerInstance(controllerType);
        }

        protected override IController GetControllerInstance(Type controllerType)
        {
            if (controllerType == null)
                return null;

            try
            {
                return ObjectFactory.GetInstance(controllerType) as IController;
            }
            catch (Exception ex)
            {
                _log.Error("Unable to resolve controller of type: " + controllerType.FullName, ex);
                throw;
            }
        }
    }
}