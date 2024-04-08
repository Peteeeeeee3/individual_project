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
        StaticNFCHandler.ClaimControl(NFC_TARGET_TYPE.GAME_NFC_HANDLER, this);
        NFCsupportedText.SetText(NFC.isSupported.ToString());

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
    public void Instance_OnNDEFDiscovered(NFCEvent e)
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
    /// Provided in plugin documentation: https://docs.airnativeextensions.com/docs/nfc/scanning (minor adjustments made)
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
            Connection.QueueMessage("BLUE");
        }
        else if (data[dataOffset + 1].Equals("GREY"))
        {
            type = PlayerType.GREY;
            Connection.QueueMessage("GREY");
        }
        else if (data[dataOffset + 1].Equals("CREAM"))
        {
            type = PlayerType.CREAM;
            Connection.QueueMessage("CREAM");
        }
        else
        {
            Connection.QueueMessage("ERROR");
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
        NfcMessanger.Initialized = true;
    }

    /// <summary>
    /// Handles the destruction of this MonoBehaviour class
    /// </summary>
    private void OnDestroy()
    {
        Connection.Unsubscribe("NFCHANDLER");
    }
}
