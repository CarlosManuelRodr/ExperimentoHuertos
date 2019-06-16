using UnityEngine;

public class ChestVisuals : MonoBehaviour
{
    public Sprite chestClosed, chestOpen;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private CanCapture canCapture;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        canCapture = this.transform.parent.GetComponentInChildren<ChestController>().canCapture;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (
            (canCapture == CanCapture.BOTH && (other.tag == "ItemA" || other.tag == "ItemB")) ||
            (canCapture == CanCapture.PLAYERA && other.tag == "ItemA") ||
            (canCapture == CanCapture.PLAYERB && other.tag == "ItemB")
           )
        {
            if (other.GetComponent<FruitController>().isSelected)
            {
                spriteRenderer.sprite = chestOpen;
                audioSource.Play();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (
            (canCapture == CanCapture.BOTH && (other.tag == "ItemA" || other.tag == "ItemB")) ||
            (canCapture == CanCapture.PLAYERA && other.tag == "ItemA") ||
            (canCapture == CanCapture.PLAYERB && other.tag == "ItemB")
           )
        {
            spriteRenderer.sprite = chestClosed;
        }
    }
}
