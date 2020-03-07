using UnityEngine;

public class ShovelController : MonoBehaviour
{
    public Material greyscaleMaterial;
    public Material defaultMaterial;

    public bool isSelected { get { return selected; } }
    public Player selector { get { return whoSelected; } }

    private SpriteRenderer shovelRenderer;
    private Player whoSelected;
    private bool selected;
    private bool active = true;

    private void Awake()
    {
        shovelRenderer = GetComponent<SpriteRenderer>();
        selected = false;
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
            shovelRenderer.sortingOrder -= 1;
        }
    }
}