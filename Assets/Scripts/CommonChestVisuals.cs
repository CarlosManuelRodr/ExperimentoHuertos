using UnityEngine;

/// <summary>
/// Administra las animaciones relativas al cambio de estado del cofre común (abierto/cerrado).
/// </summary>
public class CommonChestVisuals : MonoBehaviour
{
    public bool canCapture = true;
    public Sprite chestClosed, chestOpen;
    public GameObject chestController;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private bool chestIsOpen;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        chestIsOpen = false;
    }

    public void OpenChest()
    {
        if (!chestIsOpen)
        {
            spriteRenderer.sprite = chestOpen;
            audioSource.Play();
            chestIsOpen = true;
        }
    }

    public void CloseChest()
    {
        if (chestIsOpen)
        {
            spriteRenderer.sprite = chestClosed;
            chestIsOpen = false;
        }
    }
}