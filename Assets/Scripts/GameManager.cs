﻿using UnityEngine;
using UnityEngine.Video;

enum GameType
{
    None,
    Level,
    Tutorial
}

public class GameManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject hud;
    public GameObject experiment;
    public GameObject tutorial;
    public GameObject eventSystem;
    public GameObject round1Video, round2Video, round3Video;
    public GameObject chestClearer;
    public GameObject errorMenu;
    public bool debug = false;
    public bool debugAi = false;
    public uint rounds = 3;

    private ExperimentManager experimentManager;
    private TutorialManager tutorialManager;
    private CanvasFaderScript hudFader;
    private Parallax parallax;
    private MainMenu mainMenuScript;
    private VideoPlayer round1, round2, round3;
    private Jukebox jukebox;
    private ChestClear chestClearerScript;

    private GameType gameType = GameType.None;
    private bool prepareStart;
    private bool inExperiment;
    private bool clearing;
    private int currentID;
    private int currentRound;
    private uint totalScoreA, totalScoreB;

    private uint param_playerAFruits, param_playerBFruits, param_speedA, param_speedB;
    private bool param_simulateB, param_enableLock, param_commonCounter, param_endGameButton;
    private string path;

    void Start()
    {
        parallax = GetComponent<Parallax>();
        hudFader = hud.GetComponent<CanvasFaderScript>();
        round1 = round1Video.GetComponent<VideoPlayer>();
        round2 = round2Video.GetComponent<VideoPlayer>();
        round3 = round3Video.GetComponent<VideoPlayer>();
        jukebox = GetComponent<Jukebox>();
        chestClearerScript = chestClearer.GetComponent<ChestClear>();

        int musicVolume = PlayerPrefs.GetInt("Volume", -1);
        if (musicVolume == -1)
            musicVolume = 100;

        if (jukebox != null)
            jukebox.volume = (float) musicVolume;

        if (mainMenu != null)
        {
            mainMenuScript = mainMenu.GetComponent<MainMenu>();
            hud.SetActive(false);
        }

        experimentManager = experiment.GetComponent<ExperimentManager>();

        if (tutorial != null)
            tutorialManager = tutorial.GetComponent<TutorialManager>();

        if (debug)
        {
            this.StartExperiment(2, 1, 50, 50, debugAi, true, true, true);
            experimentManager.ActivateCursors();
            currentRound = 1;
            round1.Play();
        }

        prepareStart = false;
        inExperiment = false;
        clearing = false;
        totalScoreA = 0;
        totalScoreB = 0;

        if (eventSystem != null && errorMenu != null)
        {
            if (ManyMouseWrapper.MouseCount < 2)
            {
                eventSystem.SetActive(false);
                mainMenu.SetActive(false);
                jukebox.volume = 0.0f;
                errorMenu.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (eventSystem != null)
        {
            if (eventSystem.activeSelf && parallax.isMoving)
                eventSystem.SetActive(false);
            if (!eventSystem.activeSelf && !parallax.isMoving)
                eventSystem.SetActive(true);
        }

        if (prepareStart && !parallax.isMoving)
        {
            if (gameType == GameType.Level)
            {
                experimentManager.ActivateCursors();
                inExperiment = true;
                round1.Play();
                prepareStart = false;
            }
            else
            {
                if (tutorial != null && tutorial.activeSelf)
                {
                    tutorialManager.ActivateCursors();
                    tutorialManager.InitializeTutorial();
                    prepareStart = false;
                }
            }
        }

        if (inExperiment)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                this.EndExperiment();
        }

        if (clearing)
        {
            if (!chestClearerScript.isRunning)
            {
                this.EndGame();
                clearing = false;
                chestClearerScript.StopClear();
            }
        }
    }

    public void StartExperiment(
        uint playerAFruits, uint playerBFruits, uint speedA, uint speedB, 
        bool simulateB, bool enableLock, bool commonCounter, bool endGameButton
        )
    {
        currentRound = 1;
        path = DirectorySelector.GetSaveDirectory();
        currentID = ExperimentLogger.GetCurrentExperimentID(path);

        parallax.MoveDown();
        experiment.SetActive(true);
        experimentManager.InitializeExperiment(
            playerAFruits, playerBFruits, speedA, speedB, 
            simulateB, enableLock, commonCounter, endGameButton,
            path, currentID, currentRound
            );

        param_playerAFruits = playerAFruits;
        param_playerBFruits = playerBFruits;
        param_speedA = speedA;
        param_speedB = speedB;
        param_simulateB = simulateB;
        param_enableLock = enableLock;
        param_commonCounter = commonCounter;
        param_endGameButton = endGameButton;

        hud.SetActive(true);
        hudFader.SetFadeType(CanvasFaderScript.eFadeType.fade_in);
        hudFader.StartFading();
        prepareStart = true;
        gameType = GameType.Level;
    }

    public void StartTutorial()
    {
        Debug.Log("Tutorial started");
        tutorial.SetActive(true);
        prepareStart = true;
        parallax.MoveDown();
        gameType = GameType.Tutorial;
    }

    public void EndGame()
    {
        experimentManager.DeactivateCursors();
        parallax.MoveUp();
        if (gameType == GameType.Level)
        {
            experimentManager.StopExperiment();
            experiment.SetActive(false);

            hudFader.SetFadeType(CanvasFaderScript.eFadeType.fade_out);
            hudFader.StartFading();
            hud.SetActive(false);

            inExperiment = false;
            totalScoreA = 0;
            totalScoreB = 0;
        }
        else
        {
            tutorial.SetActive(false);
        }

        if (mainMenu != null)
            mainMenuScript.EnableMenu(true);

        gameType = GameType.None;
    }

    public void EndExperiment()
    {
        if (!clearing)
        {
            experimentManager.SaveCursorLog();

            totalScoreA += experimentManager.scoreA;
            totalScoreB += experimentManager.scoreB;

            if (currentRound == rounds)
            {
                clearing = true;
                chestClearerScript.StartClear(totalScoreA, totalScoreB);
            }
            else
            {
                currentRound++;

                experimentManager.StopExperiment();

                experimentManager.InitializeExperiment(
                    param_playerAFruits, param_playerBFruits, param_speedA, param_speedB,
                    param_simulateB, param_enableLock, param_commonCounter, param_endGameButton,
                    path, currentID, currentRound
                    );

                if (currentRound == 2)
                {
                    round2.Play();
                }
                if (currentRound == 3)
                {
                    round3.Play();
                }
            }
        }
    }

    public void EndTutorial()
    {
        tutorialManager.DeactivateCursors();
        this.EndGame();
    }
}
