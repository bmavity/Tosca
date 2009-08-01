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
    using System.Data.SqlClient;
    using NHibernate;

    public abstract class SqlDataContext :
        IDataContext
    {
        private readonly ISessionFactory _sessionFactory;
        private SqlConnection _connection;
        private volatile bool _disposed;
        private ISession _session;

        protected SqlDataContext(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public SqlConnection Connection
        {
            get
            {
                if (_connection != null)
                    return _connection;

                _connection = GetConnection();

                return _connection;
            }
        }

//        public ICommandContext CreateCommand(string text)
//        {
//            var command = new SqlCommandContext(Connection, text, CommandType.StoredProcedure);
//
//            return command;
//        }
//
//        public ICommandContext CreateCommand(string text, CommandType commandType)
//        {
//            var command = new SqlCommandContext(Connection, text, commandType);
//
//            return command;
//        }

        public ISession Session
        {
            get
            {
                if (_session != null)
                    return _session;

                _session = _sessionFactory.OpenSession(Connection);

                return _session;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~SqlDataContext()
        {
            Dispose(false);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            if (_session != null)
            {
                _session.Close();
                _session.Dispose();
                _session = null;
            }

            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }

            _disposed = true;
        }

        protected abstract SqlConnection GetConnection();
    }
}