using UnityEngine;
using CorruptedSmileStudio.JukeBox;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
/// <summary>
/// The jukebox MonoBehaviour class, handles playing and changing the song.
/// </summary>
public class Jukebox : MonoBehaviour
{
    /// <summary>
    /// The songs to be played.
    /// </summary>
    public Song[] songs;
    /// <summary>
    /// The audio source from where the sound will be playing.
    /// </summary>
    private AudioSource source;
    /// <summary>
    /// The current song to be played.
    /// </summary>	
    public int currentSong = 0;
    /// <summary>
    /// Ensures that only one jukebox instance will be available throughout a gameplay.
    /// </summary>	
    private static Jukebox tmp;
    /// <summary>
    /// The volume of the music.
    /// </summary>
    private float _volume = 100.0f;
    /// <summary>
    /// Whether to play a random song instead of in order.
    /// </summary>
    public bool random = false;
    /// <summary>
    /// Delegate signature that will be called on song change.
    /// </summary>
    /// <param name="title">A [Artist] - [Title] formatted string.</param>
    public delegate void DisplaySongChange(string title);
    /// <summary>
    /// The method to call on song changes to display the new song title.
    /// </summary>
    private DisplaySongChange songChange;
    /// <summary>
    /// Plays music on start up 
    /// </summary>
    public bool playOnStart = true;

    void Awake()
    {
        if (tmp == null)
        {
            tmp = this;
            DontDestroyOnLoad(gameObject);
            source = gameObject.GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (playOnStart)
        {
            if (random)
            {
                currentSong = -1;
                NextTrack();
            }
            else
            {
                Play();
            }
        }
    }
    /// <summary>
    /// The volume of the music.
    /// </summary>
    public float volume
    {
        get { return _volume; }
        set
        {
            _volume = value;
            _volume = _volume > 100.0f ? 100.0f : _volume;
            _volume = _volume < 0.0f ? 0.0f : _volume;
            source.volume = _volume / 100.0f;
        }
    }

    /// <summary>
    /// Plays the next song, if there is no next song plays the first song again.
    /// </summary>
    public void NextTrack()
    {
        Stop();
        if (songs.Length > 0)
        {
            if (random)
            {
                if (songs.Length > 1)
                {
                    System.Random ran = new System.Random();

                    int newSong = currentSong;
                    do
                    {
                        newSong = ran.Next(0, songs.Length);
                    } while (newSong == currentSong);
                    currentSong = newSong;
                }
            }
            else
            {
                currentSong++;
                if (currentSong == songs.Length)
                    currentSong = 0;
            }
            Play();
        }
    }
    /// <summary>
    /// Plays the previous song, if there is no previous song plays the last song in the list.
    /// </summary>
    public void PreviousTrack()
    {
        Stop();
        if (songs.Length > 0)
        {
            if (random)
            {
                if (songs.Length > 1)
                {
                    System.Random ran = new System.Random();

                    int newSong = currentSong;
                    while (newSong == currentSong)
                    {
                        newSong = ran.Next(0, songs.Length);
                    }
                    currentSong = newSong;
                }
            }
            else
            {
                currentSong--;
                if (currentSong == -1)
                    currentSong = songs.Length - 1;
            }
            Play();
        }
    }
    /// <summary>
    /// Stops playback.
    /// </summary>
    public void Stop()
    {
        source.Stop();
    }
    /// <summary>
    /// Starts playing the song at currentSong in the song list.
    /// </summary>
    public void Play()
    {
        if (songs.Length > 0)
        {
            currentSong = Mathf.Clamp(currentSong, 0, songs.Length - 1);

            if (songs[currentSong].clip != null)
            {
                volume = volume;
                source.clip = songs[currentSong].clip;
                source.Play();
                Invoke("NextTrack", songs[currentSong].clip.length);
                ShowTitle();
            }
            else
            {
                Debug.LogError(string.Format("Songs element {0} is missing an Audio Clip.", currentSong));
                NextTrack();
            }
        }
    }
    /// <summary>
    /// Returns a formatted string of the current song.
    /// </summary>
    /// <returns>Returns a string of the [Artist] - [Song]</returns>
    public string CurrentSong()
    {
        return songs[currentSong].ToString();
    }
    /// <summary>
    /// Call the SongChange method. Checks if the method has been set. Only calls if been set.
    /// </summary>
    private void ShowTitle()
    {
        if (songChange != null && source.isPlaying)
        {
            songChange(songs[currentSong].ToString());
        }
    }
    /// <summary>
    /// The method to call on song changes to display the new song title.
    /// Calls the method when assigned if the Audio is currently playing.
    /// </summary>
    public DisplaySongChange SongChange
    {
        get { return songChange; }
        set
        {
            songChange = value;
            ShowTitle();
        }
    }
}
