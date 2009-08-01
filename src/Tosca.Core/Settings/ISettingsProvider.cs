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

    /// <summary>
    /// Provides configuration values for the application
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// Returns the value associated with the key specified
        /// </summary>
        /// <param name="context">The configuration context to use when retrieving the value</param>
        /// <param name="key">The key to retrieve</param>
        /// <returns>The value of the configuration item</returns>
        string GetValue(ISettingsContext context, string key);

        T GetValue<T>(ISettingsContext context, string key);
        T GetValue<T>(ISettingsContext context, string key, T defaultValue);
        T GetValue<T>(ISettingsContext context, string key, Func<string, T> valueGenerator);
    }
}