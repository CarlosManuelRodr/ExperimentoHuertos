using UnityEngine;
using System.Collections;

/// <summary>
/// Used as an example song displayer on song change.
/// </summary>
[RequireComponent(typeof(Jukebox))]
public class SongDisplay : MonoBehaviour
{
    /// <summary>
    /// The JukeBox component
    /// </summary>
    private Jukebox box;
    /// <summary>
    /// Whether the title is showing at the moment.
    /// </summary>
    private bool showTitle = false;
    /// <summary>
    /// What the current title is.
    /// </summary>
    private string songTitle = "";
    /// <summary>
    /// The time to display the title for.
    /// </summary>
    public float secondsForTitle = 2.0f;
    /// <summary>
    /// The GUISkin to use for displaying the title.
    /// </summary>
    public GUISkin skin;

    /// <summary>
    /// Gets the JukeBox component and ensures that the SongChange delegate is set.
    /// </summary>
    void Start()
    {
        box = gameObject.GetComponent<Jukebox>();
        box.SongChange = SongTitle;
    }

    /// <summary>
    /// The delegate to be called from the Jukebox component
    /// </summary>
    /// <param name="title">The song Title.</param>
    void SongTitle(string title)
    {
        CancelInvoke();
        showTitle = true;
        songTitle = title;
        Invoke("TurnOffTitle", secondsForTitle);
    }

    /// <summary>
    /// Turns off the song title display.
    /// </summary>
    void TurnOffTitle()
    {
        showTitle = false;
    }

    /// <summary>
    /// Used to show the song title.
    /// </summary>
    void OnGUI()
    {
        GUI.skin = skin;
        if (showTitle)
        {
            GUI.Box(new Rect((Screen.width / 2.0f) - 100, Screen.height - 60.0f, 200, 60.0f), songTitle);
        }
    }
}