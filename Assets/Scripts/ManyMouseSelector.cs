using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Dropdown para seleccionar mouse a asignar a cada cursor.
/// </summary>
public class ManyMouseSelector : MonoBehaviour
{
    public Player player = Player.PlayerA;
    public int defaultSelection = 0;

    private TMP_Dropdown dropdown;
    private List<string> mouseDevices = new List<string>();

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        for (int i = 0; i < ManyMouseWrapper.MouseCount; i++)
        {
            mouseDevices.Add(ManyMouseWrapper.MouseDeviceName(i));
        }
        dropdown.AddOptions(mouseDevices);

        int selection;
        if (player == Player.PlayerA)
            selection = PlayerPrefs.GetInt("MouseIdA", defaultSelection);
        else
            selection = PlayerPrefs.GetInt("MouseIdB", defaultSelection);

        if (selection > ManyMouseWrapper.MouseCount)
            selection = defaultSelection;

        dropdown.SetValueWithoutNotify(selection);
    }

    public void DropdownValueChanged(int value)
    {
        if (player == Player.PlayerA)
            PlayerPrefs.SetInt("MouseIdA", value);
        else
            PlayerPrefs.SetInt("MouseIdB", value);
    }
}
