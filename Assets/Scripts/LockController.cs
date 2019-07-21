using UnityEngine;
using UnityEngine.UI;

enum LockStatus
{
    Locked,
    Unlocked
}

/// <summary>
/// Controlador de candado de huertos.
/// </summary>
public class LockController : MonoBehaviour
{
    public GameObject myCursor, enemyCursor;
    public Sprite locked, unlocked;

    private ManyCursorController enemyCursorController;
    private SpriteRenderer spriteRenderer;
    private ButtonStatus status = ButtonStatus.Small;
    private LockStatus lockstatus = LockStatus.Locked;
    private Text text;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCursorController = enemyCursor.GetComponent<ManyCursorController>();
        text = GetComponentInChildren<Text>();
    }

    void OnDisable()
    {
        spriteRenderer.sprite = locked;
        lockstatus = LockStatus.Locked;
        text.text = "Parcela\nbloqueada";

        if (status == ButtonStatus.Large)
        {
            this.transform.localScale /= 1.1f;
            status = ButtonStatus.Small;
        }
    }

    public void LockSwitch(string caller)
    {
        if (lockstatus == LockStatus.Unlocked && caller == myCursor.tag)
        {
            spriteRenderer.sprite = locked;
            lockstatus = LockStatus.Locked;
            text.text = "Parcela\nbloqueada";
            enemyCursorController.SelectableSwitch();
        }
        else if (lockstatus == LockStatus.Locked && caller == myCursor.tag)
        {
            spriteRenderer.sprite = unlocked;
            lockstatus = LockStatus.Unlocked;
            text.text = "Parcela\ndesbloqueada";
            enemyCursorController.SelectableSwitch();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == myCursor.tag)
        {
            if (status == ButtonStatus.Small)
            {
                this.transform.localScale *= 1.1f;
                status = ButtonStatus.Large;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == myCursor.tag)
        {
            if (status == ButtonStatus.Large)
            {
                this.transform.localScale /= 1.1f;
                status = ButtonStatus.Small;
            }
        }
    }
}
