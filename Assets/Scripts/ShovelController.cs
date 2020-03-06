using UnityEngine;

public class ShovelController : MonoBehaviour
{
    public float returnSpeed = 1.0f;
    public Material greyscaleMaterial;
    public Material defaultMaterial;

    public bool isSelected { get { return selected; } }
    public Player selector { get { return whoSelected; } }

    private SpriteRenderer shovelRenderer;
    private Rigidbody2D rigidbody2d;
    private Vector3 startPos;
    private Player whoSelected;
    private bool returnToStart;
    private bool selected;
    private bool active = true;

    private void Awake()
    {
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

    public void MakeInactive()
    {
        active = false;
        shovelRenderer.material = greyscaleMaterial;
    }

    public void MakeActive()
    {
        active = true;
        shovelRenderer.material = defaultMaterial;
    }

    public void Select(Player who)
    {
        if (active)
        {
            whoSelected = who;
            selected = true;
            shovelRenderer.sortingOrder += 1;
        }
    }

    public void Deselect()
    {
        if (active)
        {
            selected = false;
            returnToStart = true;
            shovelRenderer.sortingOrder -= 1;
        }
    }
}