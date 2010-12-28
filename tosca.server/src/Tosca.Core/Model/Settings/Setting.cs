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
namespace Tosca.Core.Model.Settings
{
	using System;
	using FluentNHibernate.Mapping;

	public class Setting
	{
		public virtual Guid ClientId { get; set; }
		public virtual string Key { get; set; }
		public virtual string Value { get; set; }

		public virtual bool Equals(Setting other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.ClientId.Equals(ClientId) && Equals(other.Key, Key);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (Setting)) return false;
			return Equals((Setting) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (ClientId.GetHashCode()*397) ^ (Key != null ? Key.GetHashCode() : 0);
			}
		}
	}

	public class SettingMap :
		ClassMap<Setting>
	{
		public SettingMap()
		{
			WithTable("`Setting`");

			UseCompositeId()
				.WithKeyProperty(x => x.ClientId)
				.WithKeyProperty(x => x.Key, "`Key`");

			Map(x => x.ClientId);
			Map(x => x.Key, "`Key`").WithLengthOf(128);
			Map(x => x.Value, "`Value`").WithLengthOf(512);
		}
	}
}