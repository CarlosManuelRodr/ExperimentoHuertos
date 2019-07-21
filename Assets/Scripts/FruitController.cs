using System;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    public GameObject highlight;
    public float returnSpeed = 1.0f;
    public bool isSelected { get { return selected; } }
    public bool isFalling { get { return falling; } }
    public bool isHighlighted { get { return highlight.activeSelf; } }
    public float fruitLogInterval = 0.2f;

    private SpriteRenderer fruitRenderer, highlightRenderer = null;
    private Rigidbody2D rigidbody2d;
    private FruitLogger fruitLogger;
    private Camera cam;
    private Color red, yellow, green;
    private Vector3 startPos;
    private bool inChest;
    private bool returnToStart;
    private bool selected;
    private bool falling;
    private float nextUpdate;
    private string selector = "";

    void Awake()
    {
        highlight.SetActive(false);
        highlightRenderer = highlight.GetComponent<SpriteRenderer>();
        fruitRenderer = GetComponent<SpriteRenderer>();
        fruitLogger = GetComponent<FruitLogger>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        cam = Camera.main;

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
        falling = false;
        nextUpdate = fruitLogInterval;
    }

    private void OnDisable()
    {
        if (fruitLogger != null)
            fruitLogger.Save();
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

        if (Time.time >= nextUpdate)
        {
            nextUpdate = Time.time + fruitLogInterval;
            if (startPos != transform.position && fruitLogger != null)
            {
                fruitLogger.Log(cam.WorldToScreenPoint(transform.position), selector);
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

                selector = other.tag;
            }

            if (other.tag == "AICursor")
            {
                EnemyAi cursor = other.gameObject.GetComponent<EnemyAi>();
                if (!cursor.isSelecting)
                    highlight.SetActive(true);
            }

            if (other.tag == "Chest" || other.tag == "CommonChest")
            {
                highlightRenderer.color = green;
                inChest = true;
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

            if (other.tag == "Chest" || other.tag == "CommonChest")
            {
                inChest = false;

                if (returnToStart)
                    highlightRenderer.color = red;
                else
                    highlightRenderer.color = yellow;
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
                falling = true;
            }
            else
                returnToStart = true;
        }
    }
}
