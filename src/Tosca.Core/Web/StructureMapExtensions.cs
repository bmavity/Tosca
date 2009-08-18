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
	using StructureMap;

	public static class StructureMapExtensions
	{
		public static void WithEach<T>(this IContainer container, Action<T> action)
		{
			foreach (var configuration in ObjectFactory.Model.PluginTypes)
			{
				if (!configuration.Implements<T>()) continue;

				var instance = (T) container.GetInstance(configuration.PluginType);

				action(instance);
			}
		}

		public static bool Implements<T>(this PluginTypeConfiguration configuration)
		{
			return configuration.PluginType.Implements<T>();
		}

		public static bool Implements<T>(this Type type)
		{
			return typeof (T).IsAssignableFrom(type);
		}
	}
}