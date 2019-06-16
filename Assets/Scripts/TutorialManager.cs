using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    public GameObject checkA, checkB;
    public GameObject tutorial1, tutorial2, tutorial3;
    public GameObject fruitA, fruitB;
    public GameObject chestA, chestB;

    private int  tutorialPhase;
    private VideoPlayer tutorial1Video, tutorial2Video, tutorial3Video;
    private FruitController fruitAController, fruitBController;
    private ChestController chestAController, chestBController;
    private bool checkedA, checkedB;

    void Start()
    {
        tutorial1Video = tutorial1.GetComponent<VideoPlayer>();
        tutorial2Video = tutorial2.GetComponent<VideoPlayer>();
        tutorial3Video = tutorial3.GetComponent<VideoPlayer>();
        fruitAController = fruitA.GetComponent<FruitController>();
        fruitBController = fruitB.GetComponent<FruitController>();
        chestAController = chestA.GetComponentInChildren<ChestController>();
        chestBController = chestB.GetComponentInChildren<ChestController>();

        this.InitializeTutorial();
    }

    void Update()
    {
        if (tutorialPhase == 1)
        {
            if (!checkedA && fruitAController.isHighlighted)
            {
                checkA.SetActive(true);
                checkA.GetComponent<AudioSource>().Play();
                checkedA = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }

            if (!checkedB && fruitBController.isHighlighted)
            {
                checkB.SetActive(true);
                checkB.GetComponent<AudioSource>().Play();
                checkedB = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }
        }

        if (tutorialPhase == 2)
        {
            if (!checkedA && fruitAController.isSelected)
            {
                checkA.SetActive(true);
                checkA.GetComponent<AudioSource>().Play();
                checkedA = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }

            if (!checkedB && fruitBController.isSelected)
            {
                checkB.SetActive(true);
                checkB.GetComponent<AudioSource>().Play();
                checkedB = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }
        }

        if (tutorialPhase == 3)
        {
            if (!checkedA && chestAController.score != 0)
            {
                checkA.SetActive(true);
                checkA.GetComponent<AudioSource>().Play();
                checkedA = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }

            if (!checkedB && chestBController.score != 0)
            {
                checkB.SetActive(true);
                checkB.GetComponent<AudioSource>().Play();
                checkedB = true;

                if (checkedA && checkedB)
                    StartCoroutine("DelayNextPhase");
            }
        }
    }
    private IEnumerator DelayNextPhase()
    {
        yield return new WaitForSeconds(1.0f);
        tutorialPhase++;
        checkedA = false;
        checkedB = false;
        checkA.SetActive(false);
        checkB.SetActive(false);
        Debug.Log("Phase: " + tutorialPhase);

        if (tutorialPhase == 2)
        {
            tutorial1Video.Stop();
            tutorial2Video.Play();
        }

        if (tutorialPhase == 3)
        {
            tutorial2Video.Stop();
            tutorial3Video.Play();
        }
    }

    public void InitializeTutorial()
    {
        checkedA = false;
        checkedB = false;

        Debug.Log("Started tutorial");
        tutorialPhase = 1;
        tutorial1Video.Play();
    }
}
