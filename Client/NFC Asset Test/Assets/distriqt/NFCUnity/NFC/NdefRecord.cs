using System;


namespace distriqt.plugins.nfc
{

    [Serializable]
    public class NdefRecord
    {

        public string id;
        public string type;
        public string tnf;
        public string payload;
        public string url;
        public string mimeType;


        public NdefRecord()
        {
        }
    }

}