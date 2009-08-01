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

    /// <summary>
    /// A general interface for passing keys to cache services (like IObjectCache)
    /// </summary>
    public interface ICacheKey
    {
        /// <summary>
        /// Creates a unique key related to this object for storing in the cache
        /// Cache keys should be globally unique and pathed to the object
        /// </summary>
        /// <returns>A string used as the key to the cache</returns>
        string GetCacheKey();

        /// <summary>
        /// Specifies how long the object should remain in the first-level cache before being discarded
        /// </summary>
        TimeSpan Lifespan { get; }
    }

    public interface ICacheKey<T> :
        ICacheKey
    {
    }
}