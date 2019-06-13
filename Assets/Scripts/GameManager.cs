using UnityEngine;
using UnityEngine.Video;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject hud;
    public GameObject experiment;
    public GameObject eventSystem;
    public GameObject round1Video, round2Video, round3Video;
    public GameObject pathInput;
    public bool debug = false;
    public bool debugAi = false;
    public uint rounds = 3;

    private ExperimentManager experimentManager;
    private CanvasFaderScript hudFader;
    private Parallax parallax;
    private MainMenu mainMenuScript;
    private VideoPlayer round1, round2, round3;
    private Jukebox jukebox;
    private SpriteRenderer mouseWarning;

    private bool prepareStart;
    private bool inExperiment;
    private int currentRound;

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
        mouseWarning = GetComponent<SpriteRenderer>();

        int musicVolume = PlayerPrefs.GetInt("Volume", -1);
        if (musicVolume == -1)
            musicVolume = 100;
        jukebox.volume = (float) musicVolume;

        if (mainMenu != null)
        {
            mainMenuScript = mainMenu.GetComponent<MainMenu>();
            hud.SetActive(false);
        }

        experiment.SetActive(false);
        experimentManager = experiment.GetComponent<ExperimentManager>();

        if (debug)
        {
            this.StartExperiment(70, 30, 50, 50, debugAi, true, true, true);
            experimentManager.ActivateCursors();
        }

        prepareStart = false;
        inExperiment = false;

        if (ManyMouseWrapper.MouseCount < 2)
        {
            eventSystem.SetActive(false);
            mainMenu.SetActive(false);
            mouseWarning.enabled = true;
            jukebox.volume = 0.0f;
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
            experimentManager.ActivateCursors();
            prepareStart = false;
            inExperiment = true;
            round1.Play();
        }

        if (inExperiment)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                this.EndExperiment();
        }
    }

    public void StartExperiment(
        uint playerAFruits, uint playerBFruits, uint speedA, uint speedB, 
        bool simulateB, bool enableLock, bool commonCounter, bool endGameButton
        )
    {
        currentRound = 1;
        path = pathInput.GetComponent<TMP_InputField>().text;

        parallax.MoveDown();
        experiment.SetActive(true);
        experimentManager.InitializeExperiment(
            playerAFruits, playerBFruits, speedA, speedB, 
            simulateB, enableLock, commonCounter, endGameButton,
            path, currentRound
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
    }

    public void EndGame()
    {
        experimentManager.DeactivateCursors();
        parallax.MoveUp();
        experimentManager.StopExperiment();
        experiment.SetActive(false);

        hudFader.SetFadeType(CanvasFaderScript.eFadeType.fade_out);
        hudFader.StartFading();
        hud.SetActive(false);

        if (mainMenu != null)
            mainMenuScript.EnableMenu(true);

        inExperiment = false;
    }

    public void EndExperiment()
    {
        if (currentRound == rounds)
        {
            this.EndGame();
        }
        else
        {
            currentRound++;

            experimentManager.StopExperiment();

            experimentManager.InitializeExperiment(
                param_playerAFruits, param_playerBFruits, param_speedA, param_speedB,
                param_simulateB, param_enableLock, param_commonCounter, param_endGameButton,
                path, currentRound
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
