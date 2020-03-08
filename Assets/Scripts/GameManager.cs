using UnityEngine;
using UnityEngine.Video;
using TMPro;

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
    private TextMeshProUGUI instructionsText;

    private GameType gameType = GameType.None;
    private bool prepareStart;
    private bool inExperiment;
    private int currentID;
    private int currentRound;
    private uint max_rounds;

    private uint param_playerAFruits, param_playerBFruits, param_speedA, param_speedB;
    private uint param_buriedA, param_buriedB;
    private bool param_commonCounter, param_endGameButton;
    private bool param_showInstructions;
    private ObjectAccessType param_lockAccess, param_shovelAccess;
    private ChestAccessType param_chestAccess;
    private string param_instruction1, param_instruction2, param_instruction3;
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
        instructionsText = instructionsPanel.transform.Find("Text").GetComponent<TextMeshProUGUI>();

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
            this.StartExperiment(
                10, 10, 50, 50, 25, 25, true, true,
                ObjectAccessType.BOTH, ObjectAccessType.BOTH, ChestAccessType.BOTH_FREE
                );
            experimentManager.ActivateCursors();
            currentRound = 1;
            round1.Play();
            timerController.StartTimer();
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
                // Si no ha de esperar al panel, activa los cursores y comienza cronómetro.
                if (!param_showInstructions)
                {
                    experimentManager.ActivateCursors();
                    inExperiment = true;
                    if (max_rounds != 1)
                        round1.Play();
                    timerController.StartTimer();
                }
            }
            else if (tutorial != null && tutorial.activeSelf)
            {
                tutorialManager.ActivateCursors();
                tutorialManager.InitializeTutorial();
            }
            prepareStart = false;
        }

        // Tecla ESC configurada para que los experimentadores interumpan el experimento.
        if (inExperiment)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                this.EndExperiment();
        }

    }

    public void AcceptInstructions()
    {
        // Función llamada por el panel de instrucciones al ser aceptado.
        instructionsPanel.SetActive(false);
        if (currentRound == 1)
        {
            inExperiment = true;
            if (max_rounds != 1)
                round1.Play();
        }
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
        experimentManager.ActivateCursors();
    }

    // MainMenu llama a StartExperiment o comienza automáticamente en configuración "debug".
    public void StartExperiment(
        uint playerAFruits, uint playerBFruits, uint buriedA, uint buriedB, uint speedA, uint speedB, 
        bool commonCounter, bool endGameButton,
        ObjectAccessType lockAccess, ObjectAccessType shovelAccess, ChestAccessType chestAccess,
        uint rounds = 3, bool show_instructions = false, 
        string instructions1 = "", string instructions2 = "", string instructions3 = ""
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
            playerAFruits, playerBFruits, buriedA, buriedB, speedA, speedB,
            commonCounter, endGameButton,
            lockAccess, shovelAccess, chestAccess,
            path, currentID, currentRound
            );

        param_playerAFruits = playerAFruits;
        param_playerBFruits = playerBFruits;
        param_buriedA = buriedA;
        param_buriedB = buriedB;
        param_speedA = speedA;
        param_speedB = speedB;
        param_commonCounter = commonCounter;
        param_endGameButton = endGameButton;
        param_lockAccess = lockAccess;
        param_shovelAccess = shovelAccess;
        param_chestAccess = chestAccess;
        param_showInstructions = show_instructions;
        param_instruction1 = instructions1;
        param_instruction2 = instructions2;
        param_instruction3 = instructions3;

        instructionsPanel.SetActive(show_instructions);
        if (show_instructions)
        {
            string text = "<color=blue>Instrucciones:<color=black>\n\n";
            instructionsText.SetText(text + param_instruction1);
            experimentManager.DeactivateCursors();
        }

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
        timerController.StopTimer();
        experimentManager.DeactivateCursors();
        parallax.MoveUp();

        if (gameType == GameType.Level)
        {
            experimentManager.StopExperiment();
            experiment.SetActive(false);

            hudFader.ResetFade();
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
                param_playerAFruits, param_playerBFruits,
                param_buriedA, param_buriedB, param_speedA, param_speedB,
                param_commonCounter, param_endGameButton,
                param_lockAccess, param_shovelAccess, param_chestAccess,
                path, currentID, currentRound
                );

            // Si muestra instrucciones, espera a que el panel llame a AcceptInstructions(),
            if (param_showInstructions)
            {
                string text = "<color=blue>Instrucciones:<color=black>\n\n";
                if (currentRound == 2)
                    instructionsText.SetText(text + param_instruction2);
                if (currentRound == 3)
                    instructionsText.SetText(text + param_instruction3);

                instructionsPanel.SetActive(true);
                experimentManager.DeactivateCursors();
            }
            else // en caso contrario muestra animación y inicia cronómetro.
            {
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
    }

    public void EndTutorial()
    {
        tutorialManager.DeactivateCursors();
        this.EndGame();
    }
}
