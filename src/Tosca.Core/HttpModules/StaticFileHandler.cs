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
namespace Tosca.Core.HttpModules
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Net;
    using System.Web;
    using System.Web.Caching;

    public class StaticFileHandler :
        IHttpAsyncHandler
    {
        private static readonly string[] _compressFileTypes = new[] {".css", ".js", ".html", ".htm"};

        private static readonly TimeSpan _defaultCacheDuration = TimeSpan.FromDays(1);

        private static readonly string[] _fileTypes = new[] {".css", ".js", ".html", ".htm", ".png", ".jpeg", ".jpg", ".gif", ".bmp"};

        private HttpContext _context;

        static StaticFileHandler()
        {
            Array.Sort(_fileTypes);
            Array.Sort(_compressFileTypes);
        }

        public virtual IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object state)
        {
            _context = context;
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            try
            {
                EnsureProperRequest(request);

                string physicalFilePath = request.PhysicalPath;

                ResponseCompressionType compressionType = GetCompressionMode(request);

                FileInfo file = new FileInfo(physicalFilePath);
                string fileExtension = file.Extension.ToLower();

                // Do we handle such file types?
                if (Array.BinarySearch(_fileTypes, fileExtension) >= 0)
                {
                    // Yes we do. 

                    // If this is a binary file like image, then we won't compress it.
                    if (Array.BinarySearch(_compressFileTypes, fileExtension) < 0)
                        compressionType = ResponseCompressionType.None;

                    // If the response bytes are already cached, then deliver the bytes directly from cache
                    string cacheKey = typeof (StaticFileHandler) + ":" + compressionType + ":" + physicalFilePath;

                    if (DeliverFromCache(context, request, response, cacheKey, physicalFilePath, compressionType))
                    {
                        // Delivered from cache
                    }
                    else
                    {
                        if (file.Exists)
                        {
                            // When not compressed, buffer is the size of the file but when compressed, 
                            // initial buffer size is one third of the file size. Assuming, compression 
                            // will give us less than 1/3rd of the size
                            using (MemoryStream memoryStream = new MemoryStream(
                                compressionType == ResponseCompressionType.None ?
                                                                                    Convert.ToInt32(file.Length) :
                                                                                                                     Convert.ToInt32((double) file.Length/3)))
                            {
                                ReadFileData(compressionType, file, memoryStream);

                                CacheAndDeliver(context, request, response, physicalFilePath, compressionType, cacheKey, memoryStream, file);
                            }
                        }
                        else
                        {
                            throw new HttpException((int) HttpStatusCode.NotFound, request.FilePath + " Not Found");
                        }
                    }
                }
                else
                {
                    TransmitFileUsingHttpResponse(request, response, physicalFilePath, compressionType, file);
                }

                return new HttpAsyncResult(callback, state, true, null, null);
            }
            catch (Exception x)
            {
                if (x is HttpException)
                {
                    HttpException h = x as HttpException;
                    response.StatusCode = h.GetHttpCode();
                    Debug.WriteLine(h.Message);
                }
                return new HttpAsyncResult(callback, state, true, null, x);
            }
        }

        public virtual void EndProcessRequest(IAsyncResult result)
        {
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        private bool DeliverFromCache(HttpContext context,
                                      HttpRequest request, HttpResponse response,
                                      string cacheKey,
                                      string physicalFilePath, ResponseCompressionType compressionType)
        {
            CachedContent cachedContent = context.Cache[cacheKey] as CachedContent;
            if (null != cachedContent)
            {
                byte[] cachedBytes = cachedContent.ResponseBytes;

                // We have it cached
                ProduceResponseHeader(response, cachedBytes.Length, compressionType,
                    physicalFilePath, cachedContent.LastModified);
                WriteResponse(response, cachedBytes, compressionType, physicalFilePath);

                Debug.WriteLine("StaticFileHandler: Cached: " + request.FilePath);
                return true;
            }
            return false;
        }

        private void CacheAndDeliver(HttpContext context,
                                     HttpRequest request, HttpResponse response,
                                     string physicalFilePath, ResponseCompressionType compressionType,
                                     string cacheKey, MemoryStream memoryStream, FileInfo file)
        {
            // Cache the content in ASP.NET Cache
            byte[] responseBytes = memoryStream.ToArray();
            context.Cache.Insert(cacheKey, new CachedContent(responseBytes, file.LastWriteTimeUtc),
                new CacheDependency(physicalFilePath),
                DateTime.Now.Add(_defaultCacheDuration), Cache.NoSlidingExpiration);

            ProduceResponseHeader(response, responseBytes.Length, compressionType,
                physicalFilePath, file.LastWriteTimeUtc);
            WriteResponse(response, responseBytes, compressionType, physicalFilePath);

            Debug.WriteLine("StaticFileHandler: NonCached: " + request.FilePath);
        }

        private static void ReadFileData(ResponseCompressionType compressionType,
                                         FileInfo file, MemoryStream memoryStream)
        {
            using (Stream outputStream =
                (compressionType == ResponseCompressionType.None ? memoryStream :
                                                                                    (compressionType == ResponseCompressionType.GZip ?
                                                                                                                                         new GZipStream(memoryStream, CompressionMode.Compress, true) :
                                                                                                                                                                                                          (Stream) new DeflateStream(memoryStream, CompressionMode.Compress))))
            {
                // We can compress and cache this file
                using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int bufSize = Convert.ToInt32(Math.Min(file.Length, 8*1024));
                    byte[] buffer = new byte[bufSize];

                    int bytesRead;
                    while ((bytesRead = fs.Read(buffer, 0, bufSize)) > 0)
                    {
                        outputStream.Write(buffer, 0, bytesRead);
                    }
                }

                outputStream.Flush();
            }
        }

        private static void EnsureProperRequest(HttpRequest request)
        {
            if (request.HttpMethod == "POST")
            {
                throw new HttpException((int) HttpStatusCode.MethodNotAllowed, "Method not allowed");
            }
            if (request.FilePath.EndsWith(".asp", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new HttpException((int) HttpStatusCode.Forbidden, "Path forbidden");
            }
        }

        private void TransmitFileUsingHttpResponse(HttpRequest request, HttpResponse response,
                                                   string physicalFilePath, ResponseCompressionType compressionType, FileInfo file)
        {
            if (file.Exists)
            {
                // We don't cache/compress such file types. Must be some binary file that's better
                // to let IIS handle
                ProduceResponseHeader(response, Convert.ToInt32(file.Length), compressionType,
                    physicalFilePath, file.LastWriteTimeUtc);
                response.TransmitFile(physicalFilePath);

                Debug.WriteLine("TransmitFile: " + request.FilePath);
            }
            else
            {
                throw new HttpException((int) HttpStatusCode.NotFound, request.FilePath + " Not Found");
            }
        }

        private void WriteResponse(HttpResponse response, byte[] bytes,
                                   ResponseCompressionType mode, string physicalFilePath)
        {
            response.OutputStream.Write(bytes, 0, bytes.Length);
            response.OutputStream.Flush();
        }

        private void ProduceResponseHeader(HttpResponse response, int count,
                                           ResponseCompressionType mode, string physicalFilePath,
                                           DateTime lastModified)
        {
            response.Buffer = false;
            response.BufferOutput = false;

            // Emit content type and encoding based on the file extension and 
            // whether the response is compressed
            response.ContentType = MimeMapping.GetMimeMapping(physicalFilePath);
            if (mode != ResponseCompressionType.None)
                response.AppendHeader("Content-Encoding", mode.ToString().ToLower());
            response.AppendHeader("Content-Length", count.ToString());

            // Emit proper cache headers that will cache the response in browser's 
            // cache for the default cache duration
            response.Cache.SetCacheability(HttpCacheability.Public);
            response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            response.Cache.SetMaxAge(_defaultCacheDuration);
            response.Cache.SetExpires(DateTime.Now.Add(_defaultCacheDuration));
            response.Cache.SetLastModified(lastModified);
        }

        private ResponseCompressionType GetCompressionMode(HttpRequest request)
        {
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncoding)) return ResponseCompressionType.None;

            acceptEncoding = acceptEncoding.ToUpperInvariant();

            if (acceptEncoding.Contains("GZIP"))
                return ResponseCompressionType.GZip;

            if (acceptEncoding.Contains("DEFLATE"))
                return ResponseCompressionType.Deflate;

            return ResponseCompressionType.None;
        }

        private class CachedContent
        {
            public DateTime LastModified;
            public byte[] ResponseBytes;

            public CachedContent(byte[] bytes, DateTime lastModified)
            {
                ResponseBytes = bytes;
                LastModified = lastModified;
            }
        }

        private enum ResponseCompressionType
        {
            None,
            GZip,
            Deflate
        }
    }
}