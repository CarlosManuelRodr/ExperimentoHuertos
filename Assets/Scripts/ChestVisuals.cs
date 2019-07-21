using UnityEngine;

/// <summary>
/// Administra las animaciones relativas al cambio de estado del cofre (abierto/cerrado).
/// </summary>
public class ChestVisuals : MonoBehaviour
{
    public Sprite chestClosed, chestOpen;
    public GameObject chestController;

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private ChestController chestControllerScript;

    private bool fruitAInside, fruitBInside;
    private bool chestIsOpen;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        chestControllerScript = chestController.GetComponent<ChestController>();

        fruitAInside = false;
        fruitBInside = false;
        chestIsOpen = false;
    }

    void Update()
    {
        if (!chestIsOpen && (fruitAInside || fruitBInside))
        {
            spriteRenderer.sprite = chestOpen;
            audioSource.Play();
            chestIsOpen = true;
        }
        if (chestIsOpen && !fruitAInside && !fruitBInside)
        {
            spriteRenderer.sprite = chestClosed;
            chestIsOpen = false;
        }
    }

    public void SetCaptured(string tag)
    {
        if (tag == "ItemA")
            fruitAInside = false;
        if (tag == "ItemB")
            fruitBInside = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "ItemA" && other.GetComponent<FruitController>().isSelected)
        {
            fruitAInside = true;
            chestControllerScript.SetToCapture(true);
        }

        if (other.tag == "ItemB" && other.GetComponent<FruitController>().isSelected)
        {
            fruitBInside = true;
            chestControllerScript.SetToCapture(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "ItemA" && other.GetComponent<FruitController>().isSelected)
        {
            fruitAInside = false;
            if (!fruitBInside)
                chestControllerScript.SetToCapture(false);
        }

        if (other.tag == "ItemB" && other.GetComponent<FruitController>().isSelected)
        {
            fruitBInside = false;
            if (!fruitAInside)
                chestControllerScript.SetToCapture(false);
        }
    }
}
