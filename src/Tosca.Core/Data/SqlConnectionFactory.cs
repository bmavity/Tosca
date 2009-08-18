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
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Reflection;
    using log4net;

    public class SqlConnectionFactory :
        ISqlConnectionFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (SqlConnectionFactory));

        public SqlConnection GetConnection(string connectionName)
        {
            return CreateConnection(GetConnectionString(connectionName));
        }

        public SqlConnection GetConnection(string connectionName, string serverName, string databaseName)
        {
            return CreateConnection(GetConnectionString(connectionName, serverName, databaseName));
        }

        private static string GetConnectionString(string connectionName)
        {
			// maybe break this down into a chain of responsibility pattern
            ConnectionStringSettings connectionSettings = ConfigurationManager.ConnectionStrings[connectionName];
        	if (connectionSettings == null)
        		connectionSettings = GetConnectionSettingsFromAssemblyConfiguration(connectionName);

        	if (connectionSettings == null)
                throw new ConfigurationErrorsException("There are no configuration string configured");

        	return connectionSettings.ConnectionString;
        }

    	private static ConnectionStringSettings GetConnectionSettingsFromAssemblyConfiguration(string connectionName)
    	{
    		var map = new ExeConfigurationFileMap
    			{
    				ExeConfigFilename = Assembly.GetExecutingAssembly().Location + ".config"
    			};

    		_log.InfoFormat("Using Configuration File: {0}", map.ExeConfigFilename);

    		Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

    		return config.ConnectionStrings.ConnectionStrings[connectionName];
    	}

    	private static string GetConnectionString(string connectionName, string serverName, string databaseName)
        {
    		return GetConnectionString(connectionName)
            	.Replace("Server=(local)", "Server=" + serverName)
            	.Replace("Database=Tosca", "Database=" + databaseName);
        }

        private static SqlConnection CreateConnection(string connectionString)
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }
    }
}