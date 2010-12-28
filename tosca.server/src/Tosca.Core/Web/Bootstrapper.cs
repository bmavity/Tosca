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
	using System.Web.Mvc;
	using Data;
	using FluentNHibernate.Cfg;
	using log4net;
	using NHibernate;
	using Settings;
	using StructureMap;
	using StructureMap.Attributes;

	public class Bootstrapper
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof (Bootstrapper));

		public static void Bootstrap()
		{
			BootstrapContainer();

			ControllerBuilder.Current.SetControllerFactory(ObjectFactory.GetInstance<StructureMapControllerFactory>());

			BootstrapServiceBus();

			ActorBootstrapper.Bootstrap(ObjectFactory.GetInstance<IContainer>());
		}

		private static void BootstrapServiceBus()
		{
			ObjectFactory.Configure(x =>
				{
					x.AddRegistry(new WebRegistry());
				});
		}

		private static void BootstrapContainer()
		{
			try
			{
				InstanceScope contextScope = HttpContext.Current != null
				                             	? InstanceScope.HttpContext
				                             	: InstanceScope.ThreadLocal;

				ObjectFactory.Configure(x =>
					{
						x.ForRequestedType<ISessionFactory>()
							.CacheBy(InstanceScope.Singleton)
							.TheDefault.Is.ConstructedBy(context => CreateSessionFactory());

						x.ForRequestedType<ISqlConnectionFactory>()
							.CacheBy(InstanceScope.Singleton)
							.TheDefault.Is.OfConcreteType<SqlConnectionFactory>();

						x.ForRequestedType<ISharedDataContext>()
							.CacheBy(contextScope)
							.TheDefault.Is.OfConcreteType<SqlSharedDataContext>();

						x.ForRequestedType<IClientDataContext>()
							.CacheBy(contextScope)
							.TheDefault.Is.OfConcreteType<SqlClientDataContext>();

						x.ForRequestedType<ISettingsProvider>()
							.TheDefault.Is.OfConcreteType<DatabaseSettingsProvider>();

						x.ForRequestedType<IClientSettings>()
							.TheDefault.Is.OfConcreteType<ClientSettings>();

						x.ForRequestedType<IDataSettings>()
							.TheDefault.Is.OfConcreteType<DataSettings>();

						x.ForRequestedType<ISharedSettings>()
							.TheDefault.Is.OfConcreteType<SharedSettings>();

						x.ForRequestedType<IObjectCache>()
							.CacheBy(InstanceScope.Singleton)
							.TheDefault.Is.OfConcreteType<AspNetCacheProvider>();
					});

				if (_log.IsDebugEnabled)
					_log.Debug(ObjectFactory.WhatDoIHave());
			}
			catch (Exception ex)
			{
				LogManager.GetLogger(typeof (Bootstrapper)).Error("Failed to initialize container", ex);
			}
		}

		private static ISessionFactory CreateSessionFactory()
		{
			return Fluently.Configure()
				.Mappings(m => m.FluentMappings.AddFromAssemblyOf<Bootstrapper>())
				.BuildSessionFactory();
		}
	}
}