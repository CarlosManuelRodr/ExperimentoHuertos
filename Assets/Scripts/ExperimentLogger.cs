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

    public void Save()
    {
        string filePath = Path.Combine(currentExperimentPath, "configuracion.txt");
        if (!File.Exists(filePath))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine("Frutas Jugador A: " + playerAFruits.ToString());
                sw.WriteLine("Frutas Jugador B: " + playerBFruits.ToString());
            }
        }
    }

    public string GetExperimentPath()
    {
        return currentExperimentPath;
    }

    void PreparePath()
    {
        try
        {
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
                experimentID = 1;
            }
            else
            {
                string[] dirs = Directory.GetDirectories(outputFolderPath, "Experimento_*", SearchOption.TopDirectoryOnly);

                if (dirs.Length != 0)
                {
                    int[] experimentIds = new int[dirs.Length];

                    for (int i = 0; i < dirs.Length; i++)
                    {
                        int n;
                        string intString = dirs[i].Split('_').Last();

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

            currentExperimentPath = Path.Combine(outputFolderPath, "Experimento_" + experimentID.ToString());
            Directory.CreateDirectory(currentExperimentPath);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error preparing path. The process failed: {0}", e.ToString());
        }
    }
}