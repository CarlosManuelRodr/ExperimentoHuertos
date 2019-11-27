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

    public void SetUpExperiment(uint nrows, uint ncolumns, uint nfruits)
    {
        rows = nrows;
        columns = ncolumns;
        fruitNumber = nfruits;
        save_log = false;

        InitialiseList();
        DestroyChildren();
        RandomBoardSetup();
    }

    public void SetUpExperiment(uint nrows, uint ncolumns, uint nfruits, string logpath)
    {
        rows = nrows;
        columns = ncolumns;
        fruitNumber = nfruits;
        m_logpath = logpath;
        save_log = true;

        InitialiseList();
        DestroyChildren();
        RandomBoardSetup();
    }

    void RandomBoardSetup()
    {
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
            if (save_log)
            {
                instance.GetComponent<FruitLogger>().SetFruitID(owner, i + 1);
                instance.GetComponent<FruitLogger>().SetPath(m_logpath);
            }
            instance.transform.SetParent(this.transform);
        }
    }

    void SquareBoardSetup()
    {
        uint fruitsInstanciated = 0;

        for (uint y = rows; y > 0; y--)
        {
            for (uint x = 0; x < columns; x++)
            {
                if (fruitsInstanciated < fruitNumber)
                {
                    GameObject instance =
                        Instantiate(fruitPrefab, new Vector3(scale * x, scale * y, 0f) + this.transform.position, Quaternion.identity) as GameObject;

                    if (save_log)
                    {
                        instance.GetComponent<FruitLogger>().SetPath(m_logpath);
                        instance.GetComponent<FruitLogger>().SetFruitID(owner, (int)fruitsInstanciated);
                    }
                    instance.transform.SetParent(this.transform);
                    fruitsInstanciated++;
                }
                else
                    break;
            }
        }
    }

    private void DestroyChildren()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
