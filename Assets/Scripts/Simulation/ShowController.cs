using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using SFB;
using Show;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;
using Light = Show.Light;

public class ShowController : MonoBehaviour
{
    [Header("General Information")]
    [Tooltip("Whether the tape is playing")] [ReadOnly] public bool playing;
    
    [Header("Simulation")] 
    [Tooltip("Whether the show simulation should be enabled or not")] public bool active = true;
    [Tooltip("How many times the show simulation should be updated per second")] [Unit("FPS")] [Range (1, 120)] public int updateRate = 60;
    [Space(5)] 
    
    [Header("Control Data")] 
    [ReadOnly] public bool[] topDrawer = new bool[300];
    [ReadOnly] public bool[] bottomDrawer = new bool[300];
    
    [Header("Audio & Video")]
    [ReadOnly] public string videoPath;
    
    

    private RR_SHW_Manager _rrShwManager;
    private OSP_Manager _ospManager;
    private GameObject _player;
    [HideInInspector] public ControlUI controlUI;
    
    [HideInInspector] public VideoPlayer referenceVideo;
    [HideInInspector] public AudioSource referenceAudio;
    private void Awake()
    {

        
        referenceVideo = transform.parent.gameObject.GetComponentInChildren<VideoPlayer>();
        referenceAudio = transform.parent.gameObject.GetComponentInChildren<AudioSource>();
        
        
        _rrShwManager = gameObject.GetComponent<RR_SHW_Manager>();
        _ospManager = gameObject.GetComponent<OSP_Manager>();
        _player = GameObject.FindGameObjectWithTag("Player");
        controlUI = _player.GetComponentInChildren<ControlUI>();
        referenceVideo = GetComponent<VideoPlayer>();
        referenceAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        referenceAudio.enabled = playing;
        topDrawer = new bool[300];
        bottomDrawer = new bool[300];
        
        if (!playing) return;
        
        if (videoPath != string.Empty) referenceVideo.time = referenceAudio.time;

        
        //Show Code
        int arrayDestination = (int)(referenceAudio.time * 60);


        //Apply the current frame of simulation data to the Mack Valves
        if (arrayDestination < _rrShwManager.rshwData.Length)
            for (int i = 0; i < 150; i++)
            {
                if (_rrShwManager.rshwData[arrayDestination].Get(i)) topDrawer[i] = true;
                if (_rrShwManager.rshwData[arrayDestination].Get(i + 150)) bottomDrawer[i] = true;
            }

        //Check if show is over
        if (!(referenceAudio.time >= referenceAudio.clip.length * referenceAudio.clip.channels)) return;

        Stop();
    }

    public void New()
    {
        
    }

    public async void Load()
    {
        try
        {
            CursorLockMode lockState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("Load");
            if (referenceAudio != null) referenceAudio.time = 0;
            if (referenceVideo != null) referenceVideo.time = 0;

            //Call File Browser

            var extensions = new[] { new ExtensionFilter("Show Files", "osp", "cshw", "sshw", "rshw") };

            string[] paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", extensions, false);

            if (paths.Length > 0)
            {
                if (paths[0].EndsWith("rshw") || paths[0].EndsWith("cshw") || paths[0].EndsWith("sshw"))
                {
                    var tcs = new TaskCompletionSource<bool>();

                    controlUI.DisplayWarning(
                        "The .*shw format is dangerous and should not be used. Only proceed if you trust the source of the showtape.\n\n(https://aka.ms/binaryformatter)",
                        (proceed) =>
                        {
                            if (!tcs.Task.IsCompleted)
                            {
                                tcs.SetResult(proceed);
                            }
                        }
                    );

                    bool proceed = await tcs.Task;

                    if (proceed)
                    {
                        IProgress<float> progress = new Progress<float>(value =>
                        {
                            controlUI.UpdateProgress(value);
                        });

                        await _rrShwManager.LoadFromUrl(paths[0], progress);
                    }
                }
            }

            Cursor.lockState = lockState;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
        }
    }

