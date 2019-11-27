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

    public uint scoreA { get { return chestAScript.GetScore(); } }
    public uint scoreB { get { return chestBScript.GetScore(); } }
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
    private ChestController chestAScript, chestBScript;
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
        uint playerAFruits, uint playerBFruits, uint speedA, uint speedB,
        bool freeOrchard, bool enableLock, bool useCommonCounter, bool endGameButton,
        string logPath, int experimentID, int roundNumber, bool save_log = true
        )
    {
        cursorAScript.speed = (int) speedA;
        cursorBScript.speed = (int) speedB;

        if (freeOrchard)
        {
            cursorAScript.selectable = Selectable.Both;
            cursorBScript.selectable = Selectable.Both;
        }
        else
        {
            cursorAScript.selectable = Selectable.PlayerA;
            cursorBScript.selectable = Selectable.PlayerB;
        }

        lockA.SetActive(enableLock);
        lockB.SetActive(enableLock);
        endExperimentButton.SetActive(endGameButton);

        m_save_log = save_log;
        if (save_log)
        {
            logger.SetExperimentID(experimentID);
            logger.SetRound(roundNumber);
            logger.SetPath(logPath);
            logger.SetFruitNumber(playerAFruits, playerBFruits);

            playerALogger.SetPath(logger.GetExperimentPath());
            playerBLogger.SetPath(logger.GetExperimentPath());

            boardManagerA.SetUpExperiment(10, 10, playerAFruits, logger.GetExperimentPath());
            boardManagerB.SetUpExperiment(10, 10, playerBFruits, logger.GetExperimentPath());
        }
        else
        {
            boardManagerA.SetUpExperiment(10, 10, playerAFruits);
            boardManagerB.SetUpExperiment(10, 10, playerBFruits);
        }

        chestAScript = chestA.GetComponentInChildren<ChestController>();
        chestBScript = chestB.GetComponentInChildren<ChestController>();

        chestAScript.SetToCapture(false);
        chestBScript.SetToCapture(false);

        commonCounter.SetActive(useCommonCounter);
        commonCounterHUD.SetActive(useCommonCounter);

        fruits_harvested_by_a = 0;
        fruits_harvested_by_b = 0;

        running = true;
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

    public void StopExperiment()
    {
        if (m_save_log)
        {
            logger.SetScore(
                chestAScript.GetScore(),
                chestBScript.GetScore()
                );
            logger.Save();
            this.SaveCursorLog();
        }

        running = false;
        chestAScript.SetScore(0);
        chestBScript.SetScore(0);
    }
}
