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

    public List<Drink> drinkList = new();
    public List<Bucket> bucketList = new();
    public List<Epoxy> epoxyList = new();
    public List<Troll> trollList = new();
    public List<Glasses> glassesList = new();
    public List<FakeDrink> fakeDrinkList = new();
    public struct Drink
    {
        public string name;
        public Vector3 pos;
        public Drink(string name, Vector3 pos)
        {
            this.name = name;
            this.pos = pos;
        }
    }
    public struct Epoxy
    {
        public string name;
        public Vector3 pos;
        public Epoxy(string name, Vector3 pos)
        {
            this.name = name;
            this.pos = pos;
        }
    }
    public struct Troll
    {
        public string name;
        public Vector3 pos;
        public Troll(string name, Vector3 pos)
        {
            this.name = name;
            this.pos = pos;
        }
    }
    public struct FakeDrink
    {
        public string name;
        public Vector3 pos;
        public FakeDrink(string name, Vector3 pos)
        {
            this.name = name;
            this.pos = pos;
        }
    }
    public struct Glasses
    {
        public string name;
        public Vector3 pos;
        public Glasses(string name, Vector3 pos)
        {
            this.name = name;
            this.pos = pos;
        }
    }

    public struct Bucket
    {
        public string name;
        public Vector3 pos;
        public Bucket(string name, Vector3 pos)
        {
            this.name = name;
            this.pos = pos;
        }
    }
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

    IEnumerator SpawnTrollAtRate(float waitTime)
    {
        while (true)
        {
            if (nm.ConnectedClients.Count >= 2)
            {
                yield return new WaitForSeconds(waitTime);
                float x = Random.Range(-20.0f, 20.0f);
                float y = Random.Range(-4.5f, 4.5f);
                Vector3 position = new Vector3(x, y, 0.0f);
                string name = "Troll-" + System.Guid.NewGuid().ToString();
                RequestSpawnTrollClientRpc(position, name);
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
                    StartCoroutine(SpawnTrollAtRate(Random.Range(15.0f, 20.0f)));
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
        Drink drinky = new Drink(name, pos);
        drinkList.Add(drinky);
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
        Epoxy epoxyy = new Epoxy(name, pos);
        epoxyList.Add(epoxyy);
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
        Bucket buckett = new Bucket(name, pos);
        bucketList.Add(buckett);
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
        FakeDrink fakeDrinkk = new FakeDrink(name, pos);
        fakeDrinkList.Add(fakeDrinkk);
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
        Glasses glassess = new Glasses(name, pos);
        glassesList.Add(glassess);
    }

    [ClientRpc]
    void RequestSpawnGlassesClientRpc(Vector3 pos, string name)
    {
        SpawnGlasses(pos, name);
    }

    void SpawnTroll(Vector3 pos, string name)
    {
        trollPrefab.name = name;
        var troll = Instantiate(trollPrefab, pos, Quaternion.identity);
        Troll trolll = new Troll(name, pos);
        trollList.Add(trolll);
    }

    [ClientRpc]
    void RequestSpawnTrollClientRpc(Vector3 pos, string name)
    {
        SpawnTroll(pos, name);
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

    public bool searchDrinkList(string name, Vector3 pos)
    {
        name = name.Replace("(Clone)", "");
        foreach(Drink drink in drinkList)
        {
            if(drink.name == name && drink.pos == pos)
            {
                Debug.Log("Drink Found");
                return true;
                
            }
            
        }
        return false;
    }
    public void removeDrinkfromList(string name)
    {   
        foreach(Drink drink in drinkList)
        {
            if (drink.name == name)
            {
                drinkList.Remove(drink);
                Debug.Log("Drink Removed");
            }
        }
        
    }
    public bool searchEpoxyList(string name, Vector3 pos)
    {
        name = name.Replace("(Clone)", "");
        foreach (Epoxy epoxy in epoxyList)
        {
            if (epoxy.name == name && epoxy.pos == pos)
            {
                Debug.Log("Epoxy Found");
                return true;

            }

        }
        return false;
    }
    public void removeEpoxyfromList(string name)
    {
        foreach (Epoxy epoxy in epoxyList)
        {
            if (epoxy.name == name)
            {
                epoxyList.Remove(epoxy);
                Debug.Log("Epoxy Removed");
            }
        }

    }

    public bool searchBucketList(string name, Vector3 pos)
    {
        name = name.Replace("(Clone)", "");
        foreach (Bucket bucket in bucketList)
        {
            if (bucket.name == name && bucket.pos == pos)
            {
                Debug.Log("Bucket Found");
                return true;

            }

        }
        return false;
    }
    public void removeBucketfromList(string name)
    {
        foreach (Bucket bucket in bucketList)
        {
            if (bucket.name == name)
            {
                bucketList.Remove(bucket);
                Debug.Log("Bucket Removed");
            }
        }

    }

    public bool searchFakeDrinkList(string name, Vector3 pos)
    {
        name = name.Replace("(Clone)", "");
        foreach (FakeDrink fakeDrink in fakeDrinkList)
        {
            if (fakeDrink.name == name && fakeDrink.pos == pos)
            {
                Debug.Log("Fake Drink Found");
                return true;

            }

        }
        return false;
    }
    public void removeFakeDrinkfromList(string name)
    {
        foreach (FakeDrink fakeDrink in fakeDrinkList)
        {
            if (fakeDrink.name == name)
            {
                fakeDrinkList.Remove(fakeDrink);
                Debug.Log("Fake Drink Removed");
            }
        }

    }
    public bool searchGlassesList(string name, Vector3 pos)
    {
        name = name.Replace("(Clone)", "");
        foreach (Glasses glasses in glassesList)
        {
            if (glasses.name == name && glasses.pos == pos)
            {
                Debug.Log("Glasses Found");
                return true;

            }

        }
        return false;
    }
    public void removeGlassesfromList(string name)
    {
        foreach (Glasses glasses in glassesList)
        {
            if (glasses.name == name)
            {
                glassesList.Remove(glasses);
                Debug.Log("Glasses Removed");
            }
        }

    }
    public bool searchTrollList(string name, Vector3 pos)
    {
        name = name.Replace("(Clone)", "");
        foreach (Troll troll in trollList)
        {
            if (troll.name == name && troll.pos == pos)
            {
                Debug.Log("Troll Found");
                return true;

            }

        }
        return false;
    }
    public void removeTrollfromList(string name)
    {
        foreach (Troll troll in trollList)
        {
            if (troll.name == name)
            {
                trollList.Remove(troll);
                Debug.Log("Epoxy Removed");
            }
        }

    }
}
