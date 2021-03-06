﻿using System.Collections;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Administrador del tutorial.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject tutorial1, tutorial2, tutorial3;
    public GameObject fruitA, fruitB;
    public GameObject chestA, chestB;
    public GameObject playerACursor, playerBCursor;
    public bool debug = false;

    private GameObject fruitAInstance, fruitBInstance;
    private ManyCursorController playerACursorController, playerBCursorController;
    private GameManager gameManagerScript;
    private Vector2 playerACursorPos, playerBCursorPos;
    private Vector2 fruitAPos, fruitBPos;
    private int tutorialPhase;
    private VideoPlayer tutorial1Video, tutorial2Video, tutorial3Video;
    private FruitController fruitAController, fruitBController;
    private ChestController chestAController, chestBController;

    void Awake()
    {
        tutorial1Video = tutorial1.GetComponent<VideoPlayer>();
        tutorial2Video = tutorial2.GetComponent<VideoPlayer>();
        tutorial3Video = tutorial3.GetComponent<VideoPlayer>();
        chestAController = chestA.GetComponentInChildren<ChestController>();
        chestBController = chestB.GetComponentInChildren<ChestController>();

        if(gameManager != null)
            gameManagerScript = gameManager.GetComponent<GameManager>();

        playerACursorController = playerACursor.GetComponent<ManyCursorController>();
        playerBCursorController = playerBCursor.GetComponent<ManyCursorController>();

        playerACursorPos = playerACursor.transform.position;
        playerBCursorPos = playerBCursor.transform.position;

        fruitAPos = new Vector2(-4.945538f, -0.9905443f);
        fruitBPos = new Vector2(3.511462f, -0.9905443f);

        fruitAInstance = null;
        fruitBInstance = null;
    }

    private void Start()
    {
        if (debug)
        {
            ActivateCursors();
            InitializeTutorial();
        }
    }

    void Update()
    {
        // Tecla para pasar la fase de tutorial.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (tutorialPhase == 3)
            {
                chestAController.SetScore(0);
                chestBController.SetScore(0);

                if (fruitAInstance != null)
                    Destroy(fruitAInstance);
                if (fruitBInstance != null)
                    Destroy(fruitBInstance);

                tutorialPhase = 0;

                if (gameManager != null)
                    gameManagerScript.EndTutorial();
            }
            else
                this.NextPhase();
        }
    }

    public void ActivateCursors()
    {
        playerACursor.transform.position = playerACursorPos;
        playerBCursor.transform.position = playerBCursorPos;

        playerACursor.SetActive(true);
        playerBCursor.SetActive(true);

        playerACursorController.SetPlayableArea(Rect.MinMaxRect(-7.6f, -4.3f, -0.5f, 4.3f));
        playerBCursorController.SetPlayableArea(Rect.MinMaxRect(0.0f, -3.85f, 7.6f, 4.3f));
    }

    public void DeactivateCursors()
    {
        if (playerACursor.activeSelf)
            playerACursor.SetActive(false);

        if (playerBCursor.activeSelf)
            playerBCursor.SetActive(false);
    }

    private IEnumerator DelayNextPhase()
    {
        yield return new WaitForSeconds(1.0f);
        this.NextPhase();
    }

    private void NextPhase()
    {
        tutorialPhase++;

        if (tutorialPhase == 2)
        {
            tutorial1Video.Stop();
            tutorial2Video.Play();
        }

        if (tutorialPhase == 3)
        {
            tutorial2Video.Stop();
            tutorial3Video.Play();

            chestA.SetActive(true);
            chestB.SetActive(true);
        }
    }

    public void InitializeTutorial()
    {
        chestA.SetActive(false);
        chestB.SetActive(false);

        fruitAInstance = Instantiate(fruitA, fruitAPos, Quaternion.identity) as GameObject;
        fruitBInstance = Instantiate(fruitB, fruitBPos, Quaternion.identity) as GameObject;
        fruitAInstance.transform.SetParent(this.transform);
        fruitBInstance.transform.SetParent(this.transform);

        fruitAController = fruitAInstance.GetComponent<FruitController>();
        fruitBController = fruitBInstance.GetComponent<FruitController>();

        tutorialPhase = 1;
        tutorial1Video.Play();
    }
}
