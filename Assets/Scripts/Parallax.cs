using UnityEngine;

/// <summary>
/// Gestor de movimiento de objetos que simula un fondo con perspectiva.
/// </summary>
public class Parallax : MonoBehaviour
{
    public GameObject back, middle, front;
    public float speed = 3.0f;
    public bool isMoving { get { return moveUp || moveDown; } }

    private Rigidbody2D rbBack, rbMiddle, rbFront;
    private bool moveUp = false, moveDown = false;
    private Vector2 backUpPos = new Vector2(-0.09f, -0.0741187f);
    private Vector2 middleUpPos = new Vector2(-6.66f, -4.429415f);
    private Vector2 frontUpPos = new Vector2(-0.06f, -2.63f);
    private Vector2 backDownPos = new Vector2(-0.09f, 1.875775f);
    private Vector2 middleDownPos = new Vector2(-6.66f, -0.3671355f);
    private Vector2 frontDownPos = new Vector2(-0.06f, 2.278452f);

    void Start()
    {
        rbBack = back.GetComponent<Rigidbody2D>();
        rbMiddle = middle.GetComponent<Rigidbody2D>();
        rbFront = front.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float step = speed * Time.deltaTime;

        if (moveUp)
        {
            rbFront.MovePosition(Vector2.MoveTowards(rbFront.position, frontUpPos, step));
            rbMiddle.MovePosition(Vector2.MoveTowards(rbMiddle.position, middleUpPos, step / 1.2f));
            rbBack.MovePosition(Vector2.MoveTowards(rbBack.position, backUpPos, step / 2.5f));

            if (
                (rbFront.position == frontUpPos) &&
                (rbMiddle.position == middleUpPos) &&
                (rbBack.position == backUpPos)
               )
            {
                moveUp = false;
            }
        }
        if (moveDown)
        {
            rbFront.MovePosition(Vector2.MoveTowards(rbFront.position, frontDownPos, step));
            rbMiddle.MovePosition(Vector2.MoveTowards(rbMiddle.position, middleDownPos, step / 1.2f));
            rbBack.MovePosition(Vector2.MoveTowards(rbBack.position, backDownPos, step / 2.5f));

            if (
                (rbFront.position == frontDownPos) &&
                (rbMiddle.position == middleDownPos) &&
                (rbBack.position == backDownPos)
               )
            {
                moveDown = false;
            }
        }
    }

    public void MoveUp()
    {
        if (!moveDown)
            moveUp = true;
    }

    public void MoveDown()
    {
        if (!moveUp)
            moveDown = true;
    }
}
