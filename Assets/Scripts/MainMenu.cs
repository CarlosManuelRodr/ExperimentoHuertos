﻿using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menú principal del juego.
/// </summary>
public class MainMenu : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject startScreen, initGame, options, about;
    public GameObject fruitSliderA, fruitSliderB, speedA, speedB;
    public GameObject percentBuriedA, percentBuriedB;
    public GameObject lockAccess, shovelAccess, chestAccess, commonCounter, endGameButton;
    public GameObject levelSelector;

    private AudioSource audioSource;
    private CanvasFaderScript canvasFader;
    private GameManager gameManagerScript;
    private MenuButtonController[] mbc;
    private LevelSelectController levelSelectController;

    private bool enabling;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canvasFader = GetComponent<CanvasFaderScript>();
        if (gameManager != null)
            gameManagerScript = gameManager.GetComponent<GameManager>();
        mbc = GetComponentsInChildren<MenuButtonController>();
        levelSelectController = levelSelector.GetComponent<LevelSelectController>();
    }

    public void DisableArrows()
    {
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("MenuArrow");
        foreach (GameObject obj in arrows)
        {
            obj.SetActive(false);
        }
    }

    public void OnStartButton()
    {
        DisableArrows();
        startScreen.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
        initGame.SetActive(true);
        audioSource.Play();
    }

    public void OnOptionButton()
    {
        DisableArrows();
        startScreen.SetActive(false);
        options.SetActive(true);
        initGame.SetActive(false);
        about.SetActive(false);
        audioSource.Play();
    }

    public void OnAboutButton()
    {
        DisableArrows();
        startScreen.SetActive(false);
        options.SetActive(false);
        initGame.SetActive(false);
        about.SetActive(true);
        audioSource.Play();
    }

    public void OnBackButton()
    {
        DisableArrows();
        startScreen.SetActive(true);
        options.SetActive(false);
        initGame.SetActive(false);
        about.SetActive(false);

        if (options.activeSelf)
            PlayerPrefs.Save();
    }

    public void OnInitButton()
    {
        DisableArrows();

        // Los niveles por defecto se definen en el archivo levels.xml.
        if (levelSelectController.GetSelectedLevelType() == LevelType.DEFAULT)
        {
            LevelData level = levelSelectController.GetSelectedLevel();
            if (gameManager != null)
            {
                canvasFader.SetFadeType(CanvasFaderScript.eFadeType.fade_out);
                canvasFader.StartFading();
                gameManagerScript.StartExperiment(
                    level.fruitsA, level.fruitsB,
                    level.buriedA, level.buriedB,
                    level.speedA, level.speedB,
                    level.commonCounter, level.endGameButton,
                    level.lockAccess, level.shovelAccess, level.chestAccess,
                    3, level.showInstructions, level.instruction1, level.instruction2, level.instruction3
                    );
                this.EnableMenu(false);
            }
            else
                Debug.LogError("No game manager found");
        }
        else if (levelSelectController.GetSelectedLevelType() == LevelType.CUSTOM) // Un nivel personalizado se define en la GUI.
        {
            // Recibe valores de configuración de elementos de interfaz gráfica.
            Slider fruitSliderAScript, fruitSliderBScript, speedAScript, speedBScript;
            Slider buriedASliderScript, buriedBSliderScript;
            Toggle commonCounterScript, endGameButtonScript;
            TMP_Dropdown lockAccessDropdown, shovelAccessDropdown, chestAccessDropdown;

            lockAccessDropdown = lockAccess.GetComponent<TMP_Dropdown>();
            shovelAccessDropdown = shovelAccess.GetComponent<TMP_Dropdown>();
            chestAccessDropdown = chestAccess.GetComponent<TMP_Dropdown>();

            ObjectAccessType lockAccessValue = ObjectAccessType.NONE;
            switch (lockAccessDropdown.value)
            {
                case 0:
                    lockAccessValue = ObjectAccessType.BOTH;
                    break;
                case 1:
                    lockAccessValue = ObjectAccessType.NONE;
                    break;
                case 2:
                    lockAccessValue = ObjectAccessType.ONLY_A;
                    break;
                case 3:
                    lockAccessValue = ObjectAccessType.ONLY_B;
                    break;
                default:
                    lockAccessValue = ObjectAccessType.BOTH;
                    break;
            }
            ObjectAccessType shovelAccessValue = ObjectAccessType.NONE;
            switch (shovelAccessDropdown.value)
            {
                case 0:
                    shovelAccessValue = ObjectAccessType.BOTH;
                    break;
                case 1:
                    shovelAccessValue = ObjectAccessType.NONE;
                    break;
                case 2:
                    shovelAccessValue = ObjectAccessType.ONLY_A;
                    break;
                case 3:
                    shovelAccessValue = ObjectAccessType.ONLY_B;
                    break;
                default:
                    shovelAccessValue = ObjectAccessType.BOTH;
                    break;
            }
            ChestAccessType chestAccessValue = ChestAccessType.BOTH_FREE;
            switch (chestAccessDropdown.value)
            {
                case 0:
                    chestAccessValue = ChestAccessType.BOTH_FREE;
                    break;
                case 1:
                    chestAccessValue = ChestAccessType.MUTUAL_BLOCK;
                    break;
                case 2:
                    chestAccessValue = ChestAccessType.A_FREE;
                    break;
                case 3:
                    chestAccessValue = ChestAccessType.B_FREE;
                    break;
                default:
                    chestAccessValue = ChestAccessType.BOTH_FREE;
                    break;
            }

            fruitSliderAScript = fruitSliderA.GetComponentInChildren<Slider>();
            fruitSliderBScript = fruitSliderB.GetComponentInChildren<Slider>();
            speedAScript = speedA.GetComponentInChildren<Slider>();
            speedBScript = speedB.GetComponentInChildren<Slider>();
            commonCounterScript = commonCounter.GetComponent<Toggle>();
            endGameButtonScript = endGameButton.GetComponent<Toggle>();
            buriedASliderScript = percentBuriedA.GetComponent<Slider>();
            buriedBSliderScript = percentBuriedB.GetComponent<Slider>();

            if (gameManager != null)
            {
                canvasFader.SetFadeType(CanvasFaderScript.eFadeType.fade_out);
                canvasFader.StartFading();
                gameManagerScript.StartExperiment(
                    (uint)fruitSliderAScript.value, (uint)fruitSliderBScript.value,
                    (uint)buriedASliderScript.value, (uint)buriedBSliderScript.value,
                    (uint)speedAScript.value, (uint)speedBScript.value,
                    commonCounterScript.isOn, endGameButtonScript.isOn,
                    lockAccessValue, shovelAccessValue, chestAccessValue,
                    3, false
                    );
                this.EnableMenu(false);
            }
            else
                Debug.LogError("No game manager found");
        }
        else if (levelSelectController.GetSelectedLevelType() == LevelType.TUTORIAL)
        {
            Debug.Log("Level type: Tutorial");
            if (gameManager != null)
            {
                canvasFader.SetFadeType(CanvasFaderScript.eFadeType.fade_out);
                canvasFader.StartFading();
                this.EnableMenu(false);
                gameManagerScript.StartTutorial();
            }
            else
                Debug.LogError("No game manager found");
        }
        else if (levelSelectController.GetSelectedLevelType() == LevelType.TEST_GAME)
        {
            if (gameManager != null)
            {
                canvasFader.SetFadeType(CanvasFaderScript.eFadeType.fade_out);
                canvasFader.StartFading();
                gameManagerScript.StartExperiment(
                    20, 20,
                    20, 20,
                    20, 20,
                    true, true,
                    ObjectAccessType.BOTH, ObjectAccessType.BOTH, ChestAccessType.BOTH_FREE, 
                    1, false
                    );
                this.EnableMenu(false);
            }
            else
                Debug.LogError("No game manager found");
        }
        audioSource.Play();
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    // Activa/desactiva menú con efecto "fade".
    public void EnableMenu(bool enable)
    {
        enabling = enable;

        if (enable)
        {
            this.transform.gameObject.SetActive(enable);
            canvasFader.SetFadeType(CanvasFaderScript.eFadeType.fade_in);
            canvasFader.StartFading();
        }
        else
        {
            foreach (MenuButtonController m in mbc)
                m.SetInteractable(false);
        }
    }

    // Función llamada al final del proceso de "fade".
    public void OnEndFading()
    {
        if (enabling)
        {
            foreach (MenuButtonController m in mbc)
                m.SetInteractable(true);
        }
        else
        {
            this.transform.gameObject.SetActive(false);
            enabling = false;
        }
    }
}
