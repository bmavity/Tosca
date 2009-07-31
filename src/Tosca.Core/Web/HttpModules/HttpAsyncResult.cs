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
    using System.Threading;
    using System.Web;

    internal class HttpAsyncResult :
        IAsyncResult
    {
        // Fields
        private AsyncCallback _callback;
        private object _result;

        // Methods
        internal HttpAsyncResult(AsyncCallback cb, object state)
        {
            _callback = cb;
            AsyncState = state;
            Status = RequestNotificationStatus.Continue;
        }

        internal HttpAsyncResult(AsyncCallback cb, object state, bool completed, object result, Exception error)
        {
            _callback = cb;
            AsyncState = state;
            IsCompleted = completed;
            CompletedSynchronously = completed;
            _result = result;
            Error = error;
            Status = RequestNotificationStatus.Continue;
            if (IsCompleted && (_callback != null))
            {
                _callback(this);
            }
        }

        internal Exception Error { get; private set; }

        internal RequestNotificationStatus Status { get; private set; }

        public object AsyncState { get; private set; }

        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        public bool CompletedSynchronously { get; private set; }

        public bool IsCompleted { get; private set; }

        internal void Complete(bool synchronous, object result, Exception error)
        {
            Complete(synchronous, result, error, RequestNotificationStatus.Continue);
        }

        internal void Complete(bool synchronous, object result, Exception error, RequestNotificationStatus status)
        {
            IsCompleted = true;
            CompletedSynchronously = synchronous;
            _result = result;
            Error = error;
            Status = status;
            if (_callback != null)
            {
                _callback(this);
            }
        }

        internal object End()
        {
            if (Error != null)
            {
                throw new HttpException(null, Error);
            }
            return _result;
        }

        internal void SetComplete()
        {
            IsCompleted = true;
        }
    }
}