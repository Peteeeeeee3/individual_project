using distriqt.plugins.nfc;
using System;
using TMPro;
using UnityEngine;

public class NFCHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI NFCsupportedText;
    [SerializeField]
    private TextMeshProUGUI DebugText1;
    [SerializeField]
    private TextMeshProUGUI DebugText2;
    [SerializeField]
    private TextMeshProUGUI DebugText3;
    [SerializeField]
    private NFCMessanger NfcMessanger;

    private string ConnectionIdString = "NFCHANDLER";

    // Start is called before the first frame update
    void Start()
    {
        Connection.Subscribe(ConnectionIdString, this);
        bool isNFCSupported = NFC.isSupported;
        NFCsupportedText.SetText(isNFCSupported.ToString());
        if (!isNFCSupported )
        {
            // Have this prompt message that device does not support NFC and then exit game upon OK button
            DebugText1.SetText("NFC not supported");
            return;
        }

        // set relevant MIME types
        // this ignores all scanned MIME types not set -> records on NFC tags for this project store RTD_TEXT (texxt/plain)
        ScanOptions scanOptions = new ScanOptions();
        scanOptions.mimeTypes = new string[] { "text/plain" };

        // register application for NFC scanning when running in foreground
        NFC.Instance.RegisterForegroundDispatch(scanOptions);
        DebugText1.SetText("Foreground NFC scanning activated");

        // register Instance_OnNDEFDiscovered() to be registered to be called when NFC tag with NDEF format is detected
        NFC.Instance.OnNdefDiscovered += Instance_OnNDEFDiscovered;

        // register with NFC Messanger
        NfcMessanger.NfcHandler = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Called when an NFC chip is reads
    /// </summary>
    /// <param name="e">NFC Event</param>
    void Instance_OnNDEFDiscovered(NFCEvent e)
    {
        DebugText2.SetText("Instance_OnNDEFDiscovered called");
        // traverse all messages
        foreach (NdefMessage message in e.tag.messages)
        {
            string debugText3 = "";
            // traverse records in message
            foreach (NdefRecord record in message.records) 
            {
                debugText3 += record.payload;
            }
            debugText3 = StringToASCII(debugText3);
            DebugText3.SetText(debugText3);
            OnRequestFigureData(debugText3);
        }
    }


    /// <summary>
    /// Used to convert string of hexadecimal values read from message to an ASCII string
    /// Provided in plugin documentation: https://docs.airnativeextensions.com/docs/nfc/scanning
    /// </summary>
    /// <param name="hexString">string of hexadecimal characters</param>
    /// <returns>ASCII string</returns>
    private static string StringToASCII(string hexString)
    {
        try
        {
            string ascii = string.Empty;
            for (int i = 0; i < hexString.Length; i += 2)
            {
                byte val = System.Convert.ToByte(hexString.Substring(i, 2), 16);
                char character = System.Convert.ToChar(val);
                ascii += character;
            }
            return ascii;
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
        return string.Empty;
    }

    /// <summary>
    /// Requests figure data from server#
    /// Called when NFC scan is triggered
    /// </summary>
    /// <param name="id"></param>
    private void OnRequestFigureData(string id)
    {
        string message = "GET FIGURE\n" +
            ConnectionIdString + "\n" +
            Globals.ACTIVE_USER_ID + "\n" +
            id;

        Connection.QueueMessage(message);
    }

    /// <summary>
    /// Updates NFC messanger
    /// </summary>
    public void OnUpdateMessanger(string[] data, int dataOffset)
    {
        PlayerType type = PlayerType.ERROR;
        if (data[dataOffset + 1].Equals("BLUE"))
        {
            type = PlayerType.BLUE;
        }
        else if (data[dataOffset + 1].Equals("GREY"))
        {
            type = PlayerType.GREY;
        }
        else if (data[dataOffset + 1].Equals("CREAM"))
        {
            type = PlayerType.CREAM;
        }

        Figure update = new Figure(
            data[dataOffset],
            type,
            Int32.Parse(data[dataOffset + 2]),
            Int32.Parse(data[dataOffset + 3]),
            float.Parse(data[dataOffset + 4]),
            float.Parse(data[dataOffset + 5]),
            float.Parse(data[dataOffset + 6]),
            float.Parse(data[dataOffset + 7]),
            ""
            );

        NfcMessanger.Figure = update;
        NfcMessanger.IsRead = false;
        if (!NfcMessanger.Initialized) { NfcMessanger.Initialized = true; }
    }

    /// <summary>
    /// Handles the destruction of this MonoBehaviour class
    /// </summary>
    private void OnDestroy()
    {
        Connection.Unsubscribe("NFCHANDLER");
    }
}
