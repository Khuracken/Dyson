using UnityEngine;
using UnityEngine.InputSystem;

/* From Unity Documentation, URL: https://docs.unity3d.com/ScriptReference/CharacterController.Move.html */

[RequireComponent(typeof(CharacterController))]
public class PlayerController2 : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [SerializeField] private float playerSpeed = 4f;
    [SerializeField] private float jumpHeight = 0.2f;
    [SerializeField] private float gravityValue = -1.62f;

    private Vector2 hMovement = Vector2.zero;
    private bool vMovement = false;

    public float playerDMGTaken;                            // Player DMG taken
    [SerializeField] public float playerHP;                 // Player Health (HitPoints)
    [SerializeField] public float playerMaxHP;              // Player MAX Health (HitPoints)

    public P2HP p2hp;            // Reference to player #2 HP C# Script 

    private void Start() {

        playerDMGTaken = 1;         // Setting up damage points taken
        playerMaxHP = 100;          // Setting up player max hp
        playerHP = playerMaxHP;     // Setting up the player HP

        p2hp.SetMaxHP(playerHP);

        controller = gameObject.GetComponent<CharacterController>();
    }

    public void onMove(InputAction.CallbackContext context) {

        hMovement = context.ReadValue<Vector2>();
    }

    public void onJump(InputAction.CallbackContext context) {

        vMovement = context.action.triggered;
    }

    void Update() {

        //Debug.Log("[P2] Current HP is: " + playerHP);

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) {

            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(hMovement.x, 0, hMovement.y);
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero) {

            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (vMovement && groundedPlayer) {

            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void OnTriggerStay(Collider collider)
    {

        // HANDLE COLLISION WITH ENEMY
        if (collider.GetComponent<Collider>().tag == "EnemyAI" &&
            GameObject.FindGameObjectWithTag("Shield") == null)
        {

            playerHP -= playerDMGTaken;     // Player loses HP
            p2hp.SetHP(playerHP);

            // LOAD MAIN MENU SCREEN WHEN PLAYER HP HAS REACHED 0
            if (playerHP <= 0)
            {
                Debug.Log("P2 is dead");
                //playerHP = playerMaxHP;    // Setting up the PlayerHP
                Destroy(GameObject.Find("Player #2"));
                //menu.LoadMenu();            // Loads main menu
            }
        }
    }
}
