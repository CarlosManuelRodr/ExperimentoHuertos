using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Range(0, 1)]
    public float scale = 1.0f;
    public GameObject fruitPrefab;

    private uint fruitNumber;
    private uint columns; // Default is 10x10
    private uint rows;
    private List<Vector2> gridPositions = new List<Vector2>();

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

        InitialiseList();
        DestroyChildren();
        RandomBoardSetup();
    }

    void RandomBoardSetup()
    {
        for (int i = 0; i < fruitNumber; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject instance =
                        Instantiate(
                            fruitPrefab,
                            scale * randomPosition + this.transform.position, 
                            Quaternion.identity
                            ) as GameObject;
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
