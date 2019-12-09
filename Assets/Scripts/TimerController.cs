using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float timer = 0.0f;
    private int lastSecond = 0;
    private bool running = false;

    void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    public void ResetTimer()
    {
        timer = 0.0f;
        lastSecond = 0;
        timerText.text = "00:00";
    }

    public void StartTimer()
    {
        this.ResetTimer();
        running = true;
    }

    public void StopTimer()
    {
        running = false;
        this.ResetTimer();
    }

    void Update()
    {
        if (running)
        {
            timer += Time.deltaTime;
            int seconds = Mathf.FloorToInt(timer % 60);
            if (seconds != lastSecond)
            {
                int minutes = Mathf.FloorToInt(timer / 60);
                string timeString = minutes.ToString("00") + ":" + seconds.ToString("00");
                timerText.text = timeString;
            }
        }
    }
}
