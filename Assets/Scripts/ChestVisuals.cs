using UnityEngine;

public class ChestVisuals : MonoBehaviour
{
    public Sprite chestClosed, chestOpen;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.tag == "ItemA" || other.tag == "ItemB") && other.GetComponent<FruitController>().isSelected)
        {
            spriteRenderer.sprite = chestOpen;
            audioSource.Play();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "ItemA" || other.tag == "ItemB")
        {
            spriteRenderer.sprite = chestClosed;
        }
    }
}
