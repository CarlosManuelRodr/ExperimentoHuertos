using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum LockStatus
{
    Locked,
    Unlocked
}

public class LockController : MonoBehaviour
{
    public GameObject myCursor, enemyCursor;
    public Sprite locked, unlocked;

    private ManyCursorController enemyCursorController;
    private GameObject collision = null;
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

    void Update()
    {
        if (collision != null)
        {
            if (Input.GetMouseButtonDown(0) && collision.tag == myCursor.tag)
            {
                enemyCursorController.SelectableSwitch();

                if (lockstatus == LockStatus.Locked)
                {
                    spriteRenderer.sprite = unlocked;
                    lockstatus = LockStatus.Unlocked;
                    text.text = "Parcela\ndesbloqueada";
                }
                else
                {
                    if (lockstatus == LockStatus.Unlocked)
                    {
                        spriteRenderer.sprite = locked;
                        lockstatus = LockStatus.Locked;
                        text.text = "Parcela\nbloqueada";
                    }
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        collision = other.gameObject;
        if (collision.tag == myCursor.tag)
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
        collision = null;
    }
}
