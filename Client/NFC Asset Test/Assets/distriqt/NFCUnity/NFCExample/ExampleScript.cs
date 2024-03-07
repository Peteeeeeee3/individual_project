using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

using distriqt.plugins.nfc;

namespace distriqt.example.nfc
{
    public class ExampleScript : MonoBehaviour
    {

        public GameObject stateTextObject;
        public GameObject logTextObject;

        public Button button1;
        public Button button2;
        public Button button3;
        public Button button4;
        public Button button5;
        public Button button6;

        private Text stateText;
        private Text logText;


        void Start()
        {
            if (stateTextObject != null)
            {
                stateText = stateTextObject.GetComponent<Text>();
            }
            if (logTextObject != null)
            {
                logText = logTextObject.GetComponent<Text>();
                logText.text = "";
            }


            Button btn;
            btn = button1.GetComponent<Button>();
            btn?.onClick.AddListener(button1_OnClick);
            btn = button2.GetComponent<Button>();
            btn?.onClick.AddListener(button2_OnClick);
            btn = button3.GetComponent<Button>();
            btn?.onClick.AddListener(button3_OnClick);
            btn = button4.GetComponent<Button>();
            btn?.onClick.AddListener(button4_OnClick);
            btn = button5.GetComponent<Button>();
            btn?.onClick.AddListener(button5_OnClick);
            btn = button6.GetComponent<Button>();
            btn?.onClick.AddListener(button6_OnClick);




            UpdateState(
                "NFC.isSupported: " + NFC.isSupported + "\n" +
                "NFC.version: " + NFC.Instance.Version()
            );

            //
            //  Check whether the plugin is supported on the current platform

            if (NFC.isSupported)
            {

                NFC.Instance.OnNdefDiscovered += Instance_OnNdefDiscovered;
                NFC.Instance.OnScanStopped += Instance_OnScanStopped;


                NFC.Instance.CheckStartupData();
            }

        }

        private void Instance_OnScanStopped(NFCEvent e)
        {
            Log("Scan stopped");
        }

        private void Instance_OnNdefDiscovered(NFCEvent e)
        {
            Log("Instance_OnNdefDiscovered");
            if (e.tag != null)
            {
                Log("Tag.id: " + e.tag.id);
                foreach (NdefMessage message in e.tag.messages)
                {
                    Log("NdefMessage:");
                    foreach (NdefRecord record in message.records)
                    {
                        Log("  record.payload: " + record.payload);
                    }
                }
            }
            else
            {
                Log("Tag is null");
            }

            
        }

        private void button1_OnClick()
        {
            Log("IsEnabled() = " + NFC.Instance.IsEnabled()); 
        }


        private void button2_OnClick()
        {
            NFC.Instance.OpenDeviceSettings();
        }


        private void button3_OnClick()
        {
            if (NFC.isSupported)
            {
                ScanOptions options = new ScanOptions();

                options.message = "Scan for tag";
                options.mimeTypes = new string[] { "*/*" };
                options.urls = new string[] { "https://airnativeextensions.com" };

                NFC.Instance.RegisterForegroundDispatch(options);
            }
            else
            {
                Log("NFC not supported");
            }
        }


        private void button4_OnClick()
        {
            if (NFC.isSupported)
            {
                NFC.Instance.UnregisterForegroundDispatch();
            }
            else
            {
                Log("NFC not supported");
            }
        }



        private bool _firstRead = true;

        private void button5_OnClick()
        {
            if (NFC.isSupported)
            {
                _firstRead = true;
                NFC.Instance.OnNdefDiscovered += Instance_OnNdefDiscovered_ReaderMode;
                NFC.Instance.EnableReaderMode();
            }
            else
            {
                Log("NFC not supported");
            }
        }


        private void button6_OnClick()
        {
            if (NFC.isSupported)
            {
                NFC.Instance.OnNdefDiscovered -= Instance_OnNdefDiscovered_ReaderMode;
                NFC.Instance.DisableReaderMode();
            }
            else
            {
                Log("NFC not supported");
            }
        }


        private void Instance_OnNdefDiscovered_ReaderMode(NFCEvent e)
        {
            if (_firstRead)
            {
                _firstRead = false;
                StartCoroutine(Restart());
            }
            else
            {
                button6_OnClick();
            }
        }




        IEnumerator Restart()
        {
            yield return new WaitForSeconds(1);
            NFC.Instance.Restart();
        }











        void UpdateState( string state )
        {
            if (stateText != null)
            {
                stateText.text = state;
            }
        }

        void Log(string message)
        {
            if (logText != null)
            {
                logText.text = message + "\n" + logText.text;
            }
            Debug.Log(message);
        }





    }

}
