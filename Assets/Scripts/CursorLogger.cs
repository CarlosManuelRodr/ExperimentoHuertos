using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Logger que guarda la posición del cursor.
/// </summary>
public class CursorLogger : MonoBehaviour
{
    private string m_path = "", m_name = "";
    private CsvExport log;

    void Awake()
    {
        m_name = "DefaultCursor";
    }

    public void SetCursorID(string name)
    {
        log = new CsvExport();
        m_name = name;
    }

    public void SetPath(string path)
    {
        m_path = path;
    }

    public void Log(Vector2 position, bool selecting)
    {
        log.AddRow();
        log["Date"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
        log["Position"] = position;
        log["Selecting"] = selecting;
    }

    public void Log(Vector2 position, bool selecting, Selectable access)
    {
        log.AddRow();
        log["Date"] = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
        log["Position"] = position;
        log["Selecting"] = selecting;

        string accessStr = "";
        switch (access)
        {
            case Selectable.PlayerA:
                accessStr = "PlayerA";
                break;
            case Selectable.PlayerB:
                accessStr = "PlayerB";
                break;
            case Selectable.Both:
                accessStr = "Both";
                break;
        }

        log["Access"] = accessStr;
    }

    public void Save()
    {
        string outputPath = Path.Combine(m_path, "cursordata_" + m_name + ".csv");
        log.ExportToFile(outputPath, true);
    }

    public void Clear()
    {
        log = new CsvExport();
    }
}