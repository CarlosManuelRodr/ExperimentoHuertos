using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject startButton, optionsButton, quitButton, startConfig;
    public GameObject fruitSliderA, fruitSliderB, aiToggle;

    private Slider fruitSliderAScript, fruitSliderBScript;
    private Toggle aiToggleScript;
    private AudioSource audioSource;
    private CanvasFaderScript canvasFader;
    private GameManager gameManagerScript;
    private MenuButtonController[] mbc;


    private bool enabling;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        canvasFader = GetComponent<CanvasFaderScript>();
        if (gameManager != null)
            gameManagerScript = gameManager.GetComponent<GameManager>();
        mbc = GetComponentsInChildren<MenuButtonController>();

        fruitSliderAScript = fruitSliderA.GetComponent<Slider>();
        fruitSliderBScript = fruitSliderB.GetComponent<Slider>();
        aiToggleScript = aiToggle.GetComponent<Toggle>();
    }

    public void OnStartButton()
    {
        startButton.SetActive(false);
        optionsButton.SetActive(false);
        quitButton.SetActive(false);
        startConfig.SetActive(true);
        audioSource.Play();
    }

    public void OnBackButton()
    {
        startConfig.SetActive(false);
        startButton.SetActive(true);
        optionsButton.SetActive(true);
        quitButton.SetActive(true);
    }

    public void OnInitButton()
    {
        if (gameManager != null)
        {
            canvasFader.SetFadeType(CanvasFaderScript.eFadeType.fade_out);
            canvasFader.StartFading();
            gameManagerScript.StartExperiment((uint) fruitSliderAScript.value, (uint) fruitSliderBScript.value, aiToggleScript.isOn);
            this.EnableMenu(false);
        }
        audioSource.Play();
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

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
