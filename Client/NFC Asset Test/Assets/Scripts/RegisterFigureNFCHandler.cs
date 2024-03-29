using distriqt.plugins.nfc;
using System;
using UI.Dialogs;
using UnityEngine;

public class RegisterFigureNFCHandler : MonoBehaviour
{
    private string ConnectionIdString = "RF_NFCHANDLER";

    // Start is called before the first frame update
    void Start()
    {
        Connection.Subscribe(ConnectionIdString, this);
        bool isNFCSupported = NFC.isSupported;

        if (!isNFCSupported)
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
            }
            return ascii;
        }
        catch (Exception ex) { Console.WriteLine(ex.Message); }
        return string.Empty;
    }

    /// <summary>
    /// Called when an NFC chip is reads
    /// </summary>
    /// <param name="e">NFC Event</param>
    void Instance_OnNDEFDiscovered(NFCEvent e)
    {
        // traverse all messages
        foreach (NdefMessage ndefMessage in e.tag.messages)
        {
            string figure_id = "";
            // traverse records in message
            foreach (NdefRecord record in ndefMessage.records)
            {
                figure_id += record.payload;
            }
            figure_id = StringToASCII(figure_id);

            string message = "REGISTER FIGURE\n" +
                Globals.ACTIVE_USER_ID + "\n" +
                figure_id;

            Connection.QueueMessage(message);
        }
    }
}
