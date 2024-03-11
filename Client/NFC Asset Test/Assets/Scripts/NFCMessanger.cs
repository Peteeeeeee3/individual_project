using UnityEngine;

public class NFCMessanger : MonoBehaviour 
{
    public Transform activePlayerModel { get; set; }
    public PlayerType playerType { get; set; }
    public string ID { get; set; }
    public NFCHandler NfcHandler { get; set; }

    private void Start()
    {
        ID = "------------------------------------------------";
    }
}