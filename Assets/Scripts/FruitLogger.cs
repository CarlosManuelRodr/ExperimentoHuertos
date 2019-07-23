using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Logger de la posición de los frutos.
/// </summary>
public class FruitLogger : MonoBehaviour
{
    private string m_path = "", m_name = "";
    private CsvExport log;

    void Awake()
    {
        m_name = "Fruit";
    }

    public void SetFruitID(string owner, Int32 number)
    {
        log = new CsvExport();
        m_name = owner + "_" + number.ToString();
    }

    public void SetPath(string path)
    {
        m_path = path;
    }

    public void Log(Vector2 position, string selector)
    {
        log.AddRow();
        log["Date"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
        log["Position"] = position;
        log["Selector"] = selector;
    }

    public void Save()
    {
        if (log != null)
        {
            string outputPath = Path.Combine(m_path, "fruit_" + m_name + ".csv");
            log.ExportToFile(outputPath, false);
        }
    }

    public void Clear()
    {
        log = new CsvExport();
    }
}
