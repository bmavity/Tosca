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
namespace Tosca.Core.Model
{
    using System;
    using FluentNHibernate.Mapping;
    using Magnum.StateMachine;
    using MassTransit;
    using MassTransit.Saga;
    using MassTransit.Services.Timeout.Messages;
    using Messages;

    public class Reservation :
        SagaStateMachine<Reservation>,
        ISaga
    {
        static Reservation()
        {
            Define(() =>
                {
                    Initially(
                        When(ReservationIsRequested)
                            .Then((s, m) =>
                                {
                                    s.Name = m.Name;
                                    s.NumberOfGuests = m.NumberOfGuests;

                                    // publish event 
                                    // schedule timeout
                                })
                            .TransitionTo(WaitingForAcceptance)
                        );

                    During(WaitingForAcceptance,
                        When(WaitTimeExpired).And(x => x.Tag == RequestTimeout)
                            .Then(s =>
                                {
                                    // add audit event for failure, report reservation not accepted
                                })
                            .TransitionTo(Failed));
                });
        }

        public virtual Guid CorrelationId { get; set; }
        public IServiceBus Bus { get; set; }

        public virtual string Name { get; set; }
        public virtual int NumberOfGuests { get; set; }

        public static State Initial { get; set; }
        public static State Completed { get; set; }
        public static State Failed { get; set; }

        public static State WaitingForAcceptance { get; set; }

        public static Event<RequestReservation> ReservationIsRequested { get; set; }
        public static Event<TimeoutExpired> WaitTimeExpired { get; set; }

        private const int RequestTimeout = 0;
    }

    public class ReservationClassMap :
        ClassMap<Reservation>
    {
        private ReservationClassMap()
        {
            Id(x => x.CorrelationId);

            Map(x => x.Name);
            Map(x => x.NumberOfGuests);
        }
    }
}