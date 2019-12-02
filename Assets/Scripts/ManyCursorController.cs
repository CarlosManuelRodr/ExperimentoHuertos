using UnityEngine;

public enum CanInteract
{
    PlayerA,
    PlayerB,
    Both
}

public enum Player
{
    PlayerA,
    PlayerB
}

/// <summary>
/// Gestor de cursores. Utiliza la biblioteca ManyMouse para gestionar el input de múltiples cursores.
/// </summary>
public class ManyCursorController : MonoBehaviour
{
    public Player player = Player.PlayerA;
    public GameObject experiment;

    [Range(0.0f, 0.1f)]
    public float cursorSpeed = 0.03f;
    public Sprite handOpen, handClosed;
    public AudioClip grab, release;
    public CanInteract fruitsAccess = CanInteract.Both;
    public float cursorLogInterval = 0.5f;

    public bool isSelecting { get { return selecting; } }
    public int speed
    {
        get { return (int)(100.0f * cursorSpeed / 0.1f); }
        set {
                if (value >= 0 && value <= 100)
                {
                    cursorSpeed = 0.1f * value / 100.0f;
                }
            }
    }

    private bool selecting;

    private ManyMouse mouse;
    private CursorLogger cursorLogger;
    private SpriteRenderer spriteRenderer;
    private Camera cam;

    private GameObject selected;
    private AudioSource audioSource;
    private ExperimentLogger experimentLogger;
    private Rect playableArea;

    private float nextUpdate;
    private float initialSpeed;
    private CanInteract initialSelectable;

    private void Awake()
    {
        initialSelectable = fruitsAccess;
        playableArea = Rect.MinMaxRect(-7.6f, -4.3f, 7.6f, 4.3f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        cursorLogger = GetComponent<CursorLogger>();
    }

    void Start()
    {
        if (experiment != null)
            experimentLogger = experiment.GetComponent<ExperimentLogger>();
        cam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        selecting = false;

        cursorLogger.SetCursorID(transform.tag);
        nextUpdate = cursorLogInterval;
        initialSpeed = cursorSpeed;
    }

    void SetUpCursor()
    {
        int mouseNumber;
        if (player == Player.PlayerA)
            mouseNumber = PlayerPrefs.GetInt("MouseIdA", 0);
        else
            mouseNumber = PlayerPrefs.GetInt("MouseIdB", 1);

        mouse = ManyMouseWrapper.GetMouseByID(mouseNumber);
        mouse.EventButtonDown = delegate { };
        mouse.EventButtonUp = delegate { };
        mouse.EventButtonDown += CloseHand;
        mouse.EventButtonUp += OpenHand;
    }

    public void SetPlayableArea(Rect rect)
    {
        playableArea = rect;
    }

    public void SelectableFruitsSwitch()
    {
        if (fruitsAccess == initialSelectable)
            fruitsAccess = CanInteract.Both;
        else
            fruitsAccess = initialSelectable;
    }

    public void SelectableFruits(CanInteract canInteract)
    {
        fruitsAccess = canInteract;
    }

    void OnEnable()
    {
        this.SetUpCursor();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        selecting = false;
        selected = null;
        fruitsAccess = initialSelectable;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        Vector3 cursorDelta = cursorSpeed * mouse.Delta;
        Vector3 mousePosition = transform.position + cursorDelta;

        if (playableArea.Contains(mousePosition))
        {
            this.transform.position = mousePosition;
            if (isSelecting && selected != null)
                selected.transform.position = mousePosition;
        }

        if (Time.time >= nextUpdate)
        {
            nextUpdate = Time.time + cursorLogInterval;
            cursorLogger.Log(cam.WorldToScreenPoint(mousePosition), isSelecting, fruitsAccess);
        }
    }

    public void CloseHand(ManyMouse mouse, int buttonId)
    {
        if (buttonId == 0)
        {
            if (selected != null)
            {
                if (selected.tag == "Lock")
                {
                    selected.GetComponent<LockController>().LockSwitch(this.tag);
                }
                else if (selected.tag == "ItemA" || selected.tag == "ItemB")
                {
                    FruitController fruit = selected.GetComponent<FruitController>();
                    if (!fruit.isBuried) // Si el fruto está enterrado no debe interactuar con el cursor.
                    {
                        audioSource.PlayOneShot(grab);
                        fruit.Select(player);
                        selecting = true;

                        if (experiment != null)
                        {
                            string selector = (player == Player.PlayerA) ? "A" : "B";
                            string fruitOwner = (selected.tag == "ItemA") ? "A" : "B";
                            experimentLogger.Log(selector + " toma fruto de " + fruitOwner);
                        }
                    }
                }
                else if (selected.tag == "Shovel")
                {
                    ShovelController shovel = selected.GetComponent<ShovelController>();
                    if (!shovel.isSelected)
                    {
                        shovel.Select(player);
                        audioSource.PlayOneShot(grab);
                        selecting = true;
                        cursorSpeed = 0.005f;

                        if (experiment != null)
                        {
                            string selector = (player == Player.PlayerA) ? "A" : "B";
                            experimentLogger.Log(selector + " toma la pala");
                        }
                    }
                }
            }

            spriteRenderer.sprite = handClosed;
        }
    }

    public void OpenHand(ManyMouse mouse, int buttonId)
    {
        if (buttonId == 0)
        {
            if (selected != null)
            {
                if (selected.tag == "ItemA" || selected.tag == "ItemB")
                {
                    audioSource.PlayOneShot(release);

                    FruitController fruit = selected.GetComponent<FruitController>();
                    if (!fruit.isBuried)
                    {
                        fruit.Deselect();
                        if (experiment != null)
                        {
                            string selector = (player == Player.PlayerA) ? "A" : "B";
                            string fruitOwner = (selected.tag == "ItemA") ? "A" : "B";
                            experimentLogger.Log(selector + " suelta fruto de " + fruitOwner);
                        }
                    }
                }
                else if (selected.tag == "Shovel")
                {
                    ShovelController shovel = selected.GetComponent<ShovelController>();
                    if (shovel.selector == player)
                    {
                        shovel.Deselect();
                        cursorSpeed = initialSpeed;
                    }

                    if (experiment != null)
                    {
                        string selector = (player == Player.PlayerA) ? "A" : "B";
                        experimentLogger.Log(selector + " suelta la pala");
                    }
                }
            }

            selecting = false;
            spriteRenderer.sprite = handOpen;
        }
    }

    bool IsSelectable(Collider2D other)
    {
        if (other.tag == "Lock")
            return true;

        if (other.tag == "Shovel")
            return true;

        if (fruitsAccess == CanInteract.Both)
        {
            if (other.tag == "ItemA" || other.tag == "ItemB")
                return true;
            else
                return false;
        }
        else
        {
            if (fruitsAccess == CanInteract.PlayerA && other.tag == "ItemA")
                return true;
            if (fruitsAccess == CanInteract.PlayerB && other.tag == "ItemB")
                return true;
        }
        return false;
    }

    public void SetSelectable(CanInteract type)
    {
        fruitsAccess = type;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isSelecting && IsSelectable(other))
        {
            selected = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!isSelecting && IsSelectable(other))
            selected = null;
    }
}
