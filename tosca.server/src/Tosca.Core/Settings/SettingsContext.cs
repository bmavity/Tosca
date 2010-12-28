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
    using Data;

    public class SettingsContext :
        ISettingsContext
    {
        public SettingsContext(Guid clientId, TimeSpan cacheLifespan)
        {
            ClientId = clientId;
            CacheLifespan = cacheLifespan;
        }

        public Guid ClientId { get; private set; }
        public TimeSpan CacheLifespan { get; private set; }

        public ICacheKey<T> GetCacheKey<T>(string key)
        {
            return CacheKey<T>.Using(ClientId, key).For(CacheLifespan);
        }

        public override string ToString()
        {
            return string.Format("Client Id {0}", ClientId);
        }
    }
}