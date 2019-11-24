using UnityEngine;
using UnityEngine.UI;

public class CommonCounter : MonoBehaviour
{
    public GameObject chestA, chestB;
    private ChestController chestAController, chestBController;
    private uint score = 0;
    private Text scoreLabel;

    void Start()
    {
        chestAController = chestA.GetComponentInChildren<ChestController>();
        chestBController = chestB.GetComponentInChildren<ChestController>();
        scoreLabel = GetComponentInChildren<Text>();
    }

    void Update()
    {
        uint totalScore = chestAController.score + chestBController.score;
        if (totalScore != score)
        {
            score = totalScore;
            scoreLabel.text = "Frutos: " + score.ToString();
        }
    }
}
