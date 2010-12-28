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
namespace Tosca.Core.Settings
{
	using System;
	using System.ComponentModel;
	using System.Linq;
	using Data;
	using Model.Settings;
	using NHibernate.Linq;

	public class DatabaseSettingsProvider :
		ISettingsProvider
	{
		private readonly IObjectCache _objectCache;
		private readonly ISharedDataContext _dataContext;

		public DatabaseSettingsProvider(ISharedDataContext dataContext, IObjectCache objectCache)
		{
			_dataContext = dataContext;
			_objectCache = objectCache;
		}

		public string GetValue(ISettingsContext context, string key)
		{
			return GetValue(context, key, () => null);
		}

		public T GetValue<T>(ISettingsContext context, string key)
		{
			string value = GetValue(context, key);

			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("The [" + key + "] setting was not found for " + context);

			return ConvertValue<T>(key, value);
		}

		public T GetValue<T>(ISettingsContext context, string key, T defaultValue)
		{
			string value = GetValue(context, key, () => new Setting
				{
					ClientId = context.ClientId,
					Key = key,
					Value = defaultValue.ToString()
				});

			return ConvertValue<T>(key, value);
		}

		public T GetValue<T>(ISettingsContext context, string key, Func<string, T> valueConverter)
		{
			string value = GetValue(context, key);

			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("The [" + key + "] setting was not found for " + context);

			return valueConverter(value);
		}

		public T GetValue<T>(ISettingsContext context, string key, Func<string, T> valueConverter, T defaultValue)
		{
			string value = GetValue(context, key, () => new Setting
				{
					ClientId = context.ClientId,
					Key = key,
					Value = defaultValue.ToString()
				});

			return valueConverter(value);
		}

		private string GetValue(ISettingsContext context, string key, Func<Setting> defaultValue)
		{
			Setting setting = GetSetting(context, key, defaultValue);
			if (setting != null)
				return setting.Value;

			return null;
		}

		private Setting GetSetting(ISettingsContext context, string key, Func<Setting> defaultValue)
		{
			var cacheKey = context.GetCacheKey<Setting>(key);

			var item = _objectCache.Get(cacheKey);
			if (item != null)
				return item;

			item = _dataContext.Session.Linq<Setting>()
				.Where(x => x.ClientId == context.ClientId && x.Key == key)
				.FirstOrDefault();

			if (item == null)
				item = defaultValue();

			if (item != null)
				_objectCache.Set(cacheKey, item);

			return item;
		}

		private static T ConvertValue<T>(string key, string value)
		{
			var tc = TypeDescriptor.GetConverter(typeof (T));

			if (tc.CanConvertFrom(typeof (string)))
			{
				return (T) tc.ConvertFrom(value);
			}

			throw new InvalidOperationException("The [" + key + "] setting could not be converted to type: " + typeof (T).FullName);
		}
	}
}