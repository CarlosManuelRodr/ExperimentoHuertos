using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gestiona el slider de cantidad de frutos.
/// </summary>
public class FruitSliderController : MonoBehaviour
{
    public GameObject ownNumber;
    public GameObject rivalSlider;
    public bool allowFew = false;

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
        if (!allowFew)
            rivalSliderScript.value = 100 - newValue;
    }
}
