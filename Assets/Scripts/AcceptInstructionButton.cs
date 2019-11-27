using UnityEngine;
using UnityEngine.UI;

public class AcceptInstructionButton : MonoBehaviour
{
    public Player owner = Player.PlayerA;
    public float scaleFactor = 1.1f;

    private GameManager gameManagerScript;
    private GameObject collision = null;
    private ButtonStatus status = ButtonStatus.Small;
    private Image image;
    private Color normal, highlight, acceptColor;
    private string ownerTag;
    bool accepted = false;


    void Start()
    {
        image = GetComponent<Image>();

        if (!ColorUtility.TryParseHtmlString("#333333", out normal))
            normal = Color.black;
        if (!ColorUtility.TryParseHtmlString("#00A201", out acceptColor))
            acceptColor = Color.green;
        highlight = Color.white;

        ownerTag = (owner == Player.PlayerA) ? "CursorA" : "CursorB";
    }

    public void Restore()
    {
        if (status == ButtonStatus.Large)
        {
            image.color = normal;
            this.transform.localScale /= scaleFactor;
            status = ButtonStatus.Small;
        }
        accepted = false;
    }

    void Update()
    {
        if (collision != null)
        {
            if (Input.GetMouseButtonDown(0) && collision.tag == ownerTag)
            {
                image.color = acceptColor;
                accepted = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!accepted)
        {
            collision = other.gameObject;
            if (collision.tag == ownerTag)
            {
                if (status == ButtonStatus.Small)
                {
                    image.color = highlight;
                    this.transform.localScale *= scaleFactor;
                    status = ButtonStatus.Large;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!accepted)
        {
            if (collision.tag == ownerTag)
            {
                if (status == ButtonStatus.Large)
                {
                    image.color = normal;
                    this.transform.localScale /= scaleFactor;
                    status = ButtonStatus.Small;
                }
            }
            collision = null;
        }
    }
}
