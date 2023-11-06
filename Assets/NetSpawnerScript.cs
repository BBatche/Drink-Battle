using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

using Unity.Networking;

public class NetSpawnerScript : NetworkBehaviour
{
    public GameObject drinkPrefab;
    public GameObject epoxyPrefab;
    public GameObject trollPrefab;
    public GameObject bucketPrefab;
    public GameObject fakeDrinkPrefab;
    public GameObject fakeDrinkChildPrefab;
    public GameObject glassesPrefab;
    public NetworkManager nm;
    public GameState gameState;
    

    private bool gameStarted = false;


    struct PlayerNetworkData : INetworkSerializable
    {
        public int player1Score;
        public int player2Score;
        public float player1MoveSpeed;
        public float player2MoveSpeed;
        public bool fakeDrink1;
        public bool fakeDrink2;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref player1Score);
            serializer.SerializeValue(ref player2Score);
            serializer.SerializeValue(ref player1MoveSpeed);
            serializer.SerializeValue(ref player2MoveSpeed);
            serializer.SerializeValue(ref fakeDrink1);
            serializer.SerializeValue(ref fakeDrink2);
        }
    }

    IEnumerator SpawnDrinksAtRate(float waitTime)
    {
        while (true)
        {
            if (nm.ConnectedClients.Count >= 2)
            {
                yield return new WaitForSeconds(waitTime);
                float x = Random.Range(-20.0f, 20.0f);
                float y = Random.Range(-4.5f, 4.5f);
                Vector3 position = new Vector3(x, y, 0.0f);
                string name = "Drink-" + System.Guid.NewGuid().ToString();
                RequestSpawnDrinksClientRpc(position, name);
            }

        }
    }
    IEnumerator SpawnEpoxyAtRate(float waitTime)
    {
        while (true)
        {
            if (nm.ConnectedClients.Count >= 2)
            {
                yield return new WaitForSeconds(waitTime);
                float x = Random.Range(-20.0f, 20.0f);
                float y = Random.Range(-4.5f, 4.5f);
                Vector3 position = new Vector3(x, y, 0.0f);
                string name = "Epoxy-" + System.Guid.NewGuid().ToString();
                RequestSpawnEpoxyClientRpc(position, name);
            }

        }
    }
    IEnumerator SpawnBucketAtRate(float waitTime)
    {
        while (true)
        {
            if (nm.ConnectedClients.Count >= 2)
            {
                yield return new WaitForSeconds(waitTime);
                float x = Random.Range(-20.0f, 20.0f);
                float y = Random.Range(-4.5f, 4.5f);
                Vector3 position = new Vector3(x, y, 0.0f);
                string name = "Bucket-" + System.Guid.NewGuid().ToString();
                RequestSpawnBucketClientRpc(position, name);
            }

        }
    }

    IEnumerator SpawnFakeDrinkAtRate(float waitTime)
    {
        while (true)
        {
            if (nm.ConnectedClients.Count >= 2)
            {
                yield return new WaitForSeconds(waitTime);
                float x = Random.Range(-20.0f, 20.0f);
                float y = Random.Range(-4.5f, 4.5f);
                Vector3 position = new Vector3(x, y, 0.0f);
                string name = "FakeDrink-" + System.Guid.NewGuid().ToString();
                RequestSpawnFakeDrinkClientRpc(position, name);
            }

        }
    }

    IEnumerator SpawnGlassesAtRate(float waitTime)
    {
        while (true)
        {
            if (nm.ConnectedClients.Count >= 2)
            {
                yield return new WaitForSeconds(waitTime);
                float x = Random.Range(-20.0f, 20.0f);
                float y = Random.Range(-4.5f, 4.5f);
                Vector3 position = new Vector3(x, y, 0.0f);
                string name = "Glasses-" + System.Guid.NewGuid().ToString();
                RequestSpawnGlassesClientRpc(position, name);
            }

        }
    }
    void Start()
    {
        
        nm.OnClientConnectedCallback += Nm_OnClientConnectedCallback;
        if (nm.IsServer)
        {
            Debug.Log("NetSpawner Started, IsServer");
        }

        if (nm.IsClient)
        {
            Debug.Log("NetSpawner Started, IsClient");
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    private void Nm_OnClientConnectedCallback(ulong obj)
    {
        if (IsServer)
        {

            Debug.Log("Client Connected -- Server");
            if (nm.ConnectedClientsList.Count == 2)
            {
                if (gameStarted == false)
                {
                    StartCoroutine(SpawnDrinksAtRate(4.0f));
                    StartCoroutine(SpawnEpoxyAtRate(Random.Range(15.0f, 20.0f)));
                    StartCoroutine(SpawnBucketAtRate(Random.Range(15.0f, 20.0f)));
                    StartCoroutine(SpawnFakeDrinkAtRate(Random.Range(15.0f, 20.0f)));
                    StartCoroutine(SpawnGlassesAtRate(Random.Range(15.0f, 20.0f)));
                    gameStarted = true;
                }
            }

        }
        if (IsClient)
        {

            Debug.Log("Client Connected -- Client");
        }
    }
    //Drink Spawning
    void SpawnDrink(Vector3 pos, string name)
    {
        drinkPrefab.name = name;
        var drink = Instantiate(drinkPrefab, pos, Quaternion.identity);
    }

    [ClientRpc]
    void RequestSpawnDrinksClientRpc(Vector3 pos, string name)
    {
        SpawnDrink(pos, name);
    }
    //Epoxy Spawning
    void SpawnEpoxy(Vector3 pos, string name)
    {
        epoxyPrefab.name = name;
        var epoxy = Instantiate(epoxyPrefab, pos, Quaternion.identity);
    }

    [ClientRpc]
    void RequestSpawnEpoxyClientRpc(Vector3 pos, string name)
    {
        SpawnEpoxy(pos, name);
    }
    //Bucket Spawning
    void SpawnBucket(Vector3 pos, string name)
    {
        bucketPrefab.name = name;
        var bucket = Instantiate(bucketPrefab, pos, Quaternion.identity);
    }

    [ClientRpc]
    void RequestSpawnBucketClientRpc(Vector3 pos, string name)
    {
        SpawnBucket(pos, name);
    }

    void SpawnFakeDrink(Vector3 pos, string name)
    {
        fakeDrinkPrefab.name = name;
        var fakeDrink = Instantiate(fakeDrinkPrefab, pos, Quaternion.identity);
    }

    [ClientRpc]
    void RequestSpawnFakeDrinkClientRpc(Vector3 pos, string name)
    {
        SpawnFakeDrink(pos, name);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    void SpawnGlasses(Vector3 pos, string name)
    {
        glassesPrefab.name = name;
        var glasses = Instantiate(glassesPrefab, pos, Quaternion.identity);
    }

    [ClientRpc]
    void RequestSpawnGlassesClientRpc(Vector3 pos, string name)
    {
        SpawnGlasses(pos, name);
    }


    [ServerRpc(RequireOwnership = false)]
    public void RequestSpawnFakeDrinkChildServerRpc(Vector3 pos, ServerRpcParams serverRpcParams =default)
    {
        string name = "FakeDrinkChild-" + System.Guid.NewGuid().ToString();
        ulong playerNum = serverRpcParams.Receive.SenderClientId;

        if (playerNum == 0)
        {
            gameState.fakeDrinkActive1 = false;
        }
        else gameState.fakeDrinkActive2 = false;

        PlayerNetworkData boolData = new PlayerNetworkData()
        {
            fakeDrink1 = gameState.fakeDrinkActive1,
            fakeDrink2 = gameState.fakeDrinkActive2,
        };
        RequestSpawnFakeDrinkChildClientRpc(pos, name, boolData);

    }

    [ClientRpc]
    void RequestSpawnFakeDrinkChildClientRpc(Vector3 pos, string name, PlayerNetworkData boolData)
    {
        fakeDrinkChildPrefab.name = name;
        Instantiate(fakeDrinkChildPrefab, pos, Quaternion.identity);
        this.gameState.fakeDrinkActive1 = boolData.fakeDrink1;
        this.gameState.fakeDrinkActive2 = boolData.fakeDrink2;
    }
}
