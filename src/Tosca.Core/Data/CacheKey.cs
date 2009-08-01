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

    public class CacheKey<T> :
        ICacheKey<T>
    {
        public const string CachePrefix = "t";

        public CacheKey(string key)
            : this(key, TimeSpan.FromMinutes(2))
        {
        }

        public CacheKey(string key, TimeSpan lifespan)
        {
            Lifespan = lifespan;

            Key = key.ToLowerInvariant();
        }

        public string Key { get; set; }

        public virtual string GetCacheKey()
        {
            return string.Format("{0}/{1}/{2}", CachePrefix, typeof (T).Name, Key.ToLowerInvariant());
        }

        public TimeSpan Lifespan { get; set; }

        public static CacheKey<T> Using(params object[] tokens)
        {
            string key = "";
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] != null)
                    key += tokens[i] + "/";
            }

            return new CacheKey<T>(key);
        }

        public CacheKey<T> For(TimeSpan lifespan)
        {
            Lifespan = lifespan;
            return this;
        }
    }
}