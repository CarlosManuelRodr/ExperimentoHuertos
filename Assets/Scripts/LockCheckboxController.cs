using UnityEngine;
using TMPro;

public class LockCheckboxController : MonoBehaviour
{
    public GameObject orchidAccessSelector;
    private TMP_Dropdown dropdownOrchid;
    private bool status;

    void Start()
    {
        dropdownOrchid = orchidAccessSelector.GetComponent<TMP_Dropdown>();
        status = true;
    }

    public void OnToggle()
    {
        status = !status;
        dropdownOrchid.interactable = status;
    }
}
