using distriqt.plugins.nfc;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class RegisterFigureNFCHandler : MonoBehaviour
{
    private string ConnectionIdString = "RF_NFCHANDLER";

    // Start is called before the first frame update
    void Start()
    {
        Connection.Subscribe(ConnectionIdString, this);

        StaticNFCHandler.ClaimControl(NFC_TARGET_TYPE.MAIN_MENU_NFC_HANDLER, this);
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
                ascii = Regex.Replace(ascii, "\x02en", "");
                ascii = ascii.Replace("\x02", "");
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
    public void Instance_OnNDEFDiscovered(NFCEvent e)
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
