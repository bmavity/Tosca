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
	using System.Web;
	using System.Web.Routing;
	using Magnum.Actors;
	using StructureMap;

	public class ActorRouteHandler :
		IRouteHandler
	{
		private readonly IContainer _container;
		private readonly Type _type;

		public ActorRouteHandler(IContainer container, Type type)
		{
			_container = container;
			_type = type;
		}

		public IHttpHandler GetHttpHandler(RequestContext requestContext)
		{
			return _container
				.With<Func<AsyncHttpActor>>(() => (AsyncHttpActor) _container.GetInstance(_type))
				.GetInstance<AsyncHttpActorHandler>();
		}
	}
}