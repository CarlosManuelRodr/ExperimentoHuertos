using UnityEngine;
using UnityEngine.UI;

public enum LockStatus
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
    public Player owner = Player.PlayerA;
    public GameObject experiment;
    public Material greyscaleMaterial;
    public Material defaultMaterial;

    private ExperimentLogger experimentLogger;
    private ManyCursorController enemyCursorController;
    private SpriteRenderer spriteRenderer;
    private ButtonStatus status = ButtonStatus.Small;
    private LockStatus lockstatus = LockStatus.Locked;
    private Text text;
    private bool active = true;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCursorController = enemyCursor.GetComponent<ManyCursorController>();
        text = GetComponentInChildren<Text>();
    }

    void Start()
    {
        if (experiment != null)
            experimentLogger = experiment.GetComponent<ExperimentLogger>();
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

    public void MakeInactive()
    {
        active = false;
        spriteRenderer.sprite = locked;
        lockstatus = LockStatus.Locked;
        text.text = "Parcela\nbloqueada";

        if (status == ButtonStatus.Large)
        {
            this.transform.localScale /= 1.1f;
            status = ButtonStatus.Small;
        }

        spriteRenderer.material = greyscaleMaterial;
    }

    public void MakeActive()
    {
        active = true;
        spriteRenderer.material = defaultMaterial;
    }

    public void SetLockMode(LockStatus set)
    {
        if (set == LockStatus.Locked)
        {
            spriteRenderer.sprite = locked;
            lockstatus = LockStatus.Locked;
            text.text = "Parcela\nbloqueada";
        }
        else if (set == LockStatus.Unlocked)
        {
            spriteRenderer.sprite = unlocked;
            lockstatus = LockStatus.Unlocked;
            text.text = "Parcela\ndesbloqueada";
        }
    }

    public void LockSwitch(string caller)
    {
        if (active)
        {
            string player = caller == "CursorA" ? "A" : "B";
            string orchidOwner = (owner == Player.PlayerA) ? "A" : "B";

            if (lockstatus == LockStatus.Unlocked && myCursor.CompareTag(caller))
            {
                spriteRenderer.sprite = locked;
                lockstatus = LockStatus.Locked;
                text.text = "Parcela\nbloqueada";
                enemyCursorController.SelectableFruitsSwitch();
                experimentLogger.Log(player + " bloquea huerto " + orchidOwner);
            }
            else if (lockstatus == LockStatus.Locked && myCursor.CompareTag(caller))
            {
                spriteRenderer.sprite = unlocked;
                lockstatus = LockStatus.Unlocked;
                text.text = "Parcela\ndesbloqueada";
                enemyCursorController.SelectableFruitsSwitch();
                experimentLogger.Log(player + " desbloquea huerto " + orchidOwner);
            }
        }
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
