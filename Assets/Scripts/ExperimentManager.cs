using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    public GameObject boardA, boardB;
    public GameObject chestA, chestB, chestCommon;
    public GameObject playerACursor, playerBCursor, aiCursor;

    private bool usingAi = false;
    private BoardManager boardManagerA, boardManagerB;
    private ChestController chestAScript, chestBScript, chestCommonScript;
    private Vector2 aiCursorPosition;

    private string path;
    private ExperimentLogger logger;
    private CursorLogger playerALogger, playerBLogger, aiLogger;

    void Awake()
    {
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
    }

    public void InitializeExperiment(uint playerAFruits, uint playerBFruits, bool aiPlayer = false)
    {
        usingAi = aiPlayer;

        boardManagerA.SetUpExperiment(10, 10, playerAFruits);
        boardManagerB.SetUpExperiment(10, 10, playerBFruits);

        chestAScript = chestA.GetComponentInChildren<ChestController>();
        chestBScript = chestB.GetComponentInChildren<ChestController>();
        chestCommonScript = chestCommon.GetComponentInChildren<ChestController>();

        chestAScript.SetToCapture(false);
        chestBScript.SetToCapture(false);
        chestCommonScript.SetToCapture(false);

        logger.SetDefaultPath();
        logger.SetFruitNumber(playerAFruits, playerBFruits);
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
    }
}
