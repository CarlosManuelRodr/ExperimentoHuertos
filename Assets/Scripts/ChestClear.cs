using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra animación del cofre vaciando los frutos recolectados.
/// </summary>
public class ChestClear : MonoBehaviour
{
    public GameObject chestA, chestB, chestCommon;
    public GameObject commonChestCounter;
    public GameObject fruitA, fruitB;
    public float speed = 1.0f;
    [Range(0, 1)]
    public float period = 0.1f;

    public bool isRunning { get { return running; } }

    private CommonChestVisuals commonChestVisuals;
    private AudioSource audioSource;
    private Text commonChestCounterText;
    private uint scoreA, scoreB;
    private uint commonChestScore;
    private bool running;

    private uint instanciatedA, instanciatedB;
    private Vector2 chestAPosition, chestBPosition, chestCommonPosition;

    void Start()
    {
        commonChestVisuals = chestCommon.GetComponentInChildren<CommonChestVisuals>();
        audioSource = GetComponent<AudioSource>();
        commonChestCounterText = commonChestCounter.GetComponent<Text>();
        running = false;
    }

    public void StartClear(uint pointsA, uint pointsB, uint pointsCommon)
    {
        instanciatedA = 0;
        instanciatedB = 0;
        commonChestScore = pointsCommon;
        commonChestCounterText.text = "Frutos: " + commonChestScore.ToString();
        chestAPosition = chestA.transform.position;
        chestBPosition = chestB.transform.position;
        chestCommonPosition = chestCommon.transform.position;
        scoreA = pointsA;
        scoreB = pointsB;
        commonChestVisuals.OpenChest();
        StartCoroutine("InstanciateFruits");
        running = true;
    }

    public void StopClear()
    {
        commonChestVisuals.CloseChest();
        StopCoroutine("InstanciateFruits");
        running = false;
    }

    void Update()
    {
        // Mueve los frutos
        float step = speed * Time.deltaTime;
        foreach (Transform child in transform)
        {
            if (child != this.transform)
            {
                child.position = Vector2.MoveTowards(child.position, chestCommonPosition, step);
            }
        }

        // Destruye los que ya han llegado
        foreach (Transform child in transform)
        {
            if (child != this.transform)
            {
                if ((Vector2) child.position == chestCommonPosition)
                {
                    Destroy(child.gameObject);
                    audioSource.Play();
                    commonChestScore++;
                    commonChestCounterText.text = "Frutos: " + commonChestScore.ToString();
                }
            }
        }
    }

    IEnumerator InstanciateFruits()
    {
        for (; ; )
        {
            if (instanciatedA < scoreA)
            {
                GameObject instance = Instantiate(fruitA, chestAPosition, Quaternion.identity) as GameObject;
                instance.transform.SetParent(this.transform);
                instance.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
                instanciatedA++;
            }

            if (instanciatedB < scoreB)
            {
                GameObject instance = Instantiate(fruitB, chestBPosition, Quaternion.identity) as GameObject;
                instance.transform.SetParent(this.transform);
                instance.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
                instanciatedB++;
            }

            if (commonChestScore == scoreA + scoreB)
            {
                yield return new WaitForSeconds(0.5f);
                running = false;
            }

            yield return new WaitForSeconds(period);
        }
    }
}
