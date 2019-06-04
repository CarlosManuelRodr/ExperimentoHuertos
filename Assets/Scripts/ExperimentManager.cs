using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject boardA, boardB;
    public GameObject chestA, chestB, chestCommon;
    public GameObject playerACursor, playerBCursor, aiCursor;
    public GameObject lockA, lockB, endExperimentButton;

    private GameManager gameManagerScript;
    private Vector2 playerACursorPos, playerBCursorPos;
    private bool usingAi = false;
    private BoardManager boardManagerA, boardManagerB;
    private ChestController chestAScript, chestBScript, chestCommonScript;
    private Vector2 aiCursorPosition;

    private string path;
    private ExperimentLogger logger;
    private CursorLogger playerALogger, playerBLogger, aiLogger;
    private bool running;

    void Awake()
    {
        gameManagerScript = gameManager.GetComponent<GameManager>();
        logger = GetComponent<ExperimentLogger>();
        boardManagerA = boardA.GetComponent<BoardManager>();
        boardManagerB = boardB.GetComponent<BoardManager>();
        playerALogger = playerACursor.GetComponent<CursorLogger>();
        playerBLogger = playerBCursor.GetComponent<CursorLogger>();

        if (aiCursor != null)
        {
            aiCursorPosition = aiCursor.GetComponent<Transform>().position;
            aiLogger = aiCursor.GetComponent<CursorLogger>();
        }

        playerACursorPos = playerACursor.transform.position;
        playerBCursorPos = playerBCursor.transform.position;

        running = false;
    }

    void Update()
    {
        if (running)
        {
            if (boardManagerA.CountFruits() == 0 && boardManagerB.CountFruits() == 0)
                gameManagerScript.EndExperiment();
        }
    }

    public void InitializeExperiment(
        uint playerAFruits, uint playerBFruits, uint speedA, uint speedB,
        bool simulateB, bool enableLock, bool commonCounter, bool endGameButton
        )
    {
        playerACursor.transform.position = playerACursorPos;
        playerBCursor.transform.position = playerBCursorPos;

        lockA.SetActive(enableLock);
        lockB.SetActive(enableLock);
        endExperimentButton.SetActive(endGameButton);

        usingAi = simulateB;

        boardManagerA.SetUpExperiment(10, 10, playerAFruits);
        boardManagerB.SetUpExperiment(10, 10, playerBFruits);

        // Set speed

        chestAScript = chestA.GetComponentInChildren<ChestController>();
        chestBScript = chestB.GetComponentInChildren<ChestController>();
        chestCommonScript = chestCommon.GetComponentInChildren<ChestController>();

        chestAScript.SetToCapture(false);
        chestBScript.SetToCapture(false);
        chestCommonScript.SetToCapture(false);

        logger.SetDefaultPath();
        logger.SetFruitNumber(playerAFruits, playerBFruits);

        running = true;
    }

    public void ActivateCursors()
    {
        playerACursor.SetActive(true);
        if (usingAi)
        {
            if (aiCursor != null)
            {
                aiCursor.transform.position = aiCursorPosition;
                aiCursor.SetActive(true);
                aiCursor.GetComponent<EnemyAi>().InitAI();
            }
            else
                Debug.LogError("ExperimentManager has no aiPlayer cursor");
        }
        else
        {
            aiCursor.SetActive(false);
            playerBCursor.SetActive(true);
        }

        playerALogger.SetPath(logger.GetExperimentPath());
        playerBLogger.SetPath(logger.GetExperimentPath());

        if (aiCursor != null)
            aiLogger.SetPath(logger.GetExperimentPath());
    }

    public void DeactivateCursors()
    {
        if (aiCursor != null && aiCursor.activeSelf)
        {
            aiCursor.GetComponent<EnemyAi>().StopAI();
            aiCursor.SetActive(false);
            aiCursor.GetComponent<Rigidbody2D>().MovePosition(aiCursorPosition);
        }

        if (playerACursor.activeSelf)
            playerACursor.SetActive(false);

        if (playerBCursor.activeSelf)
            playerBCursor.SetActive(false);
    }

    public void StopExperiment()
    {
        logger.Save();
        running = false;
    }
}
