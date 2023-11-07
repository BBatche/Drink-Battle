using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreScript : MonoBehaviour
{
    public GameState gameState;
    public TextMeshProUGUI score;
    public TextMeshProUGUI CheaterText;
    // Start is called before the first frame update
    void Start()
    {
        gameState.isCheating = false;
        CheaterText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        score.text ="Player 1: " + gameState.player1Score + "\t Player 2: " + gameState.player2Score;

        if (gameState.isCheating)
        {
            CheaterText.text = "CHEATER DETECTED";
            StartCoroutine(UpdateCheaterText(5.0f));
        }
    }
    IEnumerator UpdateCheaterText(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        gameState.isCheating = false;
        CheaterText.text = "";
    }
}
