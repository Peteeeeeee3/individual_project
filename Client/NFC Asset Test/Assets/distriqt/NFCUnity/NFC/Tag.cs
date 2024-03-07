using System;
using System.Collections.Generic;

namespace distriqt.plugins.nfc
{

    [Serializable]
    public class Tag
    {

        public string id;
        public List<string> techlist;
        public List<NdefMessage> messages;

        public Tag()
        {
        }

    }
}
