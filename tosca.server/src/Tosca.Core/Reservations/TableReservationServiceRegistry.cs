// Copyright 2007-2010 The Apache Software Foundation.
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
namespace Tosca.Core.Reservations
{
    using MassTransit;
    using MassTransit.StructureMapIntegration;
    using MassTransit.Transports.RabbitMq;
    using StructureMap.Configuration.DSL;

    public class TableReservationServiceRegistry :
        Registry
    {
        public TableReservationServiceRegistry()
        {
            ForConcreteType<TableReservationService>();

            Bus.Initialize(new StructureMapObjectBuilder(), (cfg, epf) =>
            {
                cfg.ReceiveFrom("rabbitmq://10.0.1.19/tosca.tablereservation");
                cfg.UseJsonSerializer();
            }, typeof (RabbitMqEndpoint));

            For<IServiceBus>().Use(Bus.Instance());
        }
    }
}