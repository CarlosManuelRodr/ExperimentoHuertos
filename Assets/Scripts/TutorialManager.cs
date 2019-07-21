using System.Collections;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Administrador del tutorial.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject checkA, checkB;
    public GameObject tutorial1, tutorial2, tutorial3;
    public GameObject fruitA, fruitB;
    public GameObject chestA, chestB;
    public GameObject playerACursor, playerBCursor;
    public bool debug = false;

    private GameObject fruitAInstance, fruitBInstance;
    private GameManager gameManagerScript;
    private Vector2 playerACursorPos, playerBCursorPos;
    private Vector2 fruitAPos, fruitBPos;
    private int tutorialPhase;
    private VideoPlayer tutorial1Video, tutorial2Video, tutorial3Video;
    private FruitController fruitAController, fruitBController;
    private ChestController chestAController, chestBController;
    private bool checkedA, checkedB;

    void Awake()
    {
        tutorial1Video = tutorial1.GetComponent<VideoPlayer>();
        tutorial2Video = tutorial2.GetComponent<VideoPlayer>();
        tutorial3Video = tutorial3.GetComponent<VideoPlayer>();
        chestAController = chestA.GetComponentInChildren<ChestController>();
        chestBController = chestB.GetComponentInChildren<ChestController>();
        gameManagerScript = gameManager.GetComponent<GameManager>();

        playerACursorPos = playerACursor.transform.position;
        playerBCursorPos = playerBCursor.transform.position;

        fruitAPos = new Vector2(-4.945538f, -0.9905443f);
        fruitBPos = new Vector2(3.511462f, -0.9905443f);

        fruitAInstance = null;
        fruitBInstance = null;
    }

    void Update()
    {
        // Tecla para omitir fase de tutorial.
        if (debug && Input.GetKeyDown(KeyCode.Escape))
            this.NextPhase();

        if (tutorialPhase == 1)
        {
            if (!checkedA && fruitAController.isHighlighted)
            {
                checkA.SetActive(true);
                checkA.GetComponent<AudioSource>().Play();
                checkedA = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }

            if (!checkedB && fruitBController.isHighlighted)
            {
                checkB.SetActive(true);
                checkB.GetComponent<AudioSource>().Play();
                checkedB = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }
        }

        if (tutorialPhase == 2)
        {
            if (!checkedA && fruitAController.isSelected)
            {
                checkA.SetActive(true);
                checkA.GetComponent<AudioSource>().Play();
                checkedA = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }

            if (!checkedB && fruitBController.isSelected)
            {
                checkB.SetActive(true);
                checkB.GetComponent<AudioSource>().Play();
                checkedB = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }
        }

        if (tutorialPhase == 3)
        {
            if (!checkedA && chestAController.score != 0)
            {
                checkA.SetActive(true);
                checkA.GetComponent<AudioSource>().Play();
                checkedA = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }

            if (!checkedB && chestBController.score != 0)
            {
                checkB.SetActive(true);
                checkB.GetComponent<AudioSource>().Play();
                checkedB = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }
        }

        if (tutorialPhase == 4)
        {
            if (fruitAInstance != null)
                Destroy(fruitAInstance);
            if (fruitBInstance != null)
                Destroy(fruitBInstance);

            tutorialPhase = 0;
            gameManagerScript.EndTutorial();
        }
    }

    public void ActivateCursors()
    {
        playerACursor.transform.position = playerACursorPos;
        playerBCursor.transform.position = playerBCursorPos;
        playerACursor.SetActive(true);
        playerBCursor.SetActive(true);
    }

    public void DeactivateCursors()
    {
        if (playerACursor.activeSelf)
            playerACursor.SetActive(false);

        if (playerBCursor.activeSelf)
            playerBCursor.SetActive(false);
    }

    private IEnumerator DelayNextPhase()
    {
        yield return new WaitForSeconds(1.0f);
        this.NextPhase();
    }

    private void NextPhase()
    {
        tutorialPhase++;
        checkedA = false;
        checkedB = false;
        checkA.SetActive(false);
        checkB.SetActive(false);

        if (tutorialPhase == 2)
        {
            tutorial1Video.Stop();
            tutorial2Video.Play();
        }

        if (tutorialPhase == 3)
        {
            tutorial2Video.Stop();
            tutorial3Video.Play();

            chestA.SetActive(true);
            chestB.SetActive(true);
        }
    }

    public void InitializeTutorial()
    {
        checkedA = false;
        checkedB = false;
        chestA.SetActive(false);
        chestB.SetActive(false);

        fruitAInstance = Instantiate(fruitA, fruitAPos, Quaternion.identity) as GameObject;
        fruitBInstance = Instantiate(fruitB, fruitBPos, Quaternion.identity) as GameObject;
        fruitAInstance.transform.SetParent(this.transform);
        fruitBInstance.transform.SetParent(this.transform);

        fruitAController = fruitAInstance.GetComponent<FruitController>();
        fruitBController = fruitBInstance.GetComponent<FruitController>();

        tutorialPhase = 1;
        tutorial1Video.Play();
    }
}
