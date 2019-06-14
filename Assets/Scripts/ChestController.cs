using UnityEngine;
using UnityEngine.UI;

public class ChestController : MonoBehaviour
{
    public uint score = 0;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool m_capture;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        m_capture = false;
        spriteRenderer.enabled = false;
    }

    void Start()
    {
        this.transform.parent.GetComponentInChildren<Text>().text = "Puntos: " + score;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (m_capture && (other.tag == "ItemA" || other.tag == "ItemB"))
        {
            Destroy(other.gameObject);
            audioSource.Play();
            m_capture = false;
            spriteRenderer.enabled = false;
            score++;
            this.SetScore(score);
        }
    }

    public void SetScore(uint newScore)
    {
        score = newScore;
        this.transform.parent.GetComponentInChildren<Text>().text = "Puntos: " + score;
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
