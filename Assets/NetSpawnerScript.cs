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
    public NetworkManager nm;
    
    private bool gameStarted = false;
    

    IEnumerator SpawnDrinksAtRate(float waitTime)
    {
        while (true)
        {
            if(nm.ConnectedClients.Count >= 2) 
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


    void Start()
    {
        nm.OnClientConnectedCallback += Nm_OnClientConnectedCallback;
        if(nm.IsServer)
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
            if(nm.ConnectedClientsList.Count == 2) 
            {
                if(gameStarted == false)
                {
                    StartCoroutine(SpawnDrinksAtRate(4.0f));
                    StartCoroutine(SpawnEpoxyAtRate(Random.Range(15.0f, 20.0f)));
                    StartCoroutine(SpawnBucketAtRate(Random.Range(15.0f, 20.0f)));
                    gameStarted= true;
                }
            }
            
        }
        if (IsClient)
        {
            
            Debug.Log("Client Connected -- Client");
        }
    }
    //Drink Spawning
    void SpawnDrink(Vector3 pos , string name)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
