using UnityEngine;

public class ShovelController : MonoBehaviour
{
    public GameObject highlight;
    public float returnSpeed = 1.0f;
    public bool isSelected { get { return selected; } }
    public Player selector { get { return whoSelected; } }

    private SpriteRenderer shovelRenderer;
    private Rigidbody2D rigidbody2d;
    private Vector3 startPos;
    private Player whoSelected;
    private bool returnToStart;
    private bool selected;

    private void Awake()
    {
        highlight.SetActive(false);
        rigidbody2d = GetComponent<Rigidbody2D>();
        shovelRenderer = GetComponent<SpriteRenderer>();

        selected = false;
        returnToStart = false;
        startPos = transform.position;
    }

    private void Update()
    {
        if (returnToStart)
        {
            // Mueve fruto hasta que regrese a la posición inicial.
            float step = returnSpeed * Time.deltaTime;
            Vector2 newPosition = Vector2.MoveTowards(transform.position, startPos, step);
            rigidbody2d.MovePosition(newPosition);

            if (transform.position == startPos)
                returnToStart = false;
        }
        else
        {
            // Asigna posición inicial en caso de que no haya sido asignada anteriormente.
            if (!selected && startPos != transform.position)
                startPos = transform.position;
        }
    }

    public void Select(Player who)
    {
        whoSelected = who;
        selected = true;
        highlight.SetActive(false);
        shovelRenderer.sortingOrder += 1;
    }

    public void Deselect()
    {
        selected = false;
        returnToStart = true;
        highlight.SetActive(true);
        shovelRenderer.sortingOrder -= 1;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isSelected)
        {
            // Activa el highlight en caso de que el cursor entre en contacto con la pala.
            if (other.tag == "CursorA" || other.tag == "CursorB")
            {
                highlight.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "CursorA" || other.tag == "CursorB")
            highlight.SetActive(false);
    }
}