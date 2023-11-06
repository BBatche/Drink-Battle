using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.PlayerSettings;


public class PlayerMovementScript : NetworkBehaviour
{
    public GameState gameState;
    public NetSpawnerScript ns;
    float xDirection = 0.0f;
    float yDirection = 0.0f;
    Camera camera;
    Vector3 playerPosition;

    float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        ns = FindObjectOfType<NetSpawnerScript>();
        camera = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {   

            
            
            moveSpeed = gameState.player1MoveSpeed;
            xDirection = Input.GetAxisRaw("Horizontal");
            yDirection = Input.GetAxisRaw("Vertical");

            Vector3 move = new Vector3(xDirection, yDirection, 0);

            transform.Translate(move * Time.deltaTime * moveSpeed);
            camera.transform.position = new Vector3(playerPosition.x, playerPosition.y, gameState.player1cam);
            playerPosition = new Vector3(transform.position.x +2.0f, transform.position.y+1.0f, 0);
            if (gameState.fakeDrinkActive1 && Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Server Fake Drink Child");
                ns.RequestSpawnFakeDrinkChildServerRpc(playerPosition);
            }
        }
        else
        {
        
            moveSpeed = gameState.player2MoveSpeed;
            xDirection = Input.GetAxisRaw("Horizontal");
            yDirection = Input.GetAxisRaw("Vertical");

            Vector3 move = new Vector3(xDirection, yDirection, 0);

            transform.Translate(move * Time.deltaTime * moveSpeed);
            camera.transform.position = new Vector3(playerPosition.x, playerPosition.y, gameState.player2cam);
            playerPosition = new Vector3 (transform.position.x+2.0f, transform.position.y+1.0f, 0); 
            if (gameState.fakeDrinkActive2 && Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Client Fake Drink Child");
                ns.RequestSpawnFakeDrinkChildServerRpc(playerPosition);
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner)
        {
            enabled = false;
        }
    }
}
