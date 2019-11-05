using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LockCheckboxController : MonoBehaviour
{
    public GameObject orchidAccessSelector, chestAccessSelector;
    private TMP_Dropdown dropdownOrchid, dropdownChest;
    private bool status;

    void Start()
    {
        dropdownOrchid = orchidAccessSelector.GetComponent<TMP_Dropdown>();
        dropdownChest = chestAccessSelector.GetComponent<TMP_Dropdown>();
        status = true;
    }

    public void OnToggle()
    {
        status = !status;
        dropdownOrchid.interactable = status;
        dropdownChest.interactable = status;
    }
}
