using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedSliderController : MonoBehaviour
{
    public GameObject ownNumber;

    private TextMeshProUGUI ownText;
    private Slider ownSlider;

    void Start()
    {
        ownText = ownNumber.GetComponent<TextMeshProUGUI>();
        ownSlider = GetComponent<Slider>();
    }

    public void OnUpdateValue()
    {
        uint newValue = (uint)ownSlider.value;
        ownText.SetText(newValue.ToString());
    }
}
