using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.ode
{
	public class ExternalEventVM : EventVM
	{
		/*****************************************************************************************************/
		/* Enums/Constants
		/*****************************************************************************************************/
		#region Constants
		#endregion Constants

		/*****************************************************************************************************/
		/* Data
		/*****************************************************************************************************/
		#region Data
		#endregion Data

		/*****************************************************************************************************/
		/* Constructors
		/*****************************************************************************************************/
		#region Constructors

		public ExternalEventVM(ODELib.ode.ExternalEvent externalEvent)
			: base(externalEvent)
		{
			this.OdeExternalEvent = externalEvent;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.ode.ExternalEvent OdeExternalEvent { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Message")]
		[Description("Message")]
		public string Message { get => OdeExternalEvent.Message; set => OdeExternalEvent.Message = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Message Type")]
		[Description("Message Type")]
		public string MessageType { get => OdeExternalEvent.MessageType; set => OdeExternalEvent.MessageType = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Source ID")]
		[Description("Source ID")]
		public string SourceID { get => OdeExternalEvent.SourceID; set => OdeExternalEvent.SourceID = value; }

		//----------------------------------------------------------------------------------------------------//

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
