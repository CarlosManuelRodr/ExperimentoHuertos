using UnityEngine;
using UnityEngine.Video;

enum GameType
{
    None,
    Level,
    Tutorial
}

/// <summary>
/// Gestiona la ejecución del juego. Presente durante toda la ejecución.
/// </summary>
public class GameManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject hud;
    public GameObject experiment;
    public GameObject tutorial;
    public GameObject eventSystem;
    public GameObject round1Video, round2Video, round3Video;
    public GameObject errorMenu;
    public GameObject timer;
    public GameObject instructionsPanel;
    public bool debug = false;

    private ExperimentManager experimentManager;
    private TutorialManager tutorialManager;
    private CanvasFaderScript hudFader;
    private Parallax parallax;
    private MainMenu mainMenuScript;
    private VideoPlayer round1, round2, round3;
    private Jukebox jukebox;
    private TimerController timerController;

    private GameType gameType = GameType.None;
    private bool prepareStart;
    private bool inExperiment;
    private int currentID;
    private int currentRound;
    private uint max_rounds;

    private uint param_playerAFruits, param_playerBFruits, param_speedA, param_speedB;
    private bool param_freeOrchard, param_enableLock, param_commonCounter, param_endGameButton;
    private string path;

    void Start()
    {
        // Adquiere referencias a componentes.
        parallax = GetComponent<Parallax>();
        hudFader = hud.GetComponent<CanvasFaderScript>();
        round1 = round1Video.GetComponent<VideoPlayer>();
        round2 = round2Video.GetComponent<VideoPlayer>();
        round3 = round3Video.GetComponent<VideoPlayer>();
        timerController = timer.GetComponent<TimerController>();
        jukebox = GetComponent<Jukebox>();

        // Carga configuración de volumen de las preferencias de usuario.
        int musicVolume = PlayerPrefs.GetInt("Volume", -1);
        if (musicVolume == -1)
            musicVolume = 100;
        if (jukebox != null)
            jukebox.volume = (float) musicVolume;

        // Por defecto el HUD está desactivado.
        if (mainMenu != null)
        {
            mainMenuScript = mainMenu.GetComponent<MainMenu>();
            hud.SetActive(false);
        }

        experimentManager = experiment.GetComponent<ExperimentManager>();

        if (tutorial != null)
            tutorialManager = tutorial.GetComponent<TutorialManager>();

        // Inicia automáticamente el experimento en la configuración debug.
        // Usado en escena de depuración "Experiment".
        if (debug)
        {
            this.StartExperiment(4, 3, 25, 25, false, true, true, true);
            experimentManager.ActivateCursors();
            currentRound = 1;
            round1.Play();
        }

        prepareStart = false;
        inExperiment = false;

        // Si no hay al menos dos mouse, genera pantalla de error.
        if (eventSystem != null && errorMenu != null)
        {
            if (ManyMouseWrapper.MouseCount < 2)
            {
                mainMenu.SetActive(false);
                jukebox.volume = 0.0f;
                errorMenu.SetActive(true);
            }
        }
    }

    void Update()
    {
        // Bloquea sistema de eventos si el parallax está en movimiento.
        if (eventSystem != null)
        {
            if (eventSystem.activeSelf && parallax.isMoving)
                eventSystem.SetActive(false);
            if (!eventSystem.activeSelf && !parallax.isMoving)
                eventSystem.SetActive(true);
        }

        // Cuando el parallax deja de moverse, activa el nivel.
        if (prepareStart && !parallax.isMoving)
        {
            if (gameType == GameType.Level)
            {
                experimentManager.ActivateCursors();
                inExperiment = true;
                if (max_rounds != 1)
                    round1.Play();
                prepareStart = false;
                timerController.StartTimer();
            }
            else if (tutorial != null && tutorial.activeSelf)
            {
                tutorialManager.ActivateCursors();
                tutorialManager.InitializeTutorial();
                prepareStart = false;
            }
        }

        // Tecla ESC configurada para que los experimentadores interumpan el experimento.
        if (inExperiment)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                this.EndExperiment();
        }
    }

    public void StartExperiment(
        uint playerAFruits, uint playerBFruits, uint speedA, uint speedB, 
        bool freeOrchard, bool enableLock, bool commonCounter, bool endGameButton,
        uint rounds = 3
        )
    {
        currentRound = 1;    // Comienza en ronda 1 de 3.
        max_rounds = rounds;
        path = DirectorySelector.GetSaveDirectory();                // Directorio de guardado configurable en opciones.
        currentID = ExperimentLogger.GetCurrentExperimentID(path);  // ID de experimento. Se calcula sumando uno a la última ID registrada en el log.

        // Inicia experimento y guarda configuración para rondas posteriores.
        parallax.MoveDown();
        experiment.SetActive(true);
        experimentManager.InitializeExperiment(
            playerAFruits, playerBFruits, speedA, speedB,
            freeOrchard, enableLock, commonCounter, endGameButton,
            path, currentID, currentRound
            );

        param_playerAFruits = playerAFruits;
        param_playerBFruits = playerBFruits;
        param_speedA = speedA;
        param_speedB = speedB;
        param_freeOrchard = freeOrchard;
        param_enableLock = enableLock;
        param_commonCounter = commonCounter;
        param_endGameButton = endGameButton;

        hud.SetActive(true);
        round1.Prepare();
        round2.Prepare();
        round3.Prepare();
        hudFader.SetFadeType(CanvasFaderScript.eFadeType.fade_in);
        hudFader.StartFading();
        prepareStart = true;
        gameType = GameType.Level;
    }

    public void StartTutorial()
    {
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
        if (currentRound == max_rounds)
        {
            this.EndGame();
        }
        else
        {
            // Reinicia experimento hasta llegar a la ronda 3.
            currentRound++;

            experimentManager.StopExperiment();
            experimentManager.InitializeExperiment(
                param_playerAFruits, param_playerBFruits, param_speedA, param_speedB,
                param_freeOrchard, param_enableLock, param_commonCounter, param_endGameButton,
                path, currentID, currentRound
                );

            if (currentRound == 2)
            {
                round1.Stop();
                round2.Play();
            }
            if (currentRound == 3)
            {
                round3.Play();
            }
            timerController.StartTimer();
        }
    }

    public void EndTutorial()
    {
        tutorialManager.DeactivateCursors();
        this.EndGame();
    }
}
