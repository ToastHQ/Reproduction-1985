using UnityEngine;
using UnityEngine.Video;

public class DF_ShowController : MonoBehaviour
{

    [Header("Simulation")] 
    public bool active = true;
    [Range (1, 120)] public int updateRate = 60;
    
    [Header("Control Data")]
    
    
    [HideInInspector] public Stage[] stages;

    [HideInInspector] public int currentStage;
    

    [Header("Scripts")]
    public DF_ShowtapeManager manager;

    [HideInInspector] public VideoPlayer referenceVideo;
    [HideInInspector] public AudioSource referenceAudio;

    private void Awake()
    {

        //Start up stages
        for (int i = 0; i < stages.Length; i++) stages[i].Startup();
        
        referenceVideo = transform.parent.gameObject.GetComponentInChildren<VideoPlayer>();
        referenceAudio = transform.parent.gameObject.GetComponentInChildren<AudioSource>();
    }
    

    /// <summary>
    ///     Loads audio and video from the showtape manager into the stage speakers.
    /// </summary>
    public void loadAudio()
    {
        referenceAudio.clip = manager.speakerClip;
        for (int i = 0; i < stages[currentStage].speakers.Length; i++) stages[currentStage].speakers[i].clip = manager.speakerClip;
        if (manager.videoPath != "")
        {
            if (!manager.useVideoAsReference) referenceVideo.url = manager.videoPath;
            referenceVideo.Play();
            referenceVideo.Pause();
        }

        manager.Play(true, true);
        syncAudio();
    }

    /// <summary>
    ///     Ensures audio and video is synced when the showtape is playing.
    /// </summary>
    public void syncAudio()
    {
        if (!manager.useVideoAsReference)
        {
            if (manager.videoPath != "") referenceVideo.time = referenceAudio.time;
            for (int i = 0; i < stages[currentStage].speakers.Length; i++) stages[currentStage].speakers[i].time = referenceAudio.time;
        }
    }

    /// <summary>
    ///     Stops the showtape.
    /// </summary>
    public void Stop()
    {
        referenceAudio.time = 0;
        referenceVideo.time = 0;
        manager.Play(true, false);
        if (stages[currentStage].curtains != null)
            for (int i = 0; i < stages[currentStage].curtains.curtainbools.Length; i++)
                stages[currentStage].curtains.curtainbools[i] = false;
    }

    /// <summary>
    ///     Pauses or unpauses the showtape.
    ///  !!! Will open the showtape file prompt if no showtape is loaded!
    /// </summary>
    public void TogglePlayback()
    {
        if (manager.rshwData != null)
        {
            if ((manager.useVideoAsReference && referenceVideo.isPlaying) ||
                (!manager.useVideoAsReference && referenceAudio.isPlaying))
                AVPause();
            else
                AVPlay();
        }
        else
        {
            manager.Load();
        }
    }

    /// <summary>
    ///     Pauses audio and video.
    /// </summary>
    public void AVPause()
    {
        Debug.Log("Audio Video Pause");
        if (manager.videoPath != "") referenceVideo.Pause();
        referenceAudio.Pause();
        for (int i = 0; i < stages[currentStage].speakers.Length; i++) stages[currentStage].speakers[i].Pause();
        syncAudio();
    }

    /// <summary>
    ///     Plays audio and video.
    /// </summary>
    public void AVPlay()
    {
        Debug.Log("Audio Video Pause");
        if (manager.videoPath != "") referenceVideo.Play();
        referenceAudio.Play();
        for (int i = 0; i < stages[currentStage].speakers.Length; i++) stages[currentStage].speakers[i].Play();
        syncAudio();
    }

    /// <summary>
    ///     Increases the speed of audio and video.
    /// </summary>
    /// <param name="input"></param>
    public void FFSong(int input)
    {
        if (input == -1)
            PitchBackward();
        else if (input == 0)
            referenceAudio.pitch = 1;
        else
            PitchForward();
        for (int i = 0; i < stages[currentStage].speakers.Length; i++) stages[currentStage].speakers[i].pitch = referenceAudio.pitch;
        if (manager.videoPath != "") referenceVideo.playbackSpeed = referenceAudio.pitch;
        syncAudio();
    }

    /// <summary>
    ///     Pitches forward the audio and video by one setting.
    /// </summary>
    public void PitchForward()
    {
        if (!manager.playMovements)
        {
            referenceAudio.pitch = 0;
            manager.Play(true, true);
        }

        switch (referenceAudio.pitch)
        {
            case -100:
                referenceAudio.pitch = -10;
                break;
            case -10:
                referenceAudio.pitch = -5;
                break;
            case -5:
                referenceAudio.pitch = -2;
                break;
            case -2:
                referenceAudio.pitch = -1;
                break;
            case -1:
                referenceAudio.pitch = -0.5f;
                break;
            case -0.5f:
                referenceAudio.pitch = 0.5f;
                break;
            case 0:
                referenceAudio.pitch = 0.5f;
                break;
            case 0.5f:
                referenceAudio.pitch = 1f;
                break;
            case 1:
                referenceAudio.pitch = 2f;
                break;
            case 2:
                referenceAudio.pitch = 5f;
                break;
            case 5:
                referenceAudio.pitch = 10f;
                break;
            case 10:
                referenceAudio.pitch = 100f;
                break;
        }

        manager.syncTvsAndSpeakers.Invoke();
    }

    /// <summary>
    ///     Pitches backward the audio and video by one setting.
    /// </summary>
    public void PitchBackward()
    {
        if (!manager.playMovements)
        {
            referenceAudio.pitch = 0;
            manager.Play(true, true);
        }

        switch (referenceAudio.pitch)
        {
            case 100:
                referenceAudio.pitch = 10;
                break;
            case 10:
                referenceAudio.pitch = 5;
                break;
            case 5:
                referenceAudio.pitch = 2;
                break;
            case 2:
                referenceAudio.pitch = 1;
                break;
            case 1:
                referenceAudio.pitch = 0.5f;
                break;
            case 0.5f:
                referenceAudio.pitch = -0.5f;
                break;
            case 0:
                referenceAudio.pitch = -0.5f;
                break;
            case -0.5f:
                referenceAudio.pitch = -1f;
                break;
            case -1:
                referenceAudio.pitch = -2f;
                break;
            case -2:
                referenceAudio.pitch = -5f;
                break;
            case -5:
                referenceAudio.pitch = -10f;
                break;
            case -10:
                referenceAudio.pitch = -100f;
                break;
        }

        if (manager.videoPath != "") referenceVideo.playbackSpeed = referenceAudio.pitch;
        manager.syncTvsAndSpeakers.Invoke();
    }
}