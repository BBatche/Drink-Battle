using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class DrinkCollectorScript : NetworkBehaviour
{
    public GameState gameState;
    NetworkManager nm;


    struct PlayerNetworkData : INetworkSerializable
    {
        public int player1Score;
        public int player2Score;
        public float player1MoveSpeed;
        public float player2MoveSpeed;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref player1Score);
            serializer.SerializeValue(ref player2Score);
            serializer.SerializeValue(ref player1MoveSpeed);
            serializer.SerializeValue(ref player2MoveSpeed);

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameState.player1Score = 0;
        gameState.player2Score = 0;
        gameState.player1MoveSpeed = 5.0f;
        gameState.player2MoveSpeed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player Hit");
            if (gameState.bucketActive)
            {
                gameState.bucketActive = false;
                Debug.Log("P1-" + gameState.player1Score);
                Debug.Log("P2-" + gameState.player2Score);
                Debug.Log("Player Hit and Bucket Active");
                RequestStealPointsServerRpc();
            }

        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestCollectDrinkServerRpc(string tag, ServerRpcParams serverRpcParams = default)
    {
        //Server can refer to master list in spawner to see if tag and location are valid
        //Client could pass in location of Drink

        ulong playerNum = serverRpcParams.Receive.SenderClientId;

        if (playerNum == 0)
        {
            gameState.player1Score += 1;
            Debug.Log(gameState.player1Score);
        }
        else { gameState.player2Score += 1; }

        PlayerNetworkData scoreData = new PlayerNetworkData()
        {
            player1Score = gameState.player1Score,
            player2Score = gameState.player2Score,
        };

        RequestDestroyDrinkClientRpc(tag, scoreData);


        //else ban cheater

    }


    [ClientRpc]
    void RequestDestroyDrinkClientRpc(string name, PlayerNetworkData scoreData)
    {
        GameObject temp;
        temp = GameObject.Find(name);
        Destroy(temp);

        //clients copy over current scoring
        this.gameState.player1Score = scoreData.player1Score;
        this.gameState.player2Score = scoreData.player2Score;
    }


    //contact server to validate collecting Epoxy
    [ServerRpc(RequireOwnership = false)]
    void RequestCollectEpoxyServerRpc(string tag, ServerRpcParams serverRpcParams = default)
    {

        ulong playerNum = serverRpcParams.Receive.SenderClientId;

        StartCoroutine(EpoxySlowTimer(10.0f, playerNum));

        RequestDestroyEpoxyClientRpc(tag);
    }
    [ClientRpc]
    void RequestDestroyEpoxyClientRpc(string name)
    {
        GameObject temp;
        temp = GameObject.Find(name);
        Destroy(temp);

        
    }
    [ClientRpc]
    void RequestUpdateSpeedClientRpc(ulong playerNum)
    {
        if(playerNum == 0)
        {
            gameState.player2MoveSpeed = 5.0f * .25f;
        }
        else if(playerNum == 1)
        {
            gameState.player1MoveSpeed = 5.0f * .25f;
        }
        else if(playerNum == 2)
        {
            gameState.player2MoveSpeed = 5.0f;
            gameState.player1MoveSpeed = 5.0f;
        }

        
    }
    IEnumerator EpoxySlowTimer(float waitTime,ulong playerNum)
    {
        if (playerNum == 0)
        {
            RequestUpdateSpeedClientRpc(0);
            
            Debug.Log("Player 2 Move Speed" + gameState.player2MoveSpeed);
            yield return new WaitForSeconds(waitTime);
            RequestUpdateSpeedClientRpc(2);
            
        }
        else
        {
            RequestUpdateSpeedClientRpc(1);
            Debug.Log("Player 1 Move Speed" + gameState.player1MoveSpeed);
            yield return new WaitForSeconds(waitTime);
            RequestUpdateSpeedClientRpc(2);
            
        }
        

    }
    [ServerRpc(RequireOwnership = false)]
    void RequestCollectBucketServerRpc(string tag, ServerRpcParams serverRpcParams = default)
    {
        RequestDestroyBucketClientRpc(tag);
    }

    [ClientRpc]
    void RequestDestroyBucketClientRpc(string tag)
    {
        Debug.Log("Destroying:" + tag);
        GameObject temp;
        temp = GameObject.Find(tag);
        Destroy(temp);
    

    }

    IEnumerator BucketTimer(float waitTime, ulong playerNum)
    {
        yield return new WaitForSeconds(waitTime);
        gameState.bucketActive = false;
    }

    [ServerRpc(RequireOwnership =false)]
    void RequestStealPointsServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong playerNum = serverRpcParams.Receive.SenderClientId;
        Debug.Log("Player" + playerNum + "Stealing points");
        if (playerNum == 0)
        {

            gameState.player1Score = gameState.player1Score +  gameState.player2Score/2;
            gameState.player2Score =  gameState.player2Score/2;
            gameState.bucketActive = false;
        }
        else
        {
            gameState.player2Score = gameState.player2Score +gameState.player1Score/2;
            gameState.player1Score = gameState.player1Score/2;
            gameState.bucketActive = false;
        }

        Debug.Log("P1Score- " + gameState.player1Score);
        Debug.Log("P2Score- " + gameState.player2Score);
        PlayerNetworkData scoreData = new PlayerNetworkData()
        {
            player1Score = gameState.player1Score,
            player2Score = gameState.player2Score,
        };

        RequestStealPointsClientRpc(scoreData);
    }

    [ClientRpc]
    void RequestStealPointsClientRpc(PlayerNetworkData scoreData)
    {
        
        this.gameState.player1Score = scoreData.player1Score;
        this.gameState.player2Score = scoreData.player2Score;
    }

    private void OnTriggerEnter2D(Collider2D collision)
        {
        if(collision.gameObject.tag == "Drink")
        {
            
            RequestCollectDrinkServerRpc(collision.gameObject.name);
        }
        if(collision.gameObject.tag == "Epoxy")
        {
            RequestCollectEpoxyServerRpc(collision.gameObject.name);
            
        }
        if(collision.gameObject.tag == "Bucket")
        {
            Debug.Log("Collided with Bucket-" + collision.gameObject.name);
            gameState.bucketActive = true;
            RequestCollectBucketServerRpc(collision.gameObject.name);
        }
        
        if (collision.gameObject.tag == "Troll")
        {

        }
    }
}
