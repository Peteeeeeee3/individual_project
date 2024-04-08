using distriqt.plugins.nfc;
using UnityEngine;

public enum NFC_TARGET_TYPE
{
    GAME_NFC_HANDLER,
    MAIN_MENU_NFC_HANDLER,
    NONE
}

public static class StaticNFCHandler
{
    private static NFC_TARGET_TYPE targetType = NFC_TARGET_TYPE.NONE;
    private static MonoBehaviour nfcTarget;

    /// <summary>
    /// Activates NFC behaviour
    /// </summary>
    public static void Activate()
    {
        // set relevant MIME types
        // this ignores all scanned MIME types not set -> records on NFC tags for this project store RTD_TEXT (texxt/plain)
        ScanOptions scanOptions = new ScanOptions();
        scanOptions.mimeTypes = new string[] { "text/plain" };

        // register application for NFC scanning when running in foreground
        NFC.Instance.RegisterForegroundDispatch(scanOptions);

        // register Instance_OnNDEFDiscovered() to be registered to be called when NFC tag with NDEF format is detected
        NFC.Instance.OnNdefDiscovered += Instance_OnNDEFDiscovered;
    }

    /// <summary>
    /// Can be called to claim controller of NFC behaviour.
    /// Sets claimant as target of NFC callback.
    /// </summary>
    public static void ClaimControl(NFC_TARGET_TYPE type, MonoBehaviour target)
    {
        if (type == NFC_TARGET_TYPE.NONE) { return; }

        targetType = type;
        nfcTarget = target;

        string debugText = targetType == NFC_TARGET_TYPE.GAME_NFC_HANDLER ? "GAME_NFC_HANDLER" : "MAIN_MENU_NFC_HANDLER";
        Connection.QueueMessage("Control claimed by: " + debugText);
    }

    /// <summary>
    /// Called when an NFC chip is reads
    /// </summary>
    /// <param name="e">NFC Event</param>
    static void Instance_OnNDEFDiscovered(NFCEvent e)
    {
        if (targetType == NFC_TARGET_TYPE.NONE) { return; } 

        if (targetType == NFC_TARGET_TYPE.GAME_NFC_HANDLER)
        {
            ((GameNFCHandler)nfcTarget).Instance_OnNDEFDiscovered(e);
        }
        else if (targetType == NFC_TARGET_TYPE.MAIN_MENU_NFC_HANDLER)
        {
            ((RegisterFigureNFCHandler)nfcTarget).Instance_OnNDEFDiscovered(e);
        }
    }

    /// <summary>
    /// Used to abstract NFC dependencies in other classes
    /// </summary>
    /// <returns>True if NFC is supported by device</returns>
    public static bool IsNfcSupported()
    {
        return NFC.isSupported;
    }
}
