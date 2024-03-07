using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace distriqt.plugins.nfc
{

    public class NFC : MonoBehaviour
    {
        const string version = NFCConst.VERSION;
        const string MISSING_IMPLEMENTATION_ERROR_MESSAGE = "Check you have correctly included the library for this platform";
        const string ID = NFCConst.EXTENSIONID;

#if UNITY_IOS
        const string dll = "__Internal";

        [DllImport(dll)]
        private static extern string NFC_version();
        [DllImport(dll)]
        private static extern string NFC_implementation();
        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool NFC_isSupported();
        
        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool NFC_isEnabled();
        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool NFC_openDeviceSettings();
        [DllImport(dll)]
        private static extern void NFC_checkStartupData();
        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool NFC_registerForegroundDispatch( string optionsJSON );
        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool NFC_unregisterForegroundDispatch();
        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool NFC_enableReaderMode();
        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool NFC_disableReaderMode();
        [DllImport(dll)]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool NFC_restart();


#elif UNITY_ANDROID

        private static AndroidJavaClass pluginClass;
        private static AndroidJavaObject extContext;

#endif


        private static bool _create;
        private static NFC _instance;

        
        static NFC()
        {
#if UNITY_ANDROID
            try
            {
                pluginClass = new AndroidJavaClass("com.distriqt.extension.nfc.NFCUnityPlugin");
                extContext = pluginClass.CallStatic<AndroidJavaObject>("instance");
            }
            catch
            {

            }
#endif
        }


        private NFC()
        {
        }


        /// <summary>
        /// Access to the singleton instance for all this plugins functionality
        /// </summary>
        public static NFC Instance
        {
            get
            {
                if (_instance == null)
                {
                    _create = true;

                    GameObject go = new GameObject();
                    _instance = go.AddComponent<NFC>();
                    _instance.name = ID;
                }
                return _instance;
            }
        }


        private static bool platformSupported()
        {
#if UNITY_IOS || UNITY_ANDROID
            return
                (UnityEngine.Application.platform != RuntimePlatform.OSXEditor)
                && (UnityEngine.Application.platform != RuntimePlatform.WindowsEditor)
                && (UnityEngine.Application.platform != RuntimePlatform.LinuxEditor)
            ;

//#elif UNITY_STANDALONE_OSX
//            return
//                //(Application.platform != RuntimePlatform.OSXEditor) &&
//                (Application.platform != RuntimePlatform.WindowsEditor) &&
//                (Application.platform != RuntimePlatform.LinuxEditor)
//            ;

#else
            return false;
#endif
        }


        /// <summary>
        /// Whether the current device supports the extensions functionality
        /// </summary>
        public static bool isSupported
        {
            get
            {
                try
                {
                    if (platformSupported())
                    {
#if UNITY_IOS
                        return NFC_isSupported();
#elif UNITY_ANDROID
                        return pluginClass.CallStatic<bool>("isSupported");
#endif
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
                }
                return false;
            }
        }


        /// <summary>
        /// The version of this extension.
        /// This should be of the format, MAJOR.MINOR.BUILD
        /// </summary>
        /// <returns>The version of this extension</returns>
        public string Version()
        {
            return version;
        }



        /// <summary>
        /// The native version string of the native extension
        /// </summary>
        /// <returns>The native version string of the native extension</returns>
        public string NativeVersion()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return NFC_version();
#elif UNITY_ANDROID
                    return extContext.Call<string>("version");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return "0";
        }


        /// <summary>
        /// The implementation currently in use.
        /// This should be one of the following depending on the platform in use and the functionality supported by this extension:
        /// <ul>
        /// <li><code>Android</code></li>
        /// <li><code>iOS</code></li>
        /// <li><code>default</code></li>
        /// <li><code>unknown</code></li>
        /// </ul>
        /// </summary>
        /// <returns>The implementation currently in use</returns>
        public string Implementation()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return NFC_implementation();
#elif UNITY_ANDROID
                    return extContext.Call<string>("implementation");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return "default";
        }




        /// <summary>
        /// Return true if this NFC Adapter has any features enabled.
        /// <br/>
        /// If this method returns false, the NFC hardware is guaranteed
        /// not to generate or respond to any NFC communication over its NFC radio.
        /// Applications can use this to check if NFC is enabled.
        /// </summary>
        /// <returns>true if this device has any NFC features enabled and false if not</returns>
        public bool IsEnabled()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return NFC_isEnabled();
#elif UNITY_ANDROID
                    return extContext.Call<bool>("isEnabled");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return false;
        }



        /// <summary>
        /// 
        /// </summary>
        public void CheckStartupData()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    NFC_checkStartupData();
#elif UNITY_ANDROID
                    extContext.Call("checkStartupData");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
        }



        /// <summary>
        /// If NFC is supported on the device, but not enabled calling this
        /// function will display the settings user interface allowing the user
        /// to toggle the settings.
        /// <br/>
        /// Android only as there are currently no user settings for NFC on iOS
        /// </summary>
        public bool OpenDeviceSettings()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return NFC_openDeviceSettings();
