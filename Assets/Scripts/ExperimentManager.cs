﻿using UnityEngine;

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
    public GameObject shovelA;
    public GameObject shovelB;

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
    private LockController lockAController, lockBController;
    private ShovelController shovelAController, shovelBController;

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
        lockAController = lockA.GetComponent<LockController>();
        lockBController = lockB.GetComponent<LockController>();
        shovelAController = shovelA.GetComponent<ShovelController>();
        shovelBController = shovelB.GetComponent<ShovelController>();

        playerACursorPos = new Vector2(-2.290669f, -5.53f);
        playerBCursorPos = new Vector2(-0.02999986f, -5.53f);

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

    void SaveCursorLog()
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

        // Configura acceso a candado y a los huertos.
        switch (lockAccess)
        {
            case ObjectAccessType.BOTH:
                lockAController.MakeActive();
                lockBController.MakeActive();
                cursorAScript.fruitsAccess = CanInteract.PlayerA;
                cursorBScript.fruitsAccess = CanInteract.PlayerB;
                break;
            case ObjectAccessType.NONE:
                lockAController.MakeInactive();
                lockBController.MakeInactive();
                cursorAScript.fruitsAccess = CanInteract.Both;
                cursorBScript.fruitsAccess = CanInteract.Both;
                break;
            case ObjectAccessType.ONLY_A:
                lockAController.MakeActive();
                lockBController.MakeInactive();
                cursorAScript.fruitsAccess = CanInteract.Both;
                cursorBScript.fruitsAccess = CanInteract.PlayerB;
                break;
            case ObjectAccessType.ONLY_B:
                lockAController.MakeInactive();
                lockBController.MakeActive();
                cursorAScript.fruitsAccess = CanInteract.PlayerA;
                cursorBScript.fruitsAccess = CanInteract.Both;
                break;
        }

        // Configura acceso a pala.
        switch (shovelAccess)
        {
            case ObjectAccessType.BOTH:
                shovelAController.MakeActive();
                shovelBController.MakeActive();
                break;
            case ObjectAccessType.NONE:
                shovelAController.MakeInactive();
                shovelBController.MakeInactive();
                break;
            case ObjectAccessType.ONLY_A:
                shovelAController.MakeActive();
                shovelBController.MakeInactive();
                break;
            case ObjectAccessType.ONLY_B:
                shovelAController.MakeInactive();
                shovelBController.MakeActive();
                break;
        }

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
            SaveCursorLog();
        }

        running = false;
        chestAController.SetScore(0);
        chestBController.SetScore(0);
        cursorAScript.ChangeMode(CursorMode.HandMode);
        cursorBScript.ChangeMode(CursorMode.HandMode);
        shovelAController.ChangeMode(CursorMode.HandMode);
        shovelBController.ChangeMode(CursorMode.HandMode);
        lockAController.SetLockMode(LockStatus.Locked);
        lockBController.SetLockMode(LockStatus.Locked);
    }
}
