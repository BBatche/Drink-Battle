using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu( fileName = "gameState", menuName = "State/NetworkGameState")]
public class GameState : ScriptableObject
{
    public int player1Score = 0;

    public int player2Score = 0;

    public float player1MoveSpeed = 5.0f;

    public float player2MoveSpeed = 5.0f;

    public bool bucketActive = false;

    public bool fakeDrinkActive1 = false;

    public bool fakeDrinkActive2 = false;



}
