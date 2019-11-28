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
    public Player owner = Player.PlayerA;
    public GameObject experiment;

    private ExperimentLogger experimentLogger;
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

    public void LockSwitch(string caller)
    {
        string player = (caller == "CursorA") ? "A" : "B";
        string orchidOwner = (owner == Player.PlayerA) ? "A" : "B";

        if (lockstatus == LockStatus.Unlocked && caller == myCursor.tag)
        {
            spriteRenderer.sprite = locked;
            lockstatus = LockStatus.Locked;
            text.text = "Parcela\nbloqueada";
            enemyCursorController.SelectableFruitsSwitch();
            experimentLogger.Log(player + " bloquea huerto " + orchidOwner);
        }
        else if (lockstatus == LockStatus.Locked && caller == myCursor.tag)
        {
            spriteRenderer.sprite = unlocked;
            lockstatus = LockStatus.Unlocked;
            text.text = "Parcela\ndesbloqueada";
            enemyCursorController.SelectableFruitsSwitch();
            experimentLogger.Log(player + " desbloquea huerto " + orchidOwner);
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
