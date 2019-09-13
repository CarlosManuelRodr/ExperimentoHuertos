using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeOrchadCheckboxController : MonoBehaviour
{
    public GameObject lockToggle;
    private Toggle toggle;
    private bool status;

    void Start()
    {
        toggle = lockToggle.GetComponent<Toggle>();
        status = true;
    }

    public void OnToggle()
    {
        status = !status;
        toggle.interactable = status;
        toggle.isOn = status;
    }
}
