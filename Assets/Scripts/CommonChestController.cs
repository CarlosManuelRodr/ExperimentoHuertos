using UnityEngine;
using UnityEngine.UI;

public class CommonChestController : MonoBehaviour
{
    public bool actAsCounter = false;
    public GameObject chestA, chestB;

    private ChestController chestAScript, chestBScript;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool m_capture;
    private uint myScore, globalScore;

    void Awake()
    {
        chestAScript = chestA.GetComponentInChildren<ChestController>();
        chestBScript = chestB.GetComponentInChildren<ChestController>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        m_capture = false;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (m_capture && (other.tag == "ItemA" || other.tag == "ItemB"))
        {
            Destroy(other.gameObject);
            audioSource.Play();
            m_capture = false;
            spriteRenderer.enabled = false;
            myScore++;
            this.SetScore(globalScore + myScore);
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

    public void SetToCapture(bool capture)
    {
        m_capture = capture;
        spriteRenderer.enabled = capture;
    }
}
