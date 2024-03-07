using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bl_Joystick joystick;
    [SerializeField]
    private float moveSpeed;

    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = greyCube; // temp for testing only
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayer();
        UpdateCamera();
    }

    /// <summary>
    /// Updates the player
    /// </summary>
    private void UpdatePlayer()
    {
        // update movement
        Vector3 moveVec = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        moveVec.Normalize();
        Debug.Log(Time.deltaTime);
        player.GetComponent<CharacterController>().Move(moveVec * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Updates the camera
    /// </summary>
    private void UpdateCamera()
    {
        cam.position = new Vector3(player.position.x, cam.position.y, player.position.z);
    }
}
