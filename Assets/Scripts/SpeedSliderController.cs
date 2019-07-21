using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controlador de slider de velocidad de cursor.
/// </summary>
public class SpeedSliderController : MonoBehaviour
{
    public GameObject ownNumber;

    private TextMeshProUGUI ownText = null;
    private Slider ownSlider = null;

    void Start()
    {
        ownText = ownNumber.GetComponent<TextMeshProUGUI>();
        ownSlider = GetComponent<Slider>();
    }

    public void OnUpdateValue()
    {
        if (ownSlider != null && ownText != null)
        {
            uint newValue = (uint)ownSlider.value;
            ownText.SetText(newValue.ToString());
        }
    }
}
