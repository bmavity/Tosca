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
namespace Tosca.Core.Web.Actors
{
	using System;
	using System.Web;
	using Magnum;
	using Magnum.Actors;
	using Magnum.Actors.CommandQueues;
	using Magnum.DateTimeExtensions;
	using MassTransit;
	using Messages;

	public class MakeReservationActor :
		AsyncHttpActor
	{
		private readonly CommandQueue _queue;
		private readonly IServiceBus _bus;
		private IAsyncResult _asyncResult;

		private HttpContext _context;
		private Guid _requestId;

		public MakeReservationActor(IServiceBus bus)
		{
			_bus = bus;
			_queue = new ThreadPoolCommandQueue();
		}

		// okay the moral of the story here is that I really hate this
		// it is hard to digest and makes it difficult to understand
		// i really want to make it possible to define a dynamic state machine as part of the actor
		// and then push the messaging components to the edge so that they are less intrusive into the system
		// then i can just send the event, let it get published, and wait for another event to occur

		// also if this could somehow be stored in-memory with a saga repository that would 
		// allow it to be found by mass transit, we could defer our little actor off into the in-memory repository
		// and either timeout or do something else, removing it from the repository so it goes away
		// once the request has been completed. this would have to be keyed by some Guid to avoid performance hits


		public IAsyncResult BeginAction(HttpContext context, AsyncCallback callback, object state)
		{
			_context = context;
			_requestId = CombGuid.Generate();

			var request = new RequestReservation()
				{
					RequestId = _requestId,
					Name = "Johnson",
					NumberOfGuests = 4,
				};

			_asyncResult = _bus.MakeRequest(b => b.Publish(request, c => c.SendResponseTo(_bus)))
				.When<ReservationAccepted>().IsReceived(m =>
					{
						_context.Response.ContentType = "text/plain";
						_context.Response.Write("Reservation accepted for " + m.NumberOfGuests + " guests under the name of " + m.Name);
					})
				.TimeoutAfter(30.Seconds())
				.OnTimeout(() =>
					{
						_context.Response.ContentType = "text/plain";
						_context.Response.Write("A timeout occurred waiting for a response from the reservation request.");
					})
				.BeginSend(callback, state);

			return _asyncResult;
		}
	}
}