using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador para destruir el fruto recolectado y administrar puntuación en el cofre común.
/// </summary>
public class CommonChestController : MonoBehaviour
{
    public bool actAsCounter = false;
    public GameObject chestA, chestB;
    public GameObject commonChestVisuals;

    private ChestController chestAScript, chestBScript;
    private SpriteRenderer spriteRenderer;
    private uint myScore, globalScore;

    void Awake()
    {
        chestAScript = chestA.GetComponentInChildren<ChestController>();
        chestBScript = chestB.GetComponentInChildren<ChestController>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        myScore = 0;
        globalScore = 0;
    }

    void Update()
    {
        if (actAsCounter)
        {
            uint aScore = chestAScript.GetScore();
            uint bScore = chestBScript.GetScore();
            if (aScore + bScore != globalScore)
            {
                globalScore = aScore + bScore;
                SetScore(globalScore + myScore);
            }
        }
    }

    public void SetScore(uint newScore)
    {
        this.transform.parent.GetComponentInChildren<Text>().text = "Frutos: " + newScore;
    }

    public void ResetScore()
    {
        myScore = 0;
    }

    public uint GetScore()
    {
        return myScore;
    }
}
