using UnityEngine;

public class CursorController : MonoBehaviour
{
    public Sprite handOpen, handClosed;
    public AudioClip grab, release;
    public bool isSelecting { get { return selecting; } }

    private bool selecting;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2d;

    private GameObject selected;
    private Vector3 mousePosition;
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        Cursor.visible = false;
        selecting = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CloseHand();
        if (Input.GetMouseButtonUp(0))
            OpenHand();

        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rigidbody2d.MovePosition(mousePosition);
        if (isSelecting && selected != null)
            selected.GetComponent<Rigidbody2D>().MovePosition(mousePosition);
    }

    public void CloseHand()
    {
        if (selected != null)
        {
            audioSource.PlayOneShot(grab);
            selected.GetComponent<FruitController>().Select();
            selecting = true;
        }

        spriteRenderer.sprite = handClosed;
    }

    public void OpenHand()
    {
        if (selected != null)
        {
            audioSource.PlayOneShot(release);
            selected.GetComponent<FruitController>().Deselect();
        }

        selecting = false;
        spriteRenderer.sprite = handOpen;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isSelecting && other.tag == "ItemA")
        {
            selected = other.gameObject;
        }

        if (isSelecting && other.tag == "Chest")
        {
            other.transform.parent.GetComponentInChildren<ChestController>().SetToCapture(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!isSelecting)
            selected = null;

        if (isSelecting && other.tag == "Chest")
        {
            other.transform.parent.GetComponentInChildren<ChestController>().SetToCapture(false);
        }
    }
}
