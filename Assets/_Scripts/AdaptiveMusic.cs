using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdaptiveMusic : MonoBehaviour
{
    [Header("Main Music")]
    public AudioSource musicSource;
    public Slider healthSlider;
    public Slider volumeSlider;
    public TMP_Text pauseButtonText;
    private bool isPaused = false;
    public Button pauseButton;

    [Header("Day/Night Settings")]
    public AudioClip dayMusic;
    public AudioClip nightMusic;
    public Button DayNight;
    public TMP_Text DayNightText;
    public string sun = "\u263C";
    public string moon = "\u263E";
    public Material skyboxDay;
    public Material skyboxNight;
    private bool isDay = true;
    private Image dayNightButtonImage;

    [Header("Pitch Settings")]
    public float normalPitch = 1.0f;
    public float maxPitch = 1.5f;
    public float minHealthThreshold = 5f;

    [Header("Progress Bar")]
    public Slider progressSlider;
    public TMP_Text progressText;

    [Header("Enemy Layers")]
    public Slider enemySlider;
    public List<AudioSource> enemyLayers = new List<AudioSource>();

    [Header("Playlist")]
    public TMP_Dropdown trackDropdown;
    public List<AudioClip> tracks = new List<AudioClip>();
    private int currentTrackIndex = 0;


    void Start()
    {
        dayNightButtonImage = DayNight.GetComponent<Image>();
        DayNightText.text = sun;
        //DayNightText.color = Color.orange;
        dayNightButtonImage.color = Color.white;
        pauseButton.image.color = Color.red;   // playing
        pauseButtonText.text = "Pause";

        if (dayMusic != null)
        {
            musicSource.clip = dayMusic;
            musicSource.loop = true;
        }

        if (tracks != null && tracks.Count > 0 && trackDropdown != null)
        {
            SetupPlaylist();
        }
        else
        {
            PlayMusic();
        }

        //SetupEnemySlider();
        SetupProgressSlider();
    }

    void Update()
    {
        float health = healthSlider.value;
        float healthPercent = Mathf.Clamp01(health);

        if (healthPercent * 100f > minHealthThreshold)
        {
            float t = 1f - healthPercent;
            musicSource.pitch = Mathf.Lerp(normalPitch, maxPitch, t);
        }
        else
        {
            musicSource.pitch = maxPitch;
        }

        if (volumeSlider != null)
        {
            musicSource.volume = volumeSlider.value;
        }

        if (progressSlider != null && musicSource.clip != null)
        {
            if (musicSource.clip.length > 0f)
            {
                progressSlider.SetValueWithoutNotify(musicSource.time / musicSource.clip.length);
            }
        }
        if (progressText != null && musicSource.clip != null)
        {
            float current = musicSource.time;
            float total = musicSource.clip.length;

            progressText.text = FormatTime(current) + " / " + FormatTime(total);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return minutes.ToString() + ":" + seconds.ToString("00");
    }

    public void PlayMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
            isPaused = false;
        }
    }

    public void PauseMusic()
    {
        if (!isPaused)
        {
            pauseButton.image.color = Color.green; // paused
            pauseButtonText.text = "Play";
            musicSource.Pause();
            isPaused = true;
        }
        else
        {
            pauseButton.image.color = Color.red;   // playing
            pauseButtonText.text = "Pause";
            musicSource.Play();
            isPaused = false;
        }
    }

    public void SwitchDayNight()
    {
        float currentTime = musicSource.time;

        if (isDay)
        {
            if (nightMusic != null)
            {
                musicSource.clip = nightMusic;
                if (musicSource.clip.length > 0f)
                {
                    musicSource.time = Mathf.Clamp(currentTime, 0f, musicSource.clip.length - 0.01f);
                }
                musicSource.Play();
            }
            DayNightText.text = moon;
            DayNightText.color = Color.white;
            dayNightButtonImage.color = Color.black;
            RenderSettings.skybox = skyboxNight;
            isDay = false;
        }
        else
        {
            if (dayMusic != null)
            {
                musicSource.clip = dayMusic;
                if (musicSource.clip.length > 0f)
                {
                    musicSource.time = Mathf.Clamp(currentTime, 0f, musicSource.clip.length - 0.01f);
                }
                musicSource.Play();
            }
            DayNightText.text = sun;
            //DayNightText.color = Color.orange;
            dayNightButtonImage.color = Color.white;
            RenderSettings.skybox = skyboxDay;
            isDay = true;
        }
    }

    void SetupProgressSlider()
    {
        if (progressSlider == null) return;

        progressSlider.minValue = 0f;
        progressSlider.maxValue = 1f;

        progressSlider.wholeNumbers = false;

        progressSlider.onValueChanged.AddListener(OnProgressSliderChanged);
    }

    public void OnProgressSliderChanged(float value)
    {
        if (musicSource != null && musicSource.clip != null && musicSource.clip.length > 0f)
        {
            musicSource.time = value * musicSource.clip.length;
        }
    }

    /*
    void SetupEnemySlider()
    {
        if (enemySlider == null) return;

        enemySlider.minValue = 0;
        enemySlider.maxValue = enemyLayers.Count;
        enemySlider.wholeNumbers = true;

        enemySlider.onValueChanged.AddListener(OnEnemySliderChanged);

        foreach (var src in enemyLayers)
        {
            if (src == null) continue;
            src.loop = true;
            src.Play();
            src.volume = 0f;
        }
        OnEnemySliderChanged(enemySlider.value);
    }

    public void OnEnemySliderChanged(float value)
    {
        int count = Mathf.RoundToInt(value);
        for (int i = 0; i < enemyLayers.Count; i++)
        {
            if (enemyLayers[i] == null) continue;

            enemyLayers[i].volume = (i < count) ? 1f : 0f;
        }
    }
    */
    void SetupPlaylist()
    {
        if (trackDropdown == null || tracks == null || tracks.Count == 0) return;

        trackDropdown.ClearOptions();
        List<string> names = new List<string>();
        foreach (var clip in tracks)
        {
            names.Add(clip != null ? clip.name : "Unnamed");
        }

        trackDropdown.AddOptions(names);
        trackDropdown.onValueChanged.AddListener(OnTrackDropdownChanged);

        // Start with first track
        currentTrackIndex = 0;
        musicSource.clip = tracks[currentTrackIndex];
        PlayMusic();
    }

    public void OnTrackDropdownChanged(int index)
    {
        if (tracks == null || index < 0 || index >= tracks.Count) return;

        currentTrackIndex = index;

        bool wasPlaying = musicSource.isPlaying;
        musicSource.clip = tracks[currentTrackIndex];
        musicSource.time = 0f;

        if (wasPlaying || !isPaused)
        {
            PlayMusic();
        }
    }
}