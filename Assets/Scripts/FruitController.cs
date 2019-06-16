using UnityEngine;

public class FruitController : MonoBehaviour
{
    public GameObject highlight;
    public float returnSpeed = 1.0f;
    public bool isSelected { get { return selected; } }
    public bool isHighlighted { get { return highlight.activeSelf; } }

    private SpriteRenderer fruitRenderer, highlightRenderer = null;
    private Rigidbody2D rigidbody2d;
    private Color red, yellow, green;
    private Vector3 startPos;
    private bool inChest;
    private bool returnToStart;
    private bool selected;

    void Start()
    {
        highlight.SetActive(false);
        highlightRenderer = highlight.GetComponent<SpriteRenderer>();
        fruitRenderer = GetComponent<SpriteRenderer>();

        rigidbody2d = GetComponent<Rigidbody2D>();

        if (!ColorUtility.TryParseHtmlString("#FF170BB6", out red))
            red = Color.red;
        if (!ColorUtility.TryParseHtmlString("#FFCA0BB6", out yellow))
            yellow = Color.yellow;
        if (!ColorUtility.TryParseHtmlString("#84FF0BB6", out green))
            green = Color.green;

        startPos = transform.position;
        inChest = false;
        returnToStart = false;
        selected = false;
    }

    void Update()
    {
        if (returnToStart)
        {
            float step = returnSpeed * Time.deltaTime;
            Vector2 newPosition = Vector2.MoveTowards(transform.position, startPos, step);
            rigidbody2d.MovePosition(newPosition);


            if (transform.position == startPos)
            {
                returnToStart = false;
            }

        }
        else
        {
            if (!selected && startPos != transform.position)
            {
                startPos = transform.position;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (highlightRenderer != null)
        {
            if (other.tag == "CursorA" || other.tag == "CursorB")
            {
                ManyCursorController cursor = other.gameObject.GetComponent<ManyCursorController>();
                if (!cursor.isSelecting)
                    highlight.SetActive(true);
            }

            if (other.tag == "AICursor")
            {
                EnemyAi cursor = other.gameObject.GetComponent<EnemyAi>();
                if (!cursor.isSelecting)
                    highlight.SetActive(true);
            }

            if (other.tag == "Chest")
            {
                ChestController chestController = other.transform.parent.GetComponentInChildren<ChestController>();
                if (
                    (chestController.canCapture == CanCapture.BOTH) ||
                    (chestController.canCapture == CanCapture.PLAYERA && this.tag == "ItemA") ||
                    (chestController.canCapture == CanCapture.PLAYERB && this.tag == "ItemB")
                   )
                {

                    highlightRenderer.color = green;
                    inChest = true;

                    if (selected)
                        chestController.SetToCapture(true);
                }
            }

            if (other.tag == "CommonChest")
            {
                highlightRenderer.color = green;
                inChest = true;

                if (selected)
                    other.transform.parent.GetComponentInChildren<CommonChestController>().SetToCapture(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (highlightRenderer != null)
        {
            if (other.tag == "CursorA" || other.tag == "CursorB" || other.tag == "AICursor")
            {
                highlight.SetActive(false);
            }

            if (other.tag == "Chest")
            {
                inChest = false;

                if (returnToStart)
                    highlightRenderer.color = red;
                else
                    highlightRenderer.color = yellow;

                if (selected)
                    other.transform.parent.GetComponentInChildren<ChestController>().SetToCapture(false);
            }
        }
    }

    public void Select()
    {
        highlightRenderer.color = yellow;
        highlightRenderer.sortingOrder += 2;
        fruitRenderer.sortingOrder += 2;
        selected = true;
    }

    public void Deselect()
    {
        highlightRenderer.color = red;
        highlightRenderer.sortingOrder -= 2;
        fruitRenderer.sortingOrder -= 2;
        selected = false;

        if (transform.position != startPos)
        {
            if (inChest)
            {
                rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
            }
            else
                returnToStart = true;
        }
    }
}
