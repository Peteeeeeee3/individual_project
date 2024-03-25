using System.Collections.Generic;
using TMPro;
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
    private GameObject bulletPrefab;
    [SerializeField]
    private float blueAttackInterval;
    [SerializeField]
    private float greyAttackInterval;
    [SerializeField]
    private float creamAttackInterval;
    [SerializeField]
    private GameObject gameNotReadyPanel;
    [SerializeField]
    private GameObject radialAttackPrefab;
    [SerializeField]
    private float radAttStartSize;
    [SerializeField]
    private float radAttEndSize;
    [SerializeField]
    private float radAttGrowthRate;

    // DEBUG
    [SerializeField]
    private TextMeshProUGUI DebugText5;
    //

    public float health { get; set; }
    public Transform activePlayerModel { get; private set; }
    private PlayerType activePlayerType;
    public string activePlayerID { get; private set; }
    private float attackTimer = 0;
    private Dictionary<PlayerType, float> timerResetDict;
    private bool isReady = false;
    private Transform radialAttack;
    public int expCollected { get; private set; } = 0;

    // Start is called before the first frame update
    void Start()
    {
        // REMOVE THIS - START
        activePlayerModel = greyCube; // temp for testing only
        activePlayerID = "NFC-GAME-FIGURE-1708202131970";
        activePlayerType = PlayerType.GREY;
        health = 200;
        // REMOVE THIS - END
        timerResetDict = new Dictionary<PlayerType, float>
        {
            { PlayerType.BLUE, blueAttackInterval },
            { PlayerType.GREY, greyAttackInterval },
            { PlayerType.CREAM, creamAttackInterval }
        };
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        ReadMessanger();

        if (!isReady) { return; }

        OnUpdatePlayer();
        OnUpdateCamera();

        if (radialAttack)
        {
            OnUpdateRadialAttack();
        }

        DebugText5.SetText("Health: " + health);
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
        
        // make sure player is always on the ground
        if (activePlayerModel.position.y != 5)
        {
            activePlayerModel.position = new Vector3(activePlayerModel.position.x, 5, activePlayerModel.position.z);
        }

        // handle attack only if right stick is in use
        Vector3 attackVec = new Vector3(attackJoystick.Horizontal, 0, attackJoystick.Vertical);
        if (attackVec.magnitude > 0.5)
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
                        GameObject bullet1 = Instantiate(bulletPrefab, activePlayerModel.position - normVec * bulletOffset, Quaternion.identity);
                        Bullet bulletComp1 = bullet1.GetComponent<Bullet>();
                        bulletComp1.moveDir = attackVec;
                        bulletComp1.damage = 10;
                        bulletComp1.ownerTag = "Player";
                        bulletComp1.isGrenade = false;
                        GameObject bullet2 = Instantiate(bulletPrefab, activePlayerModel.position + normVec * bulletOffset, Quaternion.identity);
                        Bullet bulletComp2 = bullet2.GetComponent<Bullet>();
                        bulletComp2.moveDir = attackVec;
                        bulletComp2.damage = 10;
                        bulletComp2.ownerTag = "Player";
                        bulletComp2.isGrenade = false;
                        break;
                
                    case PlayerType.GREY:
                        // create grenade
                        GameObject grenade = Instantiate(bulletPrefab, activePlayerModel.position, Quaternion.identity);
                        Bullet grenadeBullet = grenade.GetComponent<Bullet>();
                        grenadeBullet.moveDir = attackVec;
                        grenadeBullet.damage = 40;
                        grenadeBullet.ownerTag = "Player";
                        grenadeBullet.isGrenade = true;
                        break;
                
                    case PlayerType.CREAM:
                        radialAttack = Instantiate(radialAttackPrefab, activePlayerModel.position - new Vector3(0, 3, 0), Quaternion.identity).GetComponent<Transform>();
                        radialAttack.localScale = new Vector3(radAttStartSize, 1, radAttStartSize);
                        radialAttack.GetComponent<RadialAttackAttributes>().damage = 10;
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
    /// Updates the behavious of the radial attack
    /// </summary>
    private void OnUpdateRadialAttack()
    {
        radialAttack.position = activePlayerModel.position - new Vector3(0, 3, 0);
        radialAttack.localScale = new Vector3(radialAttack.localScale.x + radAttGrowthRate * Time.deltaTime, 1,
            radialAttack.localScale.z + radAttGrowthRate * Time.deltaTime);

        if (radialAttack.localScale.x >= radAttEndSize || radialAttack.localScale.z >= radAttEndSize) 
        { 
            Destroy(radialAttack.gameObject);
            radialAttack = null;
        }
    }

    /// <summary>
    /// Updates the camera
    /// </summary>
    private void OnUpdateCamera()
    {
        cam.position = new Vector3(activePlayerModel.position.x, cam.position.y, activePlayerModel.position.z - 20);
        cam.LookAt(activePlayerModel.position);
    }

    /// <summary>
    /// Reads and handles any updates to the NFC Messanger instance
    /// </summary>
    private void ReadMessanger()
    {
        // check for detection of a new figure, only numeric part of ID string is unique
        if (!NfcMessanger.IsRead &&
            NfcMessanger.Initialized)
        {
            // assign new player model based on 
            bool noError = true;
            switch (NfcMessanger.Figure.type)
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
                activePlayerID = NfcMessanger.Figure.id;
                activePlayerType = NfcMessanger.Figure.type;
                isReady = true;
                Time.timeScale = 1;
                gameNotReadyPanel.GetComponent<RectTransform>().position = new Vector3(100000000, 100000000, 1);
            }

            NfcMessanger.IsRead = true;
        }
    }

    /// <summary>
    /// Inflict damage to player
    /// </summary>
    /// <param name="damage">how much damage to concede</param>
    public void TakeDamage(float damage)
    {
        health -= damage;
    }

    /// <summary>
    /// Adds experience points gained
    /// </summary>
    /// <param name="exp">experience points to gain</param>
    public void GainExp(int exp)
    {
        expCollected += exp;
    }
}
