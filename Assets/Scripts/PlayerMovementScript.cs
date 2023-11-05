using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerMovementScript : NetworkBehaviour
{
    public GameState gameState;
    float xDirection = 0.0f;
    float yDirection = 0.0f;

    float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            moveSpeed = gameState.player1MoveSpeed;
            Debug.Log("P1 MS - " + moveSpeed);
            xDirection = Input.GetAxisRaw("Horizontal");
            yDirection = Input.GetAxisRaw("Vertical");

            Vector3 move = new Vector3(xDirection, yDirection, 0);

            transform.Translate(move * Time.deltaTime * moveSpeed);
        }
        else
        {
            moveSpeed = gameState.player2MoveSpeed;
            Debug.Log("P2 MS - " + moveSpeed);
            xDirection = Input.GetAxisRaw("Horizontal");
            yDirection = Input.GetAxisRaw("Vertical");

            Vector3 move = new Vector3(xDirection, yDirection, 0);

            transform.Translate(move * Time.deltaTime * moveSpeed);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }


}
