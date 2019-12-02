using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Administra el grid donde se colocan los frutos a recolectar.
/// </summary>
public class BoardManager : MonoBehaviour
{
    [Range(0, 1)]
    public float scale = 1.0f;
    public GameObject fruitPrefab;

    private uint fruitNumber;
    private uint buriedPercentage;
    private uint columns; // Por defecto son 10x10
    private uint rows;
    private List<Vector2> gridPositions = new List<Vector2>();
    private string owner;
    private string m_logpath;
    private bool save_log = true;

    private void Awake()
    {
        if (fruitPrefab.name == "Player Fruit")
            owner = "PlayerA";
        else
            owner = "PlayerB";
    }

    public int CountFruits()
    {
        return this.transform.childCount;
    }

    void InitialiseList()
    {
        gridPositions.Clear();

        for (uint y = rows; y > 0; y--)
        {
            for (uint x = 0; x < columns; x++)
            {
                gridPositions.Add(new Vector2(x, y));
            }
        }
    }

    Vector2 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector2 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    public void SetUpExperiment(uint nrows, uint ncolumns, uint nfruits, uint buriedPercent)
    {
        buriedPercentage = buriedPercent;
        rows = nrows;
        columns = ncolumns;
        fruitNumber = nfruits;
        save_log = false;

        InitialiseList();
        DestroyChildren();
        BoardSetup();
    }

    public void SetUpExperiment(uint nrows, uint ncolumns, uint nfruits, uint buriedPercent, string logpath)
    {
        buriedPercentage = buriedPercent;
        rows = nrows;
        columns = ncolumns;
        fruitNumber = nfruits;
        m_logpath = logpath;
        save_log = true;

        InitialiseList();
        DestroyChildren();
        BoardSetup();
    }

    void BoardSetup()
    {
        FruitController fruitController;
        FruitLogger fruitLogger;

        int buriedNumber = Mathf.FloorToInt((buriedPercentage / 100.0f) * fruitNumber);

        // Coloca los frutos en posiciones aleatorias dentro del grid.
        for (int i = 0; i < fruitNumber; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject instance =
                        Instantiate(
                            fruitPrefab,
                            scale * randomPosition + this.transform.position, 
                            Quaternion.identity
                            ) as GameObject;

            if (i < buriedNumber)
            {
                fruitController = instance.GetComponent<FruitController>();
                fruitController.SetBuried(true);
            }

            if (save_log)
            {
                fruitLogger = instance.GetComponent<FruitLogger>();
                fruitLogger.SetFruitID(owner, i + 1);
                fruitLogger.SetPath(m_logpath);
            }
            instance.transform.SetParent(this.transform);
        }
    }

    private void DestroyChildren()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
