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
namespace Tosca.Core.Web
{
	using System;
	using System.Threading;

	public class ActorAsyncResult :
		IAsyncResult
	{
		private readonly AsyncCallback _callback;
		private readonly object _state;
		private volatile bool _completed;

		public ActorAsyncResult()
		{
			_state = null;
		}

		public ActorAsyncResult(AsyncCallback callback, object state)
		{
			_callback = callback;
			_state = state;
		}

		public Exception Exception { get; private set; }

		public bool IsCompleted
		{
			get { return _completed; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return null; }
		}

		public object AsyncState
		{
			get { return _state; }
		}

		public bool CompletedSynchronously
		{
			get { return false; }
		}

		public void Complete()
		{
			_completed = true;

			if (_callback != null)
				_callback(this);
		}

		public void Complete(Exception exception)
		{
			Exception = exception;

			Complete();
		}
	}
}