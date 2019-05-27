using System.Collections;
using System.Collections.Generic;
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
        if ((other.tag == "CursorA" || other.tag == "CursorB") && other.GetComponent<ManyCursorController>().isSelecting)
        {
            spriteRenderer.sprite = chestOpen;
            audioSource.Play();
        }

        if (other.tag == "AICursor" && other.GetComponent<EnemyAi>().isSelecting)
        {
            spriteRenderer.sprite = chestOpen;
            audioSource.Play();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "CursorA" || other.tag == "CursorB" || other.tag == "AICursor")
        {
            spriteRenderer.sprite = chestClosed;
        }
    }
}
