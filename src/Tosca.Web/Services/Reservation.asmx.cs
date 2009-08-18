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
namespace Tosca.Web.Services
{
	using System.ComponentModel;
	using System.Web.Services;
	using Core.Components.Reservations;
	using Core.Components.Reservations.Services;
	using StructureMap;

	/// <summary>
	/// Summary description for Reservation
	/// </summary>
	[WebService(Namespace = "http://tosca.googlecode.com/services")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
		// [System.Web.Script.Services.ScriptService]
	public class Reservation :
		WebService
	{
		private readonly IReservationService _reservationService;

		public Reservation()
			: this(ObjectFactory.GetInstance<IReservationService>())
		{
		}

		private Reservation(IReservationService reservationService)
		{
			_reservationService = reservationService;
		}


		[WebMethod]
		public string RequestReservation(string name, int guestCount)
		{
			var details = new ReservationDetails
				{
					Name = name,
					GuestCount = guestCount,
				};

			return _reservationService.RequestReservation(details);
		}
	}
}