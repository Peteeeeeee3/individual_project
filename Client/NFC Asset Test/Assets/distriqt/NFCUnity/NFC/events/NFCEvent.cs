
using System;

namespace distriqt.plugins.nfc
{
	[Serializable]
	public class NFCEvent
	{


		/// <summary>
        /// Dispatched when an NDEF formatted tag is discovered.
        /// The event will contain a Tag with a valid
        /// messages array of NdefMessage objects.
		/// </summary>
		public const string ACTION_NDEF_DISCOVERED = "action.NDEF_DISCOVERED";
		
		
		/// <summary>
        /// Dispatched when a tag is discovered.
        /// <strong>Android only</strong>
		/// </summary>
		public const string ACTION_TECH_DISCOVERED = "action.TECH_DISCOVERED";
		

		/// <summary>
        /// Dispatched when an unknown formatted tag is discovered.
        /// <strong>Android only</strong>
		/// </summary>
		public const string ACTION_TAG_DISCOVERED = "action.TAG_DISCOVERED";


		/// <summary>
		/// This is dispatched when scanning completes.
		/// </summary>
		public const string SCAN_STOPPED = "scan:stopped";



		/// <summary>
		/// The NFC Tag that is associated with this event.
        /// <br/>
		/// If there is no tag this will be null
		/// </summary>
		public Tag tag = null;


        public NFCEvent()
        {
        }


    }
}