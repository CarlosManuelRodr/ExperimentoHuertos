using System.Collections.Generic;
using UnityEngine;

public enum StateIds
{
    FetchFruit,
    DepositFruit,
    Idle
}

public class EnemyAi : MonoBehaviour
{
    public GameObject myChest;
    public GameObject myBoard;
    public Sprite handOpen, handClosed;
    public float speed = 1.0f;
    public bool isSelecting { get { return selecting; } }
    public float cursorLogInterval = 0.5f;

    private bool selecting;
    private SpriteRenderer spriteRenderer;
    private List<Transform> fruitPositions;
    private StateIds currentStateId;
    private Transform currentFetch, chestTransform;
    private GameObject selected;
    private CursorLogger cursorLogger;
    private float nextUpdate;

    void Awake()
    {
        currentStateId = StateIds.Idle;
        chestTransform = myChest.GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cursorLogger = GetComponent<CursorLogger>();
        currentFetch = null;
        selecting = false;

        cursorLogger.SetCursorID(transform.tag);
        nextUpdate = cursorLogInterval;
    }

    void OnDisable()
    {
        cursorLogger.Save();
        cursorLogger.Clear();
    }

    void Update()
    {
        switch (currentStateId)
        {
            case StateIds.FetchFruit:
                this.FetchFruit();
                break;
            case StateIds.DepositFruit:
                this.DepositFruit();
                break;
            case StateIds.Idle:
                break;
        }

        if (isSelecting && selected != null)
            selected.transform.position = transform.position;

        if (Time.time >= nextUpdate)
        {
            nextUpdate = Time.time + cursorLogInterval;
            cursorLogger.Log(transform.position, currentStateId == StateIds.DepositFruit);
        }
    }

    List<Transform> GetFruitPositions()
    {
        Transform[] positions = myBoard.GetComponentsInChildren<Transform>();
        List<Transform> positionList = new List<Transform>(); ;

        foreach (Transform t in positions)
        {
            if (t.gameObject != this.gameObject)
                positionList.Add(t);
        }

        return positionList;
    }

    Transform RandomFruit()
    {
        int randomIndex = Random.Range(0, fruitPositions.Count);
        Transform randomPosition = fruitPositions[randomIndex];
        if (randomPosition.gameObject == null)
        {
            fruitPositions = GetFruitPositions();
            randomIndex = Random.Range(0, fruitPositions.Count);
            randomPosition = fruitPositions[randomIndex];
        }
        fruitPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void FetchFruit()
    {
        if (currentFetch == null)
        {
            fruitPositions = GetFruitPositions();
            currentFetch = RandomFruit();
        }

        if (transform.position == currentFetch.position)
        {
            CloseHand();
            currentStateId = StateIds.DepositFruit;
            selecting = true;
            myChest.GetComponentInChildren<ChestController>().SetToCapture(false);
        }
        else
        {
            float step = speed * Time.deltaTime;
            Vector2 newPosition = Vector2.MoveTowards(transform.position, currentFetch.position, step);
            this.transform.position = newPosition;
        }
    }

    void DepositFruit()
    {
        if (currentFetch == null)
        {
            currentStateId = StateIds.FetchFruit;
            currentFetch = RandomFruit();
        }

        if (transform.position == chestTransform.position)
        {
            OpenHand();
            if (fruitPositions.Count == 0)
                this.StopAI();
            else
            {
                currentStateId = StateIds.FetchFruit;
                currentFetch = RandomFruit();
            }
        }
        else
        {
            float step = speed * Time.deltaTime;
            Vector2 newPosition = Vector2.MoveTowards(transform.position, chestTransform.position, step);
            this.transform.position = newPosition;
        }
    }

    public void CloseHand()
    {
        if (currentFetch == null)
        {
            currentStateId = StateIds.FetchFruit;
            currentFetch = RandomFruit();
        }
        else
        {
            if (selected != null)
            {
                selected.GetComponent<FruitController>().Select();
                selecting = true;
            }
            else
            {
                InitAI(); // Restart
            }
        }

        spriteRenderer.sprite = handClosed;
    }

    public void OpenHand()
    {
        if (selected != null)
        {
            selected.GetComponent<FruitController>().Deselect();
        }

        selecting = false;
        spriteRenderer.sprite = handOpen;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isSelecting && other.tag == "ItemB")
        {
            selected = other.gameObject;
        }

        if (isSelecting && other.tag == "Chest")
        {
            other.transform.parent.GetComponentInChildren<ChestController>().SetToCapture(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (isSelecting && other.tag == "Chest")
        {
            other.transform.parent.GetComponentInChildren<ChestController>().SetToCapture(false);
        }
    }

    public void InitAI()
    {
        fruitPositions = GetFruitPositions();
        currentStateId = StateIds.FetchFruit;
    }

    public void StopAI()
    {
        currentStateId = StateIds.Idle;
        currentFetch = null;
        selecting = false;
        spriteRenderer.sprite = handOpen;
    }
}
