using UnityEngine;

public class NFCMessanger : MonoBehaviour 
{
    public Transform activePlayerModel { get; set; }
    public PlayerType playerType { get; set; }
    public string ID { get; set; }
    public NFCHandler NfcHandler { get; set; }
    public bool initialized { get; set; }

    private void Start()
    {
        initialized = true; // CHANGE THIS TO FALSE WHEN NOT TESTING IN UNITY
        ID = "------------------------------------------------";
        playerType = PlayerType.CREAM;
        activePlayerModel = null;
    }
}