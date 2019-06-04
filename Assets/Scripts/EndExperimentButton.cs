using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum ButtonStatus
{
    Small,
    Large
}

public class EndExperimentButton : MonoBehaviour
{
    public GameObject gameManager;
    private GameManager gameManagerScript;
    private GameObject collision = null;
    private ButtonStatus status = ButtonStatus.Small;
    private Image image;

    void Start()
    {
        gameManagerScript = gameManager.GetComponent<GameManager>();
        image = GetComponent<Image>();
    }

    public void Restore()
    {
        if (status == ButtonStatus.Large)
        {
            image.color = Color.white;
            this.transform.localScale /= 1.1f;
            status = ButtonStatus.Small;
        }
    }

    void Update()
    {
        if (collision != null)
        {
            if (Input.GetMouseButtonDown(0) && (collision.tag == "CursorA" || collision.tag == "CursorB"))
            {
                Restore();
                gameManagerScript.EndExperiment();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        collision = other.gameObject;
        if (collision.tag == "CursorA" || collision.tag == "CursorB")
        {
            if (status == ButtonStatus.Small)
            {
                image.color = Color.green;
                this.transform.localScale *= 1.1f;
                status = ButtonStatus.Large;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "CursorA" || other.gameObject.tag == "CursorB")
        {
            if (status == ButtonStatus.Large)
            {
                image.color = Color.white;
                this.transform.localScale /= 1.1f;
                status = ButtonStatus.Small;
            }
        }
        collision = null;
    }
}
