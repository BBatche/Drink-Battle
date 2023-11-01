using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode;
using Unity.Networking;

public class NetSpawnerScript : NetworkBehaviour
{
    public GameObject drinkPrefab;
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
                float y = Random.Range(-7.0f, 7.0f);
                Vector3 position = new Vector3(x, y, 0.0f);
                string name = "drink-" + System.Guid.NewGuid().ToString();
                RequestSpawnDrinksClientRpc(position, name);
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
                    gameStarted= true;
                }
            }
            
        }
        if (IsClient)
        {
            Debug.Log("Client Connecteed -- Client");
        }
    }
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
