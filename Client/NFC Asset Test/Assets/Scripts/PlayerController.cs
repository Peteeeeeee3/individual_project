using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    BLUE,
    GREY,
    CREAM,
    ERROR
}

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform cam;
    [SerializeField]
    private Transform blueCube;
    [SerializeField]
    private Transform greyCube;
    [SerializeField]
    private Transform creamCube;
    [SerializeField]
    private bl_Joystick movementJoystick;
    [SerializeField]
    private bl_Joystick attackJoystick;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private NFCMessanger NfcMessanger;
    [SerializeField]
    private GameObject BulletAsset;
    [SerializeField]
    private float blueAttackInterval;
    [SerializeField]
    private float greyAttackInterval;
    [SerializeField]
    private float creamAttackInterval;

    private Transform activePlayerModel;
    private PlayerType activePlayerType;
    private string activePlayerID;
    private float attackTimer = 0;
    private Dictionary<PlayerType, float> timerResetDict;

    // Start is called before the first frame update
    void Start()
    {
        activePlayerModel = greyCube; // temp for testing only
        activePlayerID = "_just_testing_01";
        activePlayerType = PlayerType.ERROR;
        timerResetDict = new Dictionary<PlayerType, float>
        {
            { PlayerType.BLUE, blueAttackInterval },
            { PlayerType.GREY, greyAttackInterval },
            { PlayerType.CREAM, creamAttackInterval }
        };
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdatePlayer();
        OnUpdateCamera();
        ReadMessanger();
    }

    /// <summary>
    /// Updates the player
    /// </summary>
    private void OnUpdatePlayer()
    {
        // update movement
        Vector3 moveVec = new Vector3(movementJoystick.Horizontal, 0, movementJoystick.Vertical);
        moveVec.Normalize();
        activePlayerModel.GetComponent<CharacterController>().Move(moveVec * moveSpeed * Time.deltaTime);

        // handle attack only if right stick is in use
        Vector3 attackVec = new Vector3(attackJoystick.Horizontal, 0, attackJoystick.Vertical);
        if (attackVec.magnitude > 0)
        {
            attackVec.Normalize();

            // update attack timer
            attackTimer += Time.deltaTime;

            // attack only if time interval has passed
            float timerResetValue;
            timerResetDict.TryGetValue(activePlayerType, out timerResetValue);
            if (attackTimer > timerResetValue)
            {
                switch (activePlayerType)
                {
                    case PlayerType.BLUE:
                        // find a vector normal to attack vector, use y-axis as attack vector is always parallel to x & z-axis
                        Vector3 normVec = Vector3.Cross(attackVec, new Vector3(0, 1, 0));
                        float bulletOffset = 10;
                        // creat bullets
                        GameObject bullet1 = Instantiate(BulletAsset, activePlayerModel.position - normVec * bulletOffset, Quaternion.identity);
                        bullet1.GetComponent<Bullet>().moveDir = attackVec;
                        GameObject bullet2 = Instantiate(BulletAsset, activePlayerModel.position + normVec * bulletOffset, Quaternion.identity);
                        bullet2.GetComponent<Bullet>().moveDir = attackVec;
                        break;
                
                    case PlayerType.GREY:
                        break;
                
                    case PlayerType.CREAM:
                        break;

                    case PlayerType.ERROR:
                        // do not attack if player is error type
                        break;
                }

                attackTimer = 0;
            }
        }
    }

    /// <summary>
    /// Updates the camera
    /// </summary>
    private void OnUpdateCamera()
    {
        cam.position = new Vector3(activePlayerModel.position.x, cam.position.y, activePlayerModel.position.z);
    }

    /// <summary>
    /// Reads and handles any updates to the NFC Messanger instance
    /// </summary>
    private void ReadMessanger()
    {
        // check for detection of a new figure, only numeric part of ID string is unique
        if (!activePlayerID.Substring(16).Equals(NfcMessanger.ID.Substring(16)))
        {
            // TODO: validate ID -> break if invalid
            //NfcMessanger.NfcHandler.ValidateFigure();

            // assign new player model based on 
            bool noError = true;
            switch (NfcMessanger.playerType)
            {
                case PlayerType.BLUE:
                    activePlayerModel = blueCube;
                    break;
                case PlayerType.GREY:
                    activePlayerModel = greyCube;
                    break;
                case PlayerType.CREAM:
                    activePlayerModel = creamCube;
                    break;
                case PlayerType.ERROR:
                    noError = false;
                    // TODO: send error message to player
                    Debug.Log("Invalid or unsupported player model");
                    break;
            }

            // update all active player information only if all data is valid for use
            if (noError)
            {
                activePlayerID = NfcMessanger.ID;
                activePlayerType = NfcMessanger.playerType;
            }
        }
    }
}
