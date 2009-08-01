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
    using System.Linq.Expressions;
    using Data;
    using Model.Configuration;
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
            ConfigurationSetting configurationItem = GetConfigurationSetting(context, key);
            if (configurationItem != null)
                return configurationItem.Value ?? "";

            return null;
        }

        public T GetValue<T>(ISettingsContext context, string key)
        {
            string value = GetValue(context, key);

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("No value found for configuration key " + key, "key");

            return ConvertValue<T>(key, value);
        }

        public T GetValue<T>(ISettingsContext context, string key, T defaultValue)
        {
            string value = GetValue(context, key);

            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return ConvertValue<T>(key, value);
        }

        public T GetValue<T>(ISettingsContext context, string key, Func<string, T> valueGenerator)
        {
            string value = GetValue(context, key);

            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("No value found for configuration key " + key, "key");

            return valueGenerator(value);
        }

        private ConfigurationSetting GetConfigurationSetting(ISettingsContext context, string key)
        {
            var cacheKey = context.GetCacheKey<ConfigurationSetting>(key);

            return GetConfigurationSetting(x => x.ClientId == context.ClientId && x.Key == key, cacheKey);
        }

        private T GetConfigurationSetting<T>(Expression<Func<T, bool>> expression, ICacheKey<T> cacheKey)
            where T : class
        {
            var item = _objectCache.Get(cacheKey);
            if (item != null)
                return item;

            item = _dataContext.Session.Linq<T>()
                .Where(expression)
                .FirstOrDefault();

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

            throw new InvalidOperationException("Could not convert configuration value " + key + " to " + typeof (T).FullName);
        }
    }
}