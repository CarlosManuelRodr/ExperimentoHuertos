using UnityEngine;
using UnityEngine.UI;

public enum CanCapture
{
    PLAYERA,
    PLAYERB,
    BOTH
}

public class ChestController : MonoBehaviour
{
    public CanCapture canCapture = CanCapture.BOTH;
    public bool dummy = false;
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
        if (!dummy)
            this.transform.parent.GetComponentInChildren<Text>().text = "Puntos: " + score;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (m_capture)
        {
            if (
                (canCapture == CanCapture.BOTH && (other.tag == "ItemA" || other.tag == "ItemB")) ||
                (canCapture == CanCapture.PLAYERA && other.tag == "ItemA") ||
                (canCapture == CanCapture.PLAYERB && other.tag == "ItemB")
               )
            {
                Destroy(other.gameObject);
                audioSource.Play();
                m_capture = false;
                spriteRenderer.enabled = false;
                score++;
                this.SetScore(score);
            }
        }
    }

    public void SetScore(uint newScore)
    {
        score = newScore;
        if (!dummy)
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
