using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject startScreen, initGame, options;
    public GameObject fruitSliderA, fruitSliderB, speedA, speedB;
    public GameObject simulateB, enableLock, commonCounter, endGameButton;

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
        initGame.SetActive(true);
        audioSource.Play();
    }

    public void OnOptionButton()
    {
        DisableArrows();
        startScreen.SetActive(false);
        options.SetActive(true);
        initGame.SetActive(false);
        audioSource.Play();
    }

    public void OnBackButton()
    {
        DisableArrows();
        startScreen.SetActive(true);
        options.SetActive(false);
        initGame.SetActive(false);

        if (options.activeSelf)
            PlayerPrefs.Save();
    }

    public void OnInitButton()
    {
        Slider fruitSliderAScript, fruitSliderBScript, speedAScript, speedBScript;
        Toggle simulateBScript, enableLockScript, commonCounterScript, endGameButtonScript;
        fruitSliderAScript = fruitSliderA.GetComponentInChildren<Slider>();
        fruitSliderBScript = fruitSliderB.GetComponentInChildren<Slider>();
        speedAScript = speedA.GetComponentInChildren<Slider>();
        speedBScript = speedA.GetComponentInChildren<Slider>();
        simulateBScript = simulateB.GetComponent<Toggle>();
        enableLockScript = enableLock.GetComponent<Toggle>();
        commonCounterScript = commonCounter.GetComponent<Toggle>();
        endGameButtonScript = endGameButton.GetComponent<Toggle>();

        DisableArrows();

        if (gameManager != null)
        {
            canvasFader.SetFadeType(CanvasFaderScript.eFadeType.fade_out);
            canvasFader.StartFading();
            gameManagerScript.StartExperiment(
                (uint) fruitSliderAScript.value, (uint) fruitSliderBScript.value,
                (uint) speedAScript.value, (uint) speedBScript.value,
                simulateBScript.isOn, enableLockScript.isOn,
                commonCounterScript.isOn, endGameButtonScript.isOn
                );
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
