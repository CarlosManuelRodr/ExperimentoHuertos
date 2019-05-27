using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject hud;
    public GameObject experiment;
    public GameObject eventSystem;
    public bool debug = false;
    public bool debugAi = false;

    private ExperimentManager experimentManager;
    private CanvasFaderScript hudFader;
    private Parallax parallax;
    private MainMenu mainMenuScript;
    private bool prepareStart;

    void Start()
    {
        parallax = GetComponent<Parallax>();
        hudFader = hud.GetComponent<CanvasFaderScript>();

        if (mainMenu != null)
        {
            mainMenuScript = mainMenu.GetComponent<MainMenu>();
            hud.SetActive(false);
        }

        experiment.SetActive(false);
        experimentManager = experiment.GetComponent<ExperimentManager>();

        if (debug)
        {
            this.StartExperiment(70, 30, debugAi);
            experimentManager.ActivateCursors();
        }

        prepareStart = false;
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
        }
    }

    public void StartExperiment(uint playerAFruits, uint playerBFruits, bool aiPlayer = false)
    {
        parallax.MoveDown();
        experiment.SetActive(true);
        experimentManager.InitializeExperiment(playerAFruits, playerBFruits, aiPlayer);

        hud.SetActive(true);
        hudFader.SetFadeType(CanvasFaderScript.eFadeType.fade_in);
        hudFader.StartFading();
        prepareStart = true;
    }

    public void EndExperiment()
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
    }
}
