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
        public bool fakeDrink1;
        public bool fakeDrink2;
        public float player1camDist;
        public float player2camDist;
        public bool troll1;
        public bool troll2;
        public bool trollingP1;
        public bool trollingP2;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref player1Score);
            serializer.SerializeValue(ref player2Score);
            serializer.SerializeValue(ref player1MoveSpeed);
            serializer.SerializeValue(ref player2MoveSpeed);
            serializer.SerializeValue(ref fakeDrink1);
            serializer.SerializeValue(ref fakeDrink2);
            serializer.SerializeValue(ref player1camDist);
            serializer.SerializeValue(ref player2camDist);
            serializer.SerializeValue(ref troll1);
            serializer.SerializeValue(ref troll2);
            serializer.SerializeValue(ref trollingP1);
            serializer.SerializeValue(ref trollingP2);

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameState.player1Score = 0;
        gameState.player2Score = 0;
        gameState.player1MoveSpeed = 5.0f;
        gameState.player2MoveSpeed = 5.0f;
        gameState.bucketActive = false;
        gameState.fakeDrinkActive1= false;
        gameState.fakeDrinkActive2= false;
        gameState.player1cam = -21.0f;
        gameState.player2cam = -21.0f;
        gameState.trollActive1 = false;
        gameState.trollActive2 = false;
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

            if (gameState.bucketActive)
            {
                gameState.bucketActive = false;

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
            yield return new WaitForSeconds(waitTime);
            RequestUpdateSpeedClientRpc(2);
            
        }
        else
        {
            RequestUpdateSpeedClientRpc(1);
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


    [ServerRpc(RequireOwnership = false)]
    void SetDecoyStatusServerRpc(string name, ServerRpcParams serverRpcParams = default)
    {
        ulong playerNum = serverRpcParams.Receive.SenderClientId;
        if (playerNum == 0)
        {
            gameState.fakeDrinkActive1 = true;
        }
        else gameState.fakeDrinkActive2 = true;

        PlayerNetworkData boolData = new PlayerNetworkData()
        {
            fakeDrink1 = gameState.fakeDrinkActive1,
            fakeDrink2 = gameState.fakeDrinkActive2,
        };
        RequestDestroyFakeDrinkClientRpc(name, boolData);
    }


    [ClientRpc]
    void RequestDestroyFakeDrinkClientRpc(string tag, PlayerNetworkData boolData)
    {

        GameObject temp;
        temp = GameObject.Find(tag);
        Destroy(temp);

        this.gameState.fakeDrinkActive1 = boolData.fakeDrink1;
        this.gameState.fakeDrinkActive2 = boolData.fakeDrink2;
    }


    [ServerRpc(RequireOwnership = false)]
    void RequestCollectFakeDrinkChildServerRpc(string tag, ServerRpcParams serverRpcParams = default)
    {
        RequestDestroyFakeDrinkChildClientRpc(tag);
    }

    [ClientRpc]
    void RequestDestroyFakeDrinkChildClientRpc(string tag)
    {

        GameObject temp;
        temp = GameObject.Find(tag);
        Destroy(temp);


    }

    [ServerRpc(RequireOwnership = false)]
    void RequestCollectGlassesServerRpc(string name, ServerRpcParams serverRpcParams = default)
    {
        ulong playerNum = serverRpcParams.Receive.SenderClientId;

        if (playerNum == 0)
        {
            StartCoroutine(GlassesZoomTimer(10.0f, 0));
        }
        else StartCoroutine(GlassesZoomTimer(10.0f, 1));

        RequestDestroyGlassesClientRpc(name);
    }
    [ClientRpc]
    void RequestDestroyGlassesClientRpc(string tag)
    {
        GameObject temp;
        temp = GameObject.Find(tag);
        Destroy(temp);

    }
    [ClientRpc]
    void RequestUpdateCamDistClientRpc(ulong playerNum)
    {
        if (playerNum == 0)
        {
            gameState.player2cam = -5.0f;
        }
        else if (playerNum == 1)
        {
            gameState.player1cam = -5.0f;
        }
        else if (playerNum == 2)
        {
            gameState.player1cam = -21.0f;
            gameState.player2cam = -21.0f;
        }


    }
    IEnumerator GlassesZoomTimer(float waitTime, ulong playerNum)
    {
        if(playerNum == 0)
        {
            RequestUpdateCamDistClientRpc(0);
            yield return new WaitForSeconds(waitTime);
            RequestUpdateCamDistClientRpc(2);
        }
        else
        {
            RequestUpdateCamDistClientRpc(1);
            yield return new WaitForSeconds(waitTime);
            RequestUpdateCamDistClientRpc(2);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetTrollStatusServerRpc(string name, ServerRpcParams serverRpcParams = default)
    {
        ulong playerNum = serverRpcParams.Receive.SenderClientId;


        StartCoroutine(TrollActiveTimer(10.0f, playerNum));

        RequestDestroyTrollClientRpc(name);
    }

    [ClientRpc]
    void RequestDestroyTrollClientRpc(string name)
    {
        GameObject temp;
        temp = GameObject.Find(name);
        Destroy(temp);
    }
    [ClientRpc]
    void RequestUpdateTrollClientRpc(ulong playerNum)
    {
        if (playerNum == 0)
        {
            gameState.trollActive1 = true;
            gameState.trollingPlayer2 = true;
        }
        else if (playerNum == 1)
        {
            gameState.trollActive2 = true;
            gameState.trollingPlayer1 = true;
        }
        else if (playerNum == 2)
        {
            gameState.trollActive1 = false;
            gameState.trollActive2 = false;
            gameState.trollingPlayer1 = false;
            gameState.trollingPlayer2 = false;
        }


    }
    IEnumerator TrollActiveTimer(float waitTime, ulong playerNum)
    {
        if (playerNum == 0)
        {
            RequestUpdateTrollClientRpc(0);
            yield return new WaitForSeconds(waitTime);
            RequestUpdateTrollClientRpc(2);

        }
        else
        {
            RequestUpdateTrollClientRpc(1);
            yield return new WaitForSeconds(waitTime);
            RequestUpdateTrollClientRpc(2);

        }


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
            gameState.bucketActive = true;
            RequestCollectBucketServerRpc(collision.gameObject.name);
        }
        
        if (collision.gameObject.tag == "Troll")
        {
            SetTrollStatusServerRpc(collision.gameObject.name);
        }
        if (collision.gameObject.tag == "FakeDrink")
        {
            SetDecoyStatusServerRpc(collision.gameObject.name);
        }



        if (collision.gameObject.tag == "FakeDrinkChild")
        {
            RequestCollectFakeDrinkChildServerRpc(collision.gameObject.name);
        }

        if (collision.gameObject.tag == "Glasses")
        {
            RequestCollectGlassesServerRpc(collision.gameObject.name);
        }
        if (collision.gameObject.tag == "FakeCollider")
        {
            string parentName = collision.transform.parent.gameObject.name;
            RequestCheckTrollingServerRpc(parentName);
        }
        
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestCheckTrollingServerRpc(string name, ServerRpcParams serverRpcParams = default)
    {
        ulong playerNum = serverRpcParams.Receive.SenderClientId;

        if(playerNum == 0)
        {
            if (gameState.trollingPlayer1)
            {
                RequestDestroyDrinkEarlyClientRpc(name, playerNum);
            }
        }
        if(playerNum == 1)
        {
            if (gameState.trollingPlayer2)
            {
                RequestDestroyDrinkEarlyClientRpc(name, playerNum);
            }
        }
    }

    [ClientRpc]
    void RequestDestroyDrinkEarlyClientRpc(string name , ulong playerNum)
    {
        GameObject parent;
        
        
        parent = GameObject.Find(name);
        Destroy(parent);
        
    }
}
