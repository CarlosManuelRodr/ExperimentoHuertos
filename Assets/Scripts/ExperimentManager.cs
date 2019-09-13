using UnityEngine;

/// <summary>
/// Gestor de ejecución del experimento.
/// </summary>
public class ExperimentManager : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject boardA, boardB;
    public GameObject chestA, chestB, chestCommon;
    public GameObject playerACursor, playerBCursor;
    public GameObject lockA, lockB, endExperimentButton;

    public uint scoreA { get { return chestAScript.GetScore(); } }
    public uint scoreB { get { return chestBScript.GetScore(); } }
    public uint scoreCommon { get { return chestCommonScript.GetScore(); } }

    private GameManager gameManagerScript;
    private Vector2 playerACursorPos, playerBCursorPos;
    private BoardManager boardManagerA, boardManagerB;
    private ChestController chestAScript, chestBScript;
    private CommonChestController chestCommonScript;
    private Vector2 aiCursorPosition;
    private ManyCursorController cursorAScript, cursorBScript;

    private string path;
    private ExperimentLogger logger;
    private CursorLogger playerALogger, playerBLogger;
    private bool running;

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
        bool freeOrchard, bool enableLock, bool commonCounter, bool endGameButton,
        string logPath, int experimentID, int roundNumber
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

        logger.SetExperimentID(experimentID);
        logger.SetRound(roundNumber);
        logger.SetPath(logPath);
        logger.SetFruitNumber(playerAFruits, playerBFruits);

        playerALogger.SetPath(logger.GetExperimentPath());
        playerBLogger.SetPath(logger.GetExperimentPath());

        boardManagerA.SetUpExperiment(10, 10, playerAFruits, logger.GetExperimentPath());
        boardManagerB.SetUpExperiment(10, 10, playerBFruits, logger.GetExperimentPath());

        chestAScript = chestA.GetComponentInChildren<ChestController>();
        chestBScript = chestB.GetComponentInChildren<ChestController>();
        chestCommonScript = chestCommon.GetComponentInChildren<CommonChestController>();
        chestCommonScript.actAsCounter = commonCounter;

        chestAScript.SetToCapture(false);
        chestBScript.SetToCapture(false);

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
        logger.SetScore(
            chestAScript.GetScore(),
            chestBScript.GetScore(),
            chestCommonScript.GetScore()
            );
        logger.Save();
        this.SaveCursorLog();
        running = false;

        chestAScript.SetScore(0);
        chestBScript.SetScore(0);
        chestCommonScript.ResetScore();
        chestCommonScript.SetScore(0);
    }
}
