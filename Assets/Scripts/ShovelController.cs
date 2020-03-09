using UnityEngine;

public class ShovelController : MonoBehaviour
{
    public Sprite shovelIcon, handIcon;
    public Material greyscaleMaterial;
    public Material defaultMaterial;
    public GameObject myCursor;

    public bool isActive
    {
        get { return active; }
    }

    private SpriteRenderer shovelRenderer;
    private ButtonStatus status = ButtonStatus.Small;
    private bool active = true;

    private void Awake()
    {
        shovelRenderer = GetComponent<SpriteRenderer>();
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

    public void ChangeMode(CursorMode mode)
    {
        shovelRenderer.sprite = (mode == CursorMode.HandMode) ? shovelIcon : handIcon;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (active)
        {
            if (other.gameObject.CompareTag(myCursor.tag))
            {
                if (status == ButtonStatus.Small)
                {
                    this.transform.localScale *= 1.1f;
                    status = ButtonStatus.Large;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (active)
        {
            if (other.gameObject.CompareTag(myCursor.tag))
            {
                if (status == ButtonStatus.Large)
                {
                    this.transform.localScale /= 1.1f;
                    status = ButtonStatus.Small;
                }
            }
        }
    }
}