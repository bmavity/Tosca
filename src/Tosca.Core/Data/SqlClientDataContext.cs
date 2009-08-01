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
    using System.Data.SqlClient;
    using NHibernate;
    using Settings;

    public class SqlClientDataContext :
        SqlDataContext,
        IClientDataContext
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly IDataSettings _settings;

        public SqlClientDataContext(ISqlConnectionFactory connectionFactory,
                                    IDataSettings settings,
                                    ISessionFactory sessionFactory)
            : base(sessionFactory)
        {
            _connectionFactory = connectionFactory;
            _settings = settings;
        }

        protected override SqlConnection GetConnection()
        {
            return _connectionFactory.GetConnection("Client", _settings.ServerName, _settings.DatabaseName);
        }
    }
}