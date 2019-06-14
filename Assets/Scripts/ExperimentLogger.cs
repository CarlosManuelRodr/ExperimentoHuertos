using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class ExperimentLogger : MonoBehaviour
{
    private string outputFolderPath;
    private string currentExperimentPath;
    private int experimentID;
    private uint playerAFruits, playerBFruits;
    private uint playerAScore, playerBScore, commonChestScore;
    private int round = 1;

    public void SetDefaultPath()
    {
        outputFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "HuertosLog");
        this.PreparePath();
    }

    public void SetPath(string path)
    {
        outputFolderPath = path;
        this.PreparePath();
    }

    public void SetFruitNumber(uint fruitsA, uint fruitsB)
    {
        playerAFruits = fruitsA;
        playerBFruits = fruitsB;
    }

    public void SetScore(uint aScore, uint bScore, uint commonScore)
    {
        playerAScore = aScore;
        playerBScore = bScore;
        commonChestScore = commonScore;
    }

    public void SetRound(int nround)
    {
        round = nround;
    }

    public void SetExperimentID(int id)
    {
        experimentID = id;
    }

    public void Save()
    {
        Debug.Log("Saving to: " + currentExperimentPath);
        string filePath = Path.Combine(currentExperimentPath, "resultados.txt");
        if (!File.Exists(filePath))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine("Frutas huerto A: " + playerAFruits.ToString());
                sw.WriteLine("Frutas huerto B: " + playerBFruits.ToString());
                sw.WriteLine("Puntuacion jugador A: " + playerAScore.ToString());
                sw.WriteLine("Puntuacion jugador B: " + playerBScore.ToString());
                sw.WriteLine("Puntuacion cofre comun: " + commonChestScore.ToString());
            }
        }
    }

    public string GetExperimentPath()
    {
        return currentExperimentPath;
    }

    public static int GetCurrentExperimentID(string path)
    {
        int experimentID;

        if (!Directory.Exists(path))
        {
            experimentID = 1;
        }
        else
        {
            string[] dirs = Directory.GetDirectories(path, "Experimento_*", SearchOption.TopDirectoryOnly);

            if (dirs.Length != 0)
            {
                int[] experimentIds = new int[dirs.Length];

                for (int i = 0; i < dirs.Length; i++)
                {
                    int n;
                    string intString = dirs[i].Split('_')[1];

                    if (!Int32.TryParse(intString, out n))
                    {
                        n = -1;
                    }
                    experimentIds[i] = n;
                }

                experimentID = experimentIds.Max() + 1;
            }
            else
                experimentID = 1;
        }

        return experimentID;
    }

    void PreparePath()
    {
        try
        {
            if (!Directory.Exists(outputFolderPath))
                Directory.CreateDirectory(outputFolderPath);

            Debug.Log("Current experimentID: " + experimentID);
            Debug.Log("Current round: " + round);

            currentExperimentPath = Path.Combine(
                outputFolderPath, 
                "Experimento_" + experimentID.ToString() + "_Ronda_" + round.ToString()
                );
            Directory.CreateDirectory(currentExperimentPath);
        }
        catch (Exception e)
        {
            Debug.Log("Error preparing path. The process failed: ");
            Debug.Log(e.ToString());
        }
    }
}