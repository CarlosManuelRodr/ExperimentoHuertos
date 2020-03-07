using UnityEngine;

/// <summary>
/// Controlador del fruto. Gestiona la interacción con el cursor y el cofre.
/// </summary>
public class FruitController : MonoBehaviour
{
    public GameObject highlight;
    public float returnSpeed = 1.0f;
    public bool buried = false;
    public bool isSelected { get { return selected; } }
    public bool isFalling { get { return falling; } }
    public bool isHighlighted { get { return highlight.activeSelf; } }
    public bool isBuried { get { return buried;  } }
    public Player selector { get { return whoSelected; } }
    public float fruitLogInterval = 0.2f;
    public Sprite idleSprite, buriedSprite;
    public AudioClip hitAudio, unearthAudio;
    public bool log = true;

    private SpriteRenderer fruitRenderer, highlightRenderer = null;
    private Rigidbody2D rigidbody2d;
    private FruitLogger fruitLogger;
    private Camera cam;
    private ParticleSystem particles;
    private AudioSource audioSource;
    private Player whoSelected;
    private Color red, yellow, green;
    private Vector3 startPos;
    private bool inChest;
    private bool returnToStart;
    private bool selected;
    private bool falling;
    private float nextUpdate;
    private int resistance;

    void Awake()
    {
        highlight.SetActive(false);
        highlightRenderer = highlight.GetComponent<SpriteRenderer>();
        fruitRenderer = GetComponent<SpriteRenderer>();
        fruitLogger = GetComponent<FruitLogger>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        particles = GetComponentInChildren<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
        cam = Camera.main;

        // Colores del highlight. Usa colores predefinidos en caso de que
        // el parsing falle.
        if (!ColorUtility.TryParseHtmlString("#FF170BB6", out red))
            red = Color.red;
        if (!ColorUtility.TryParseHtmlString("#FFCA0BB6", out yellow))
            yellow = Color.yellow;
        if (!ColorUtility.TryParseHtmlString("#84FF0BB6", out green))
            green = Color.green;

        startPos = transform.position;
        inChest = false;
        returnToStart = false;
        selected = false;
        falling = false;
        resistance = Random.Range(3, 10);
        nextUpdate = fruitLogInterval;
        audioSource.clip = hitAudio;
        if (buried)
            this.SetBuried(true);
    }

    public void SetBuried(bool buried_a)
    {
        buried = buried_a;
        if (buried)
            fruitRenderer.sprite = buriedSprite;
        else
            fruitRenderer.sprite = idleSprite;
    }

    private void OnDisable()
    {
        // Guarda el log cuando el fruto es destruido.
        if (fruitLogger != null)
            fruitLogger.Save();
    }

    void Update()
    {
        if (!buried)
        {
            if (returnToStart)
            {
                // Mueve fruto hasta que regrese a la posición inicial.
                float step = returnSpeed * Time.deltaTime;
                Vector2 newPosition = Vector2.MoveTowards(transform.position, startPos, step);
                rigidbody2d.MovePosition(newPosition);

                if (transform.position == startPos)
                    returnToStart = false;
            }
            else
            {
                // Asigna posición inicial en caso de que no haya sido asignada anteriormente.
                if (!selected && startPos != transform.position)
                    startPos = transform.position;
            }

            // Guarda posición del fruto en el log.
            if (Time.time >= nextUpdate)
            {
                nextUpdate = Time.time + fruitLogInterval;
                if (log && startPos != transform.position && fruitLogger != null)
                {
                    string selector = (whoSelected == Player.PlayerA) ? "PlayerA" : "PlayerB";
                    fruitLogger.Log(cam.WorldToScreenPoint(transform.position), selector);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (buried)
        {
            
        }
        else
        {
            // Activa el highlight en caso de que el cursor entre en contacto con el fruto.
            if (other.tag == "CursorA" || other.tag == "CursorB")
            {
                ManyCursorController cursor = other.gameObject.GetComponent<ManyCursorController>();
                if (!cursor.isSelecting)
                    highlight.SetActive(true);
            }

            // En caso de que entre en contacto con un cofre, activa el highlight verde.
            if (other.tag == "Chest")
            {
                ChestVisuals chestVisuals = other.GetComponentInChildren<ChestVisuals>();
                chestVisuals.numberOfOccupants++;

                if (chestVisuals.numberOfOccupants == 1 && isSelected)
                {
                    if (
                        chestVisuals.chestAccess == CanInteract.Both ||
                        (selector == Player.PlayerA && chestVisuals.chestAccess == CanInteract.PlayerA) ||
                        (selector == Player.PlayerB && chestVisuals.chestAccess == CanInteract.PlayerB)
                       )
                    {
                        chestVisuals.EnterFruit(this);
                        highlightRenderer.color = green;
                        inChest = true;
                    }
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!buried)
        {
            if (other.tag == "CursorA" || other.tag == "CursorB")
                highlight.SetActive(false);

            if (other.tag == "Chest")
            {
                ChestVisuals chestVisuals = other.GetComponentInChildren<ChestVisuals>();
                chestVisuals.numberOfOccupants--;

                if (inChest)
                {
                    if (!falling)
                    {
                        chestVisuals.ExitFruit();
                        inChest = false;
                    }

                    if (returnToStart)
                        highlightRenderer.color = red;
                    else
                        highlightRenderer.color = yellow;
                }
            }
        }
    }

    public void Select(Player who)
    {
        if (!buried)
        {
            whoSelected = who;
            highlightRenderer.color = yellow;
            highlightRenderer.sortingOrder += 2;
            fruitRenderer.sortingOrder += 2;
            selected = true;
        }
    }

    public void Deselect()
    {
        if (!buried)
        {
            highlightRenderer.color = red;
            highlightRenderer.sortingOrder -= 2;
            fruitRenderer.sortingOrder -= 2;
            selected = false;

            if (transform.position != startPos)
            {
                // Si se deselecciona dentro de cofre, cae como cuerpo rígido. En caso contrario
                // regresa a la posición inicial.
                if (inChest)
                {
                    rigidbody2d.bodyType = RigidbodyType2D.Dynamic;
                    falling = true;
                }
                else
                    returnToStart = true;
            }
        }
    }
}
