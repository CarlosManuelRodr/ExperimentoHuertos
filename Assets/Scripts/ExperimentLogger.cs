using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logger de la configuración del experimento.
/// </summary>
public class ExperimentLogger : MonoBehaviour
{
    private string outputFolderPath;
    private string currentExperimentPath;
    private int experimentID;
    private uint playerAFruits, playerBFruits;
    private uint playerAScore, playerBScore;
    private int round = 1;
    private List<string> eventLog = new List<string>();

    public void Log(string logTxt)
    {
        eventLog.Add(DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + ", " + logTxt);
    }

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

    public void SetScore(uint aScore, uint bScore)
    {
        playerAScore = aScore;
        playerBScore = bScore;
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
        // Guarda archivo de resutados
        Debug.Log("Saving to: " + currentExperimentPath);
        string filePath = Path.Combine(currentExperimentPath, "resultados.txt");
        if (!File.Exists(filePath))
        {
            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine("Frutas huerto A: " + playerAFruits.ToString());
                sw.WriteLine("Frutas huerto B: " + playerBFruits.ToString());
                sw.WriteLine("Puntuacion jugador A: " + playerAScore.ToString());
                sw.WriteLine("Puntuacion jugador B: " + playerBScore.ToString());
            }
        }

        // Guarda archivo de eventos
        filePath = Path.Combine(currentExperimentPath, "eventos.txt");
        if (!File.Exists(filePath))
        {
            using (StreamWriter sw = File.CreateText(filePath))
            {
                foreach (string s in eventLog)
                    sw.WriteLine(s);
            }
        }
    }

    public string GetExperimentPath()
    {
        return currentExperimentPath;
    }

    // Calcula la ID del experimento actual sumando una unidad a la ID del último experimento realizado.
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
            // Crea directorio de salida de logs (por defecto "<userdir>/HuertosLog") en caso de que no exista.
            if (!Directory.Exists(outputFolderPath))
                Directory.CreateDirectory(outputFolderPath);

            // Crea directorio de ronda.
            currentExperimentPath = Path.Combine(
                outputFolderPath, 
                "Experimento_" + experimentID.ToString() + "_Ronda_" + round.ToString()
                );
            Directory.CreateDirectory(currentExperimentPath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error preparing path. The process failed: ");
            Debug.LogError(e.ToString());
        }
    }
}