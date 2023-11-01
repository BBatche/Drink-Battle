using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DrinkCollectorScript : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
        
    }
    [ServerRpc(RequireOwnership = false)]
    private void RequestCollectDrinkServerRpc(string tag)
    {
        //Server can refer to master list in spawner to see if tag and location are valid
        //Client could pass in location of Drink

        //if valid update score
        RequestDestroyDrinkClientRpc(tag);

        //else ban cheater
    }

    [ClientRpc]
    void RequestDestroyDrinkClientRpc(string name)
    {
        GameObject temp;
        temp = GameObject.Find(name);
        Destroy(temp);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Drink")
        {
            Debug.Log("Collided with Drink");
            RequestCollectDrinkServerRpc(collision.gameObject.name);
        }
    }
}