#elif UNITY_ANDROID
                    return extContext.Call<bool>("openDeviceSettings");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return false;
        }



        /// <summary>
        /// <strong>Android</strong>
        /// With Android this call will register a few basic NDEF filters and tech filters to
        /// receive NFC events while your application is in the foreground.Calling this will not
        /// necessarily enable your application to be launched from a tag. To do that make sure
        /// you add the appropriate filters to your manifest additions.
        /// <br/>
        /// This function is useful if you just wish to scan for NFC tags while your application is
        /// <br/>
        ///
        /// <strong>iOS</strong>
        /// On iOS this call will start a scan for NFC tags. This is the only method available for iOS
        /// developers to receive NFC notifications.On iOS you may receive a NFCEvent.SCAN_STOPPED
        /// event to indicate this process was stopped.
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns>true if NFC scanning was successfully registered / started and false if scanning was already in progress or if the extension hasn't been setup correctly.</returns>
        public bool RegisterForegroundDispatch(ScanOptions options)
        {
            try
            {
                if (platformSupported())
                {
                    if (options == null)
                    {
                        options = new ScanOptions();
                        options.mimeTypes = new string[] { "*/*" };
                    }
#if UNITY_IOS
                    return NFC_registerForegroundDispatch( options.toJSONString() );
#elif UNITY_ANDROID
                    return extContext.Call<bool>("registerForegroundDispatch", options.toJSONString() );
#endif
                    }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return false;
        }


        /// <summary>
        /// This will cancel the operation started by the registerForegroundDispatch() function.
        /// </summary>
        /// <returns>true if NFC scanning was successfully unregistered / stopped and false if scanning was not in progress or if the extension hasn't been setup correctly.</returns>
        public bool UnregisterForegroundDispatch()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return NFC_unregisterForegroundDispatch();
#elif UNITY_ANDROID
                    return extContext.Call<bool>("unregisterForegroundDispatch");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return false;
        }




        public bool EnableReaderMode()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return NFC_enableReaderMode();
#elif UNITY_ANDROID
                    return extContext.Call<bool>("enableReaderMode");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return false;
        }


        public bool DisableReaderMode()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return NFC_disableReaderMode();
#elif UNITY_ANDROID
                    return extContext.Call<bool>("disableReaderMode");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return false;
        }


        public bool Restart()
        {
            try
            {
                if (platformSupported())
                {
#if UNITY_IOS
                    return NFC_restart();
#elif UNITY_ANDROID
                    return extContext.Call<bool>("restart");
#endif
                }
            }
            catch (EntryPointNotFoundException e)
            {
                throw new Exception(MISSING_IMPLEMENTATION_ERROR_MESSAGE, e);
            }
            return false;
        }




        //
        //  EVENT HANDLER
        //

        [System.Serializable]
        private class EventData
        {
            public string code = "";
            public string data = "";
        }


        public void Dispatch(string message)
        {
            try
            {
                EventData eventData = JsonUtility.FromJson<EventData>(message);
                switch (eventData.code)
                {
                    case "extension:error":
                        {
                            // ERROR
                            Debug.Log(eventData.data);
                            break;
                        }
                    case NFCEvent.ACTION_NDEF_DISCOVERED:
                        {
                            NFCEvent e = JsonUtility.FromJson<NFCEvent>(eventData.data);
                            OnNdefDiscovered?.Invoke(e);
                            break;
                        }
                    case NFCEvent.ACTION_TAG_DISCOVERED:
                        {
                            NFCEvent e = JsonUtility.FromJson<NFCEvent>(eventData.data);
                            OnTagDiscovered?.Invoke(e);
                            break;
                        }
                    case NFCEvent.ACTION_TECH_DISCOVERED:
                        {
                            NFCEvent e = JsonUtility.FromJson<NFCEvent>(eventData.data);
                            OnTechDiscovered?.Invoke(e);
                            break;
                        }
                    case NFCEvent.SCAN_STOPPED:
                        {
                            NFCEvent e = JsonUtility.FromJson<NFCEvent>(eventData.data);
                            OnScanStopped?.Invoke(e);
                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }



        //
        //  EVENTS
        //


        public delegate void NFCEventHandler(NFCEvent e);


        /// <summary>
        /// Dispatched when an NDEF formatted tag is discovered.
        /// The event will contain a Tag with a valid
        /// messages array of NdefMessage objects.
		/// </summary>
        public event NFCEventHandler OnNdefDiscovered;


        /// <summary>
        /// Dispatched when an unknown formatted tag is discovered.
        /// <strong>Android only</strong>
		/// </summary>
        public event NFCEventHandler OnTagDiscovered;


        /// <summary>
        /// Dispatched when a tag is discovered.
        /// <strong>Android only</strong>
		/// </summary>
        public event NFCEventHandler OnTechDiscovered;


        /// <summary>
		/// This is dispatched when scanning completes.
		/// </summary>
        public event NFCEventHandler OnScanStopped;



        //
        //  MonoBehaviour
        //


        public void Awake()
        {
            if (_create)
            {
                _create = false;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // Enforce singleton
                Destroy(gameObject);
            }
        }


        public void OnDestroy()
        {
        }


    }

}
