using TMPro;
using UnityEngine;

public class NFCMessanger : MonoBehaviour 
{
    [SerializeField]
    public TextMeshProUGUI debugText;
    public bool Initialized { get; set; }
    public bool IsRead { get; set; } = false;
    public GameNFCHandler NfcHandler { get; set; }
    public Figure Figure { get; set; }

    private void Start()
    {
        Initialized = false; // CHANGE THIS TO FALSE WHEN NOT TESTING IN UNITY
        debugText.SetText("NFCMessanger created");
        /*Figure = new Figure(
            "Figure-Testing-Id",
            PlayerType.CREAM,
            1,
            0,
            150,
            10,
            0.7f,
            1,
            "GREY-Id");*/
    }
}