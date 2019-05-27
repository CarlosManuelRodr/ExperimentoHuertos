using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    public GameObject highlight;
    public float returnSpeed = 1.0f;

    private SpriteRenderer fruitRenderer, highlightRenderer;
    private Rigidbody2D rigidbody2d;
    private Color red, yellow, green;
    private Vector3 startPos;
    private bool inChest;
    private bool returnToStart;
    private bool selecting;

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
        selecting = false;
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
            if (!selecting && startPos != transform.position)
            {
                startPos = transform.position;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
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
            highlightRenderer.color = green;
            inChest = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
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
        }
    }

    public void Select()
    {
        highlightRenderer.color = yellow;
        highlightRenderer.sortingOrder += 2;
        fruitRenderer.sortingOrder += 2;
        selecting = true;
    }

    public void Deselect()
    {
        highlightRenderer.color = red;
        highlightRenderer.sortingOrder -= 2;
        fruitRenderer.sortingOrder -= 2;
        selecting = false;

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
