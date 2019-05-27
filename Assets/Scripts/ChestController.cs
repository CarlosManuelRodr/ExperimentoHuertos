using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool m_capture;
    private uint score;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        m_capture = false;
        spriteRenderer.enabled = false;
        score = 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (m_capture && (other.tag == "ItemA" || other.tag == "ItemB"))
        {
            Destroy(other.gameObject);
            audioSource.Play();
            m_capture = false;
            spriteRenderer.enabled = false;
            score++;
            this.transform.parent.GetComponentInChildren<Text>().text = "Puntos: " + score;
        }
    }

    public void SetToCapture(bool capture)
    {
        m_capture = capture;
        spriteRenderer.enabled = capture;
    }
}
