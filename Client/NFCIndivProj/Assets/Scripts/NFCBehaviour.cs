using System.Collections;
using TMPro;
using UnityEngine;

public class NFCBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI NFCText;
    [SerializeField]
    private TextMeshProUGUI DebugText1;
    [SerializeField]
    private TextMeshProUGUI DebugText2;
    [SerializeField]
    private TextMeshProUGUI DebugText3;
    [SerializeField]
    private TextMeshProUGUI DebugText4;
    [SerializeField]
    private TextMeshProUGUI DebugText5;

    bool background = true;

    // Start is called before the first frame update
    void Start()
    {
        AndroidNFCReader.enableBackgroundScan();
        DebugText1.SetText("Background Scan Enabled");
        AndroidNFCReader.ScanNFC(gameObject.name, "OnFinishScan");
        DebugText2.SetText("ScanNFC() called and set");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGUI()
    {
    }

    // NFC callback
    void OnFinishScan(string result)
    {
        DebugText3.SetText("OnFinishScan() called");
        // Cancelled
        if (result == AndroidNFCReader.CANCELLED)
        {

            // Error
        }
        else if (result == AndroidNFCReader.ERROR)
        {


            // No hardware
        }
        else if (result == AndroidNFCReader.NO_HARDWARE)
        {
        }

        NFCText.SetText(result);
        DebugText4.SetText("test set to: " + result);
    }
}
