using UnityEngine;

/// <summary>
/// Administra las animaciones relativas al cambio de estado del cofre (abierto/cerrado).
/// </summary>
public class ChestVisuals : MonoBehaviour
{
    public Sprite chestClosed, chestOpen;
    public GameObject chestController;
    public Player owner = Player.PlayerA;
    public CanInteract chestAccess = CanInteract.Both;
    public uint numberOfOccupants
    {
        get { return occupants; }
        set { occupants = value; }
    }

    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private ChestController chestControllerScript;
    private uint occupants;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        chestControllerScript = chestController.GetComponent<ChestController>();
        occupants = 0;
    }

    public void SetCaptured()
    {
        spriteRenderer.sprite = chestClosed;
        chestControllerScript.SetToCapture(false);
    }

    void SetChestToCapture(string itemTag)
    {
        spriteRenderer.sprite = chestOpen;
        audioSource.Play();

        if (itemTag == "ItemA")
            chestControllerScript.SetToCapture(true);

        if (itemTag == "ItemB")
            chestControllerScript.SetToCapture(true);
    }

    void DisableCapture()
    {
        spriteRenderer.sprite = chestClosed;
        chestControllerScript.SetToCapture(false);
    }

    public void EnterFruit(FruitController fruit)
    {
        if (chestAccess == CanInteract.Both)
            SetChestToCapture(fruit.tag);
        if (chestAccess == CanInteract.PlayerA && fruit.selector == Player.PlayerA)
            SetChestToCapture(fruit.tag);
        if (chestAccess == CanInteract.PlayerB && fruit.selector == Player.PlayerB)
            SetChestToCapture(fruit.tag);
    }

    public void ExitFruit()
    {
        this.DisableCapture();
    }

    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.tag == "ItemA" || other.tag == "ItemB")
    //    {
    //        occupants++;
    //        if (occupants <= 1)
    //        {
    //            FruitController fruit = other.GetComponent<FruitController>();

    //            if (fruit.isSelected)
    //            {
    //                if (chestAccess == CanInteract.Both)
    //                    SetChestToCapture(other.tag);
    //                if (chestAccess == CanInteract.PlayerA && fruit.selector == Player.PlayerA)
    //                    SetChestToCapture(other.tag);
    //                if (chestAccess == CanInteract.PlayerB && fruit.selector == Player.PlayerB)
    //                    SetChestToCapture(other.tag);
    //            }
    //        }
    //    }
    //}

    //void OnTriggerExit2D(Collider2D other)
    //{
    //    FruitController fruit = other.GetComponent<FruitController>();
    //    if (other.tag == "ItemA" || other.tag == "ItemB")
    //    {
    //        if (occupants <= 1)
    //        {
    //            if (fruit.isSelected)
    //                this.DisableCapture();
    //        }
    //        occupants--;
    //    }
    //}
}
