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
	using System.Web.Routing;
	using Magnum.Actors;
	using StructureMap;
	using StructureMap.Graph;

	public static class ActorBootstrapper
	{
		private static void RegisterRoutes(IContainer container)
		{
			container.WithEach<AsyncHttpActor>(x =>
				{
					Type handlerType = x.GetType();

					string routeUrl = handlerType.Name;
					if (routeUrl.EndsWith("Actor"))
						routeUrl = routeUrl.Substring(0, routeUrl.Length - 5);

					var route = new Route(routeUrl, new ActorRouteHandler(container, handlerType));

					RouteTable.Routes.Add(route);
				});
		}

		public static void Bootstrap(IContainer container)
		{
			RegisterActors(container);

			RegisterRoutes(container);
		}

		private static void RegisterActors(IContainer container)
		{
			container.Configure(x =>
				{
					x.Scan(y =>
						{
							y.TheCallingAssembly();
							y.With<ActorConvention>();
						});
				});
		}

		private class ActorConvention :
			TypeRules,
			ITypeScanner
		{
			public void Process(Type type, PluginGraph graph)
			{
				if (!IsConcrete(type)) return;

				if (type.Name.EndsWith("Actor") && type.Implements<AsyncHttpActor>())
					graph.AddType(typeof(AsyncHttpActor), type);
			}
		}
	}
}