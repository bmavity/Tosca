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
    using System.IO.Compression;
    using System.Web;
    using Core.HttpModules;

    public class HttpCompressionModule : IHttpModule
    {
        void IHttpModule.Dispose()
        {
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.PostAcquireRequestState += context_PostAcquireRequestState;
            context.EndRequest += context_EndRequest;
        }

        private static void context_EndRequest(object sender, EventArgs e)
        {
            HttpApplication context = sender as HttpApplication;
            context.PostAcquireRequestState -= context_PostAcquireRequestState;
            context.EndRequest -= context_EndRequest;
        }

        private static void context_PostAcquireRequestState(object sender, EventArgs e)
        {
            RegisterCompressFilter();
        }

        private static void RegisterCompressFilter()
        {
            HttpContext context = HttpContext.Current;

            if (context.Handler is StaticFileHandler
                || context.Handler is DefaultHttpHandler) return;

            HttpRequest request = context.Request;

            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncoding)) return;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponse response = HttpContext.Current.Response;

            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
            else if (acceptEncoding.Contains("DEFLATE"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}