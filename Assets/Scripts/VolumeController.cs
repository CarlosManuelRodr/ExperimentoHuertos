using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeController : MonoBehaviour
{
    public GameObject ownNumber;
    public GameObject gameManager;

    private TextMeshProUGUI ownText;
    private Slider ownSlider;
    private AudioSource jukebox;

    void Start()
    {
        ownText = ownNumber.GetComponent<TextMeshProUGUI>();
        ownSlider = GetComponent<Slider>();
        jukebox = gameManager.GetComponent<AudioSource>();
    }

    public void OnUpdateValue()
    {
        uint newValue = (uint)ownSlider.value;
        ownText.SetText(newValue.ToString());
        jukebox.volume = (float) newValue / 100.0f;
    }
}
