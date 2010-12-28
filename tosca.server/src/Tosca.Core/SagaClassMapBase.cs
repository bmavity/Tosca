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
namespace Tosca.Core.Components
{
    using FluentNHibernate.Mapping;
    using Magnum.Infrastructure.StateMachine;
    using MassTransit.Saga;

    public class SagaClassMapBase<T> :
        ClassMap<T>
        where T : SagaStateMachine<T>, ISaga
    {
        protected SagaClassMapBase()
        {
            Id(x => x.CorrelationId);
            Map(x => x.CurrentState)
                .Access.AsReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
                .CustomTypeIs<StateMachineUserType>();
        }
    }
}