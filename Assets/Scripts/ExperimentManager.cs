using UnityEngine;

/// <summary>
/// Gestor de ejecución del experimento.
/// </summary>
public class ExperimentManager : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject boardA, boardB;
    public GameObject chestA, chestB;
    public GameObject playerACursor, playerBCursor;
    public GameObject lockA, lockB, endExperimentButton;
    public GameObject commonCounter;
    public GameObject commonCounterHUD;
    public GameObject shovel, shovelSupport;

    public uint scoreA { get { return chestAController.GetScore(); } }
    public uint scoreB { get { return chestBController.GetScore(); } }
    public uint harvestedA
    {
        get { return fruits_harvested_by_a; }
        set { fruits_harvested_by_a = value; }
    }
    public uint harvestedB
    {
        get { return fruits_harvested_by_b; }
        set { fruits_harvested_by_b = value; }
    }

    private uint fruits_harvested_by_a;
    private uint fruits_harvested_by_b;

    private GameManager gameManagerScript;
    private Vector2 playerACursorPos, playerBCursorPos;
    private BoardManager boardManagerA, boardManagerB;
    private ChestController chestAController, chestBController;
    private ChestVisuals chestAVisuals, chestBVisuals;
    private ManyCursorController cursorAScript, cursorBScript;

    private string path;
    private ExperimentLogger logger;
    private CursorLogger playerALogger, playerBLogger;
    private bool running;
    private bool m_save_log;

    void Awake()
    {
        gameManagerScript = gameManager.GetComponent<GameManager>();
        logger = GetComponent<ExperimentLogger>();
        boardManagerA = boardA.GetComponent<BoardManager>();
        boardManagerB = boardB.GetComponent<BoardManager>();
        playerALogger = playerACursor.GetComponent<CursorLogger>();
        playerBLogger = playerBCursor.GetComponent<CursorLogger>();
        cursorAScript = playerACursor.GetComponent<ManyCursorController>();
        cursorBScript = playerBCursor.GetComponent<ManyCursorController>();

        chestAVisuals = chestA.GetComponentInChildren<ChestVisuals>();
        chestBVisuals = chestB.GetComponentInChildren<ChestVisuals>();
        chestAController = chestA.GetComponentInChildren<ChestController>();
        chestBController = chestB.GetComponentInChildren<ChestController>();

        playerACursorPos = playerACursor.transform.position;
        playerBCursorPos = playerBCursor.transform.position;

        running = false;
    }

    void Update()
    {
        if (running)
        {
            // Termina experimento cuando se hayan recolectado todos los frutos.
            if (boardManagerA.CountFruits() == 0 && boardManagerB.CountFruits() == 0)
            {
                gameManagerScript.EndExperiment();
            }
        }
    }

    public void SaveCursorLog()
    {
        playerALogger.Save();
        playerBLogger.Save();
        playerALogger.Clear();
        playerBLogger.Clear();
    }

    public void InitializeExperiment(
        uint playerAFruits, uint playerBFruits, uint buriedA, uint buriedB, uint speedA, uint speedB,
        bool useCommonCounter, bool endGameButton,
        ObjectAccessType lockAccess, ObjectAccessType shovelAccess, ChestAccessType chestAccess,
        string logPath, int experimentID, int roundNumber, bool save_log = true
        )
    {
        cursorAScript.speed = (int) speedA;
        cursorBScript.speed = (int) speedB;

        if (buriedA == 0 && buriedB == 0)
        {
            shovel.SetActive(false);
            shovelSupport.SetActive(false);
        }
        else
        {
            shovel.SetActive(true);
            shovelSupport.SetActive(true);
        }

        // Configura el acceso a los huertos.
        cursorAScript.fruitsAccess = CanInteract.PlayerA;
        cursorBScript.fruitsAccess = CanInteract.PlayerB;

        // Configura el acceso a los cofres.
        switch (chestAccess)
        {
            case ChestAccessType.BOTH_FREE:
                chestAVisuals.chestAccess = CanInteract.Both;
                chestBVisuals.chestAccess = CanInteract.Both;
                break;
            case ChestAccessType.MUTUAL_BLOCK:
                chestAVisuals.chestAccess = CanInteract.PlayerA;
                chestBVisuals.chestAccess = CanInteract.PlayerB;
                break;
            case ChestAccessType.A_FREE:
                chestAVisuals.chestAccess = CanInteract.PlayerA;
                chestBVisuals.chestAccess = CanInteract.Both;
                break;
            case ChestAccessType.B_FREE:
                chestAVisuals.chestAccess = CanInteract.Both;
                chestBVisuals.chestAccess = CanInteract.PlayerB;
                break;
        }

        // Configura logger.
        m_save_log = save_log;
        if (save_log)
        {
            logger.SetExperimentID(experimentID);
            logger.SetRound(roundNumber);
            logger.SetPath(logPath);
            logger.SetFruitNumber(playerAFruits, playerBFruits);

            playerALogger.SetPath(logger.GetExperimentPath());
            playerBLogger.SetPath(logger.GetExperimentPath());

            boardManagerA.SetUpExperiment(10, 10, playerAFruits, buriedA, logger.GetExperimentPath());
            boardManagerB.SetUpExperiment(10, 10, playerBFruits, buriedB, logger.GetExperimentPath());
        }
        else
        {
            boardManagerA.SetUpExperiment(10, 10, playerAFruits, buriedA);
            boardManagerB.SetUpExperiment(10, 10, playerBFruits, buriedB);
        }

        

        lockA.SetActive(enableLock);
        lockB.SetActive(enableLock);
        endExperimentButton.SetActive(endGameButton);

        chestAController.SetToCapture(false);
        chestBController.SetToCapture(false);

        commonCounter.SetActive(useCommonCounter);
        commonCounterHUD.SetActive(useCommonCounter);

        fruits_harvested_by_a = 0;
        fruits_harvested_by_b = 0;

        running = true;
    }

    public void ActivateCursors()
    {
        cursorAScript.Setup(playerACursorPos);
        cursorBScript.Setup(playerBCursorPos);

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

    public void StopExperiment()
    {
        if (m_save_log)
        {
            logger.SetScore(
                chestAController.GetScore(),
                chestBController.GetScore()
                );
            logger.SetCollected(harvestedA, harvestedB);
            logger.Save();
            this.SaveCursorLog();
        }

        running = false;
        chestAController.SetScore(0);
        chestBController.SetScore(0);
    }
}
