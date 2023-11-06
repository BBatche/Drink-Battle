using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Security.Cryptography.X509Certificates;

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

    public float player1cam = -21.0f;

    public float player2cam = -21.0f;

    public bool trollActive1 = false;

    public bool trollActive2 = false;

    public bool trollingPlayer1 = false;

    public bool trollingPlayer2 = false;



}
