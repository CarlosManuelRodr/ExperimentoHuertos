using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador para destruir el fruto recolectado y administrar puntuación.
/// </summary>
public class ChestController : MonoBehaviour
{
    public bool dummy = false;
    public uint score = 0;
    public GameObject chestVisuals;
    public GameObject experiment;
    public Player owner = Player.PlayerA;

    private ChestVisuals chestVisualsScript;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private ExperimentLogger experimentLogger;
    private ExperimentManager experimentManager;
    private bool m_capture;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        chestVisualsScript = chestVisuals.GetComponent<ChestVisuals>();

        m_capture = false;
        spriteRenderer.enabled = false;
    }

    void Start()
    {
        if (!dummy)
            this.transform.parent.GetComponentInChildren<Text>().text = "Frutos: " + score;

        if (experiment != null)
        {
            experimentManager = experiment.GetComponent<ExperimentManager>();
            experimentLogger = experiment.GetComponent<ExperimentLogger>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (m_capture && (other.tag == "ItemA" || other.tag == "ItemB"))
        {
            FruitController fruit = other.GetComponent<FruitController>();
            if (fruit.isFalling)
            {
                chestVisualsScript.SetCaptured();
                Destroy(other.gameObject);
                audioSource.Play();
                score++;
                this.SetScore(score);

                if (experiment != null)
                {
                    if (fruit.selector == Player.PlayerA)
                        experimentManager.harvestedA++;
                    else
                        experimentManager.harvestedB++;

                    string player = (fruit.selector == Player.PlayerA) ? "A" : "B";
                    string fruitFrom = (other.tag == "ItemA") ? "A" : "B";
                    string chestOwner = (owner == Player.PlayerA) ? "A" : "B";
                    experimentLogger.Log(player + " deposita fruto de " + fruitFrom + " en cesto " + chestOwner);
                }
            }
        }
    }

    public void SetScore(uint newScore)
    {
        score = newScore;
        if (!dummy)
            this.transform.parent.GetComponentInChildren<Text>().text = "Frutos: " + score;
    }

    public uint GetScore()
    {
        return score;
    }

    public void SetToCapture(bool capture)
    {
        m_capture = capture;
        spriteRenderer.enabled = capture;
    }
}
