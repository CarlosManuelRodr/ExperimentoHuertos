using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum LevelType
{
    TUTORIAL,
    TEST_GAME,
    DEFAULT,
    CUSTOM
}

// LevelData contiene todos los parámetros que definen la configuración de un nivel.
public struct LevelData
{
    public string name;
    public string description;
    public Sprite image;
    public uint fruitsA, fruitsB;
    public uint speedA, speedB;
    public bool freeOrchard, enableLock;
    public bool commonCounter, endGameButton;
    public bool showInstructions;
    public string instruction1, instruction2, instruction3;
}

/// <summary>
/// Controlador de elemento Dropdown que contiene el selector de niveles cargados desde "levels.xml".
/// </summary>
public class LevelSelectController : MonoBehaviour
{
    public GameObject defaultLevelOptions, customLevelOptions;
    public GameObject fruitsA, fruitsB, speedA, speedB;
    public GameObject enableLock, commonCounter, endGameButton;
    public Sprite tutorialImage;

    private AudioSource sound;
    private TMP_Dropdown dropdown;
    private List<LevelData> levels;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        sound = GetComponent<AudioSource>();
        levels = GetLevelData();
        dropdown.AddOptions(GetLevelNames(levels));
        dropdown.AddOptions(new List<string> { "Personalizado" });
    }

    public void OnLevelSelected()
    {
        if (dropdown.value == levels.Count + 2)
        {
            // Activa menú de configuración de nivel.
            defaultLevelOptions.SetActive(false);
            customLevelOptions.SetActive(true);
        }
        else
        {
            // Activa menú de descripción de nivel.
            defaultLevelOptions.SetActive(true);
            customLevelOptions.SetActive(false);

            TMP_InputField levelDescription = defaultLevelOptions.GetComponentInChildren<TMP_InputField>();
            Image levelImage = defaultLevelOptions.transform.Find("LevelImage").GetComponent<Image>();

            if (dropdown.value == 0)
            {
                // Descripción del tutorial.
                levelDescription.text = "<color=green>Instrucciones:<color=black>\n\nEn el tutorial"
                                        + " usted aprendera los controles basicos del juego.";
                levelImage.sprite = tutorialImage;
            }
            else if (dropdown.value == 1)
            {
                // Descripción de la partida de prueba.
                levelDescription.text = "<color=green>Instrucciones:<color=black>\n\nEn el nivel de prueba"
                                        + " usted podra probar el juego.";
                levelImage.sprite = tutorialImage;
            }
            else
            {
                // Descripción de nivel estándar.
                LevelData ld = levels[dropdown.value-2];
                levelDescription.text = "<color=green>Instrucciones:<color=black>\n\n" + ld.description;
                levelImage.sprite = ld.image;
            }
        }
        sound.Play();
    }

    // Obtiene parámetros del nivel seleccionado en el dropdown.
    public LevelData GetSelectedLevel()
    {
        if (dropdown.value == 0)
            return new LevelData();
        else if (dropdown.value == 1)
            return new LevelData();
        else
        {
            if (dropdown.value == levels.Count + 2)
                return new LevelData();
            else
                return levels[dropdown.value - 2];
        }
    }

    public LevelType GetSelectedLevelType()
    {
        if (dropdown.value == 0)
            return LevelType.TUTORIAL;
        else if (dropdown.value == 1)
            return LevelType.TEST_GAME;
        else
        {
            if (dropdown.value == levels.Count + 2)
                return LevelType.CUSTOM;
            else
                return LevelType.DEFAULT;
        }
    }

    List<string> GetLevelNames(List<LevelData> levelData)
    {
        List<string> output = new List<string>();
        foreach (LevelData level in levelData)
            output.Add(level.name);

        return output;
    }

    // Carga la configuración de niveles desde archivo "levels.xml".
    List<LevelData> GetLevelData()
    {
        List<LevelData> output = new List<LevelData>();

        string path = @"Levels/Levels.xml";
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
                    levelData.name = level["name"].InnerText;
                    levelData.description = level["description"].InnerText;
                    levelData.image = IMG2Sprite.LoadNewSprite("Levels/" + level["image"].InnerText);
                    levelData.fruitsA = Convert.ToUInt32(level["fruitsA"].InnerText);
                    levelData.fruitsB = Convert.ToUInt32(level["fruitsB"].InnerText);
                    levelData.speedA = Convert.ToUInt32(level["speedA"].InnerText);
                    levelData.speedB = Convert.ToUInt32(level["speedB"].InnerText);
                    levelData.freeOrchard = Convert.ToBoolean(level["freeOrchard"].InnerText);

                    if (levelData.freeOrchard)
                        levelData.enableLock = false;
                    else
                        levelData.enableLock = Convert.ToBoolean(level["enableLock"].InnerText);

                    levelData.commonCounter = Convert.ToBoolean(level["commonCounter"].InnerText);
                    levelData.endGameButton = Convert.ToBoolean(level["endGameButton"].InnerText);

                    levelData.showInstructions = Convert.ToBoolean(level["showInstructions"].InnerText);
                    levelData.instruction1 = level["instruction1"].InnerText;
                    levelData.instruction2 = level["instruction2"].InnerText;
                    levelData.instruction3 = level["instruction3"].InnerText;
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