    public async void Convert()
    {
        try
        {
            CursorLockMode lockState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            OpenShowtapePackage output = null;
            
            var extensions = new[] { new ExtensionFilter("Show Files", "cshw", "sshw", "rshw") };

            string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Showtape to Convert", "", extensions, false);
        
            if (paths.Length > 0)
            {
                if (paths[0].EndsWith("rshw") || paths[0].EndsWith("cshw") || paths[0].EndsWith("sshw"))
                { 
                    IProgress<float> progress = new Progress<float>(value =>
                    {
                        controlUI.UpdateProgress(value);
                    });
                    
                    output = await _ospManager.ConvertFileAsync(paths[0], progress);
                    controlUI.UpdateProgress(0);
                }
            }

            string savePath = StandaloneFileBrowser.SaveFilePanel("Save Converted Showtape", "", Path.GetFileNameWithoutExtension(paths[0]) + ".osp", "osp");
            if (savePath != string.Empty)
            {
                _ospManager.Save(savePath, output);
            }
        
            Cursor.lockState = lockState;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n" + e.StackTrace);
        }
    }

    /// <summary>
    ///     Loads audio and video from the showtape manager into the stage speakers.
    /// </summary>
    public void loadAudio()
    {
        if (videoPath != string.Empty)
        {
            referenceVideo.url = videoPath;
            referenceVideo.Play();
            referenceVideo.Pause();
        }

        Play();
    }

    /// <summary>
    ///  Stops the active showtape.
    /// </summary>
    public void Stop()
    {
        videoPath = string.Empty;
        referenceAudio.clip = null;
        referenceAudio.time = 0;
        referenceVideo.time = 0;
        _rrShwManager.rshwData = null;
        Pause();
        CloseCurtains();
    }

    /// <summary>
    ///     Pauses or plays the show simulation.
    ///  !!! Will open the showtape file prompt if no showtape is loaded!
    /// </summary>
    public void TogglePlayback()
    {
        if (_rrShwManager.rshwData != null)
        {
            if (playing)
                Pause();
            else
                Play();
        }
        else
        {
            Load();
        }
    }

    /// <summary>
    ///  Temporarily stops the show simulation
    /// </summary>
    public void Pause()
    {
        if (videoPath != string.Empty) referenceVideo.Pause();
        playing = false;
    }

    /// <summary>
    ///  Starts / Resumes the show simulation
    /// </summary>
    public void Play()
    {
        if (videoPath != string.Empty) referenceVideo.Play();
        playing = true;
    }

    /// <summary>
    /// Opens all curtains in the scene
    /// </summary>
    public void OpenCurtains()
    {
        var curtains = transform.root.GetComponentsInChildren<Curtains>();
        for (int i = 0; i < curtains.Length; i++)
        {
            for (int x = 0; x < curtains[i].curtainbools.Length; x++)
                curtains[i].curtainbools[x] = false;
        }
    }
    
    /// <summary>
    /// Closes all curtains in the scene
    /// </summary>
    public void CloseCurtains()
    {
        var curtains = transform.root.GetComponentsInChildren<Curtains>();
        for (int i = 0; i < curtains.Length; i++)
        {
            for (int x = 0; x < curtains[i].curtainbools.Length; x++)
                curtains[i].curtainbools[x] = false;
        }
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
        if (videoPath != "") referenceVideo.playbackSpeed = referenceAudio.pitch;
    }

    /// <summary>
    ///     Pitches forward the audio and video by one setting.
    /// </summary>
    public void PitchForward()
    {
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
    }

    /// <summary>
    ///     Pitches backward the audio and video by one setting.
    /// </summary>
    public void PitchBackward()
    {

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

        if (videoPath != string.Empty) referenceVideo.playbackSpeed = referenceAudio.pitch;
    }
}