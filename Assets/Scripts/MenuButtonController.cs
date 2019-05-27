using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject arrow;
    private AudioSource audioSource;
    private bool interactable;
    private Button button;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();
        arrow.SetActive(false);
        interactable = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (interactable)
        {
            arrow.SetActive(true);
            audioSource.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        arrow.SetActive(false);
    }

    public void OnClick()
    {
        arrow.SetActive(false);
    }

    public void SetInteractable(bool sel)
    {
        interactable = sel;
        button.interactable = sel;
    }
}
