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
namespace Tosca.Core.Web.HttpModules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Web;
    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using log4net.Repository.Hierarchy;

    public class PerformanceModule :
        IHttpModule
    {
        private const string LogRequestDurationKey = "request_duration";
        private const string LogRequestStartedAtKey = "request_time";
        private const string LogRequestUrlKey = "request_url";

        private static readonly ILog _log = LogManager.GetLogger("Tosca.PageMetrics");

        private static readonly object _queryCountKey = new object();
        private static readonly object _queryTextKey = new object();
        private static readonly object _requestStartKey = new object();
        private static readonly object _requestStopwatchKey = new object();

        public static int QueryCount
        {
            get
            {
                object value = HttpContext.Current.Items[_queryCountKey];
                if (value == null)
                    return 0;

                return (int) value;
            }
        }

        public static List<string> Queries
        {
            get
            {
                List<string> queries = ((List<string>) HttpContext.Current.Items[_queryTextKey]);
                if (queries == null)
                {
                    HttpContext.Current.Items[_queryTextKey] = queries = new List<string>();
                }
                return queries;
            }
        }


        public void Init(HttpApplication context)
        {
            InitializeLogger();

            context.BeginRequest += OnBeginRequest;
            context.EndRequest += OnEndRequest;
        }

        public void Dispose()
        {
        }

        private void OnBeginRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            context.Items[_requestStartKey] = DateTime.UtcNow;
            context.Items[_requestStopwatchKey] = Stopwatch.StartNew();
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            Stopwatch stopwatch = ((Stopwatch) context.Items[_requestStopwatchKey]);
            stopwatch.Stop();

            DateTime startedAt = ((DateTime) context.Items[_requestStartKey]);

            string url = context.Request.RawUrl;

            ThreadContext.Properties[LogRequestDurationKey] = stopwatch.ElapsedMilliseconds.ToString();
            ThreadContext.Properties[LogRequestStartedAtKey] = startedAt;
            ThreadContext.Properties[LogRequestUrlKey] = url;

            _log.InfoFormat("{0} {1} {2} {3}", startedAt.ToString("yyyy-MM-dd HH:mm:ss.fff"), stopwatch.ElapsedMilliseconds, QueryCount, url);

            ResetLogThreadContext();
        }

        private static void ResetLogThreadContext()
        {
            ThreadContext.Properties[LogRequestDurationKey] = null;
            ThreadContext.Properties[LogRequestStartedAtKey] = null;
            ThreadContext.Properties[LogRequestUrlKey] = null;
        }

        private void InitializeLogger()
        {
            var appender = new QueryAppender {Name = "PerformanceAppender"};

            AddAppenderToLogger((Logger) LogManager.GetLogger("NHibernate.SQL").Logger, appender);

            AddAppenderToLogger((Logger) LogManager.GetLogger("Tosca.SQL").Logger, appender);
        }

        private void AddAppenderToLogger(Logger logger, QueryAppender appender)
        {
            lock (logger)
            {
                logger.Additivity = true;
                if (HasQueryAppender(logger) == false)
                {
                    logger.AddAppender(appender);
                    logger.Level = logger.Hierarchy.LevelMap["DEBUG"];
                }
            }
        }

        public static void IncrementQueryCount()
        {
            HttpContext.Current.Items[_queryCountKey] = QueryCount + 1;
        }

        public static void AppendQuery(string query)
        {
            Queries.Add(query);
        }

        private static bool HasQueryAppender(IAppenderAttachable logger)
        {
            foreach (IAppender appender in logger.Appenders)
            {
                if (appender is QueryAppender)
                    return true;
            }
            return false;
        }

        public class QueryAppender :
            IAppender
        {
            public void Close()
            {
            }

            public void DoAppend(LoggingEvent loggingEvent)
            {
                if (string.Empty.Equals(loggingEvent.MessageObject))
                    return;

                IncrementQueryCount();

                if (loggingEvent.MessageObject != null)
                    AppendQuery(loggingEvent.MessageObject.ToString());
            }

            public string Name { get; set; }
        }
    }
}