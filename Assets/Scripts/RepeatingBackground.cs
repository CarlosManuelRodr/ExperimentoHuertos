using UnityEngine;

public class RepeatingBackground : MonoBehaviour
{

    private BoxCollider2D groundCollider;
    private float groundHorizontalLength;
    private float speed;

    private void Start()
    {
        groundCollider = GetComponent<BoxCollider2D>();
        groundHorizontalLength = groundCollider.size.x;
        speed = this.transform.parent.GetComponent<MovingBackground>().speed;
    }

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);

        if (transform.localPosition.x < -groundHorizontalLength)
        {
            RepositionBackground();
        }
    }

    //Moves the object this script is attached to right in order to create our looping background effect.
    private void RepositionBackground()
    {
        Vector2 groundOffSet = new Vector2((groundHorizontalLength * 2f) - 0.02f, 0);
        transform.localPosition = (Vector2)transform.localPosition + groundOffSet;
    }
}