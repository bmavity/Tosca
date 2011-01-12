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
namespace Tosca.ReservationService
{
    using Core.Reservations;
    using log4net.Config;
    using StructureMap;
    using Topshelf;
    using Topshelf.Configuration.Dsl;

    internal class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            ObjectFactory.Initialize(init=>
            {
                init.AddRegistry(new TableReservationServiceRegistry());
            });

            var runner = RunnerConfigurator.New(cfg =>
            {
                cfg.SetServiceName("tosca.tablereservation");
                cfg.SetDisplayName("Tosca Table Reservation System");
                cfg.SetDescription("Used to maximize table usage");

                cfg.ConfigureService<TableReservationService>(sc =>
                {
                    sc.HowToBuildService(name => ObjectFactory.GetInstance<TableReservationService>());
                    sc.WhenStarted(s=>s.Start());
                    sc.WhenStopped(s=>s.Stop());
                });
            });

            Runner.Host(runner, args);
        }
    }
}