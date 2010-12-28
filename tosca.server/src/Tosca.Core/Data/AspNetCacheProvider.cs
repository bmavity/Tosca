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
namespace Tosca.Core.Data
{
    using System;
    using System.Web;
    using System.Web.Caching;

    public class AspNetCacheProvider :
        IObjectCache
    {
        private readonly Cache _cache;

        public AspNetCacheProvider()
        {
            _cache = HttpRuntime.Cache;
        }

        public void Dispose()
        {
        }

        public T Get<T>(ICacheKey<T> key)
        {
            return (T) _cache.Get(key.GetCacheKey());
        }

        public void Set<T>(ICacheKey<T> key, T value)
        {
            _cache.Insert(key.GetCacheKey(), value, null, DateTime.UtcNow + key.Lifespan, Cache.NoSlidingExpiration);
        }

        public void Evict(ICacheKey key)
        {
            _cache.Remove(key.GetCacheKey());
        }
    }
}