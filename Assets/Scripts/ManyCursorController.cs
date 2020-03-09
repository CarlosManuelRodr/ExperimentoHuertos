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

public enum CursorMode
{
    HandMode,
    ShovelMode
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
    public AudioClip grab, release, changeModeSound;
    public CanInteract fruitsAccess = CanInteract.Both;
    public float cursorLogInterval = 0.5f;
    public Sprite handIcon, shovelIcon;

    public bool isSelecting
    {
        get {
            return selecting;
        }
    }
    public int speed
    {
        get {
            return (int)(100.0f * cursorSpeed / 0.1f);
        }
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
    private BoxCollider2D boxCollider2D;

    private GameObject selected;
    private AudioSource audioSource;
    private ExperimentLogger experimentLogger;
    private Rect playableArea;

    private float nextUpdate;
    private CanInteract initialSelectable;
    private CursorMode cursorMode = CursorMode.HandMode;

    private void Awake()
    {
        initialSelectable = fruitsAccess;
        playableArea = Rect.MinMaxRect(-7.6f, -4.3f, 7.6f, 4.3f);
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        cursorLogger = GetComponent<CursorLogger>();
        boxCollider2D = GetComponent<BoxCollider2D>();
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
    }

    public void ChangeMode(CursorMode mode)
    {
        if (this.isActiveAndEnabled)
            audioSource.PlayOneShot(changeModeSound);
        cursorMode = mode;
        spriteRenderer.sprite = (mode == CursorMode.HandMode) ? handIcon : shovelIcon;
        boxCollider2D.offset = (mode == CursorMode.HandMode)
            ? new Vector2(0.005844295f, -0.00748992f)
            : new Vector2(-0.09f, -0.16f);
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

    public void Setup(Vector2 position)
    {
        transform.localPosition = position;
    }

    void OnEnable()
    {
        SetUpCursor();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDisable()
    {
        selecting = false;
        selected = null;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ChangeMode(CursorMode.HandMode);
    }

    void Update()
    {
        Vector3 cursorDelta = cursorSpeed * mouse.Delta;
        Vector3 mousePosition = transform.position + cursorDelta;

        if (playableArea.Contains(mousePosition))
        {
            this.transform.position = mousePosition;
            if (isSelecting)
            {
                Vector2 shovelOffset = new Vector2(-0.13f, -0.31f);
                Vector2 mousePos2d = mousePosition;
                selected.transform.position =
                    cursorMode == CursorMode.HandMode ? mousePos2d : mousePos2d + shovelOffset;
            }
        }

        if (Time.time >= nextUpdate)
        {
            nextUpdate = Time.time + cursorLogInterval;
            cursorLogger.Log(cam.WorldToScreenPoint(mousePosition), isSelecting, fruitsAccess);
        }
    }

    void CloseHand(ManyMouse mouseId, int buttonId)
    {
        if (isActiveAndEnabled &&  buttonId == 0)
        {
            if (selected != null)
            {
                if (selected.CompareTag("Lock"))
                {
                    selected.GetComponent<LockController>().LockSwitch(tag);
                }
                else if (selected.CompareTag("ItemA") || selected.CompareTag("ItemB"))
                {
                    FruitController fruit = selected.GetComponent<FruitController>();
                    if (fruit.isBuried) // Si el fruto está enterrado no debe interactuar con el cursor.
                    {
                        int digStrength = (cursorMode == CursorMode.ShovelMode) ? 5 : 1;
                        fruit.Dig(digStrength);
                    }
                    else
                    {
                        audioSource.PlayOneShot(grab);
                        fruit.Select(player);
                        selecting = true;

                        if (cursorMode == CursorMode.ShovelMode)
                            fruit.spriteRenderer.sortingOrder += 4;

                        if (experiment != null)
                        {
                            string selector = (player == Player.PlayerA) ? "A" : "B";
                            string fruitOwner = selected.CompareTag("ItemA") ? "A" : "B";
                            experimentLogger.Log(selector + " toma fruto de " + fruitOwner);
                        }
                    }
                }
                else if (selected.CompareTag("ShovelA") || selected.CompareTag("ShovelB"))
                {
                    CursorMode newMode =
                        cursorMode == CursorMode.HandMode ? CursorMode.ShovelMode : CursorMode.HandMode;
                    ShovelController shovelController = selected.GetComponent<ShovelController>();
                    if (shovelController.isActive)
                    {
                        ChangeMode(newMode);
                        shovelController.ChangeMode(newMode);
                        
                        if (experiment != null)
                        {
                            string selector = (player == Player.PlayerA) ? "A" : "B";
                            string toolName = (cursorMode == CursorMode.HandMode) ? "Mano" : "Pala";
                            experimentLogger.Log(selector + " cambia herramienta a " + toolName);
                        }
                    }
                }
            }

            if (cursorMode == CursorMode.HandMode)
                spriteRenderer.sprite = handClosed;
        }
    }

    void OpenHand(ManyMouse mouseId, int buttonId)
    {
        if (buttonId == 0)
        {
            if (selected != null)
            {
                if (selected.CompareTag("ItemA") || selected.CompareTag("ItemB"))
                {
                    audioSource.PlayOneShot(release);

                    FruitController fruit = selected.GetComponent<FruitController>();
                    if (!fruit.isBuried)
                    {
                        fruit.Deselect();
                        if (cursorMode == CursorMode.ShovelMode && fruit.spriteRenderer.sortingOrder == 3)
                            fruit.spriteRenderer.sortingOrder -= 4;
                        
                        if (experiment != null)
                        {
                            string selector = (player == Player.PlayerA) ? "A" : "B";
                            string fruitOwner = selected.CompareTag("ItemA") ? "A" : "B";
                            experimentLogger.Log(selector + " suelta fruto de " + fruitOwner);
                        }
                    }
                }
            }

            selecting = false;
            
            if (cursorMode == CursorMode.HandMode)
                spriteRenderer.sprite = handOpen;
        }
    }

    bool IsSelectable(Collider2D other)
    {
        if (other.CompareTag("Lock"))
            return true;

        if (player == Player.PlayerA && other.CompareTag("ShovelA"))
            return true;
        if (player == Player.PlayerB && other.CompareTag("ShovelB"))
            return true;

        if (fruitsAccess == CanInteract.Both)
        {
            if (other.CompareTag("ItemA") || other.CompareTag("ItemB"))
                return true;
        }
        else
        {
            if (fruitsAccess == CanInteract.PlayerA && other.CompareTag("ItemA"))
                return true;
            if (fruitsAccess == CanInteract.PlayerB && other.CompareTag("ItemB"))
                return true;
        }
        return false;
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
