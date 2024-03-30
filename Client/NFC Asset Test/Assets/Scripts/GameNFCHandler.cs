using distriqt.plugins.nfc;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class GameNFCHandler : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI NFCsupportedText;
    [SerializeField]
    private TextMeshProUGUI DebugText1;
    [SerializeField]
    private NFCMessanger NfcMessanger;

    private string ConnectionIdString = "G_NFCHANDLER";

    // Start is called before the first frame update
    void Start()
    {
        Connection.Subscribe(ConnectionIdString, this);
        bool isNFCSupported = NFC.isSupported;
        NFCsupportedText.SetText(isNFCSupported.ToString());
        if (!isNFCSupported )
        {
            // Have this prompt message that device does not support NFC and then exit game upon OK button
            return;
        }

        // set relevant MIME types
        // this ignores all scanned MIME types not set -> records on NFC tags for this project store RTD_TEXT (texxt/plain)
        ScanOptions scanOptions = new ScanOptions();
        scanOptions.mimeTypes = new string[] { "text/plain" };

        // register application for NFC scanning when running in foreground
        NFC.Instance.RegisterForegroundDispatch(scanOptions);

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
        // traverse all messages
        foreach (NdefMessage message in e.tag.messages)
        {
            string tempText = "";
            // traverse records in message
            foreach (NdefRecord record in message.records) 
            {
                tempText += record.payload;
            }
            tempText = StringToASCII(tempText);
            OnRequestFigureData(tempText);
            DebugText1.SetText(tempText);
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
                byte val = Convert.ToByte(hexString.Substring(i, 2), 16);
                char character = Convert.ToChar(val);
                ascii += character;
                ascii = Regex.Replace(ascii, "\x02en", "");
                ascii = ascii.Replace("\x02", "");
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
            Int32.Parse(data[dataOffset + 4]),
            float.Parse(data[dataOffset + 5]),
            float.Parse(data[dataOffset + 6]),
            float.Parse(data[dataOffset + 7]),
            float.Parse(data[dataOffset + 8]),
            ""
            );

        NfcMessanger.Figure = update;
        NfcMessanger.IsRead = false;
        if (!NfcMessanger.Initialized) { NfcMessanger.Initialized = true; }
        DebugText1.SetText(DebugText1.text + " Figure set / IsRead == " + NfcMessanger.IsRead + " / Initialized == " + NfcMessanger.Initialized);
    }

    /// <summary>
    /// Handles the destruction of this MonoBehaviour class
    /// </summary>
    private void OnDestroy()
    {
        Connection.Unsubscribe("NFCHANDLER");
    }

    ~GameNFCHandler()
    {
        NFC.Instance.OnNdefDiscovered -= Instance_OnNDEFDiscovered;
    }
}
