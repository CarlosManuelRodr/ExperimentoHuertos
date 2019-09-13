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
        if (toggle.isOn && !status)
            toggle.isOn = false;
    }
}
