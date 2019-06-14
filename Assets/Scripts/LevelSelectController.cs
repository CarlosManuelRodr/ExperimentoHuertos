using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public struct LevelData
{
    public bool lockLevel;
    public string name;
    public int fruitsA, fruitsB;
    public int speedA, speedB;
    public bool simulateB, enableLock;
    public bool commonCounter, endGameButton;
}

public class LevelSelectController : MonoBehaviour
{
    public GameObject moreOptions;
    public GameObject fruitsA, fruitsB, speedA, speedB;
    public GameObject simulateB, enableLock, commonCounter, endGameButton;

    private TMP_Dropdown dropdown;
    private List<LevelData> levels;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        levels = GetLevelData();
        dropdown.AddOptions(GetLevelNames(levels));
        OnLevelSelected();
    }

    public static void SetSliderValue(GameObject slider, int value)
    {
        Slider componentSlider = slider.GetComponentInChildren<Slider>();
        GameObject number = slider.transform.Find("Number").gameObject;
        TextMeshProUGUI componentTxt = number.GetComponentInChildren<TextMeshProUGUI>();
        componentSlider.value = value;
        componentTxt.text = Convert.ToString(value);
    }

    public static void SetSliderInteractable(GameObject slider, bool value)
    {
        Slider componentSlider = slider.GetComponentInChildren<Slider>();
        componentSlider.interactable = value;
    }

    public static void SetToggleValue(GameObject toggle, bool value)
    {
        Toggle componentToggle = toggle.GetComponent<Toggle>();
        componentToggle.isOn = value;
    }

    public static void SetToggleInteractable(GameObject toggle, bool value)
    {
        Toggle componentToggle = toggle.GetComponent<Toggle>();
        componentToggle.interactable = value;
    }

    public void OnLevelSelected()
    {
        LevelData ld = levels[dropdown.value];
        SetSliderValue(fruitsA, ld.fruitsA);
        SetSliderValue(fruitsB, ld.fruitsB);
        SetSliderValue(speedA, ld.speedA);
        SetSliderValue(speedB, ld.speedB);
        SetToggleValue(simulateB, ld.simulateB);
        SetToggleValue(enableLock, ld.enableLock);
        SetToggleValue(commonCounter, ld.commonCounter);
        SetToggleValue(endGameButton, ld.endGameButton);

        SetSliderInteractable(fruitsA, ld.lockLevel);
        SetSliderInteractable(fruitsB, ld.lockLevel);
        SetSliderInteractable(speedA, ld.lockLevel);
        SetSliderInteractable(speedB, ld.lockLevel);
        SetToggleInteractable(simulateB, ld.lockLevel);
        SetToggleInteractable(enableLock, ld.lockLevel);
        SetToggleInteractable(commonCounter, ld.lockLevel);
        SetToggleInteractable(endGameButton, ld.lockLevel);
    }

    List<string> GetLevelNames(List<LevelData> levelData)
    {
        List<string> output = new List<string>();
        foreach (LevelData level in levelData)
            output.Add(level.name);

        return output;
    }

    List<LevelData> GetLevelData()
    {
        List<LevelData> output = new List<LevelData>();

        string path = @"Levels.xml";
        if (File.Exists(path))
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode root = doc.FirstChild;

            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    XmlElement level = (XmlElement)root.ChildNodes[i];
                    LevelData levelData;
                    levelData.lockLevel = Convert.ToBoolean(level["lockLevel"].InnerText);
                    levelData.name = level["name"].InnerText;
                    levelData.fruitsA = Convert.ToInt32(level["fruitsA"].InnerText);
                    levelData.fruitsB = Convert.ToInt32(level["fruitsB"].InnerText);
                    levelData.speedA = Convert.ToInt32(level["speedA"].InnerText);
                    levelData.speedB = Convert.ToInt32(level["speedB"].InnerText);
                    levelData.simulateB = Convert.ToBoolean(level["simulateB"].InnerText);
                    levelData.enableLock = Convert.ToBoolean(level["enableLock"].InnerText);
                    levelData.commonCounter = Convert.ToBoolean(level["commonCounter"].InnerText);
                    levelData.endGameButton = Convert.ToBoolean(level["endGameButton"].InnerText);

                    output.Add(levelData);
                }
            }
        }
        else
        {
            Debug.LogError("Levels.xml not found");
        }

        return output;
    }
}
