using System;
using System.Collections.Generic;

namespace distriqt.plugins.nfc
{
    [Serializable]
    public class NdefMessage
    {

        public List<NdefRecord> records;

        public NdefMessage()
        {
        }
    }

}