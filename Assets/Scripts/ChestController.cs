using UnityEngine;
using UnityEngine.UI;

public class ChestController : MonoBehaviour
{
    public bool dummy = false;
    public uint score = 0;
    public GameObject chestVisuals;

    private ChestVisuals chestVisualsScript;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
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
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (m_capture && (other.tag == "ItemA" || other.tag == "ItemB"))
        {
            if (other.GetComponent<FruitController>().isFalling)
            {
                chestVisualsScript.SetCaptured(other.tag);
                Destroy(other.gameObject);
                audioSource.Play();
                score++;
                this.SetScore(score);
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
