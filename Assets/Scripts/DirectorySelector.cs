using System;
using System.IO;
using UnityEngine;
using TMPro;
using SFB;

public class DirectorySelector : MonoBehaviour
{
    public GameObject inputObject;
    private TMP_InputField inputField;
    private string defaultPath;

    void Awake()
    {
        inputField = inputObject.GetComponent<TMP_InputField>();
        defaultPath = PlayerPrefs.GetString("Path", "");
        if (defaultPath == "")
            defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "HuertosLog");
        inputField.text = defaultPath;
    }

    public void ChangeDirectory()
    {
        var path = StandaloneFileBrowser.OpenFolderPanel("Select Folder", "", false);
        if (path.Length != 0)
        {
            inputField.text = path[0];
            PlayerPrefs.SetString("Path", path[0]);
        }
    }
}
