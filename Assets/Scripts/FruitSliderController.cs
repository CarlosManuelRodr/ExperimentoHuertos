using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FruitSliderController : MonoBehaviour
{
    public GameObject ownNumber;
    public GameObject rivalSlider;

    private TextMeshProUGUI ownText;
    private Slider ownSlider;
    private Slider rivalSliderScript;

    void Start()
    {
        ownText = ownNumber.GetComponent<TextMeshProUGUI>();
        rivalSliderScript = rivalSlider.GetComponent<Slider>();
        ownSlider = GetComponent<Slider>();
    }

    public void OnUpdateValue()
    {
        uint newValue = (uint) ownSlider.value;
        ownText.SetText(newValue.ToString());
        rivalSliderScript.value = 100 - newValue;
    }
}
