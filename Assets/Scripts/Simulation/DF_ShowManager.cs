using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class DF_ShowManager : MonoBehaviour
{
    public enum SignalChange
    {
        normal,
        PreCU,
        PrePTT
    }

    //Stages
    [Header("Stage / Characters")] public StageSelector[] stages;

    [HideInInspector] public int currentStage;


    //Inspector Objects
    [Header("Inspector Objects")]
    public AudioSource[] speakerR;

    public AudioSource[] speakerL;
    [HideInInspector] public GameObject mackValves;
    [HideInInspector] public DF_ShowtapeCreator creator;
    [HideInInspector] public VideoPlayer videoplayer;
    [HideInInspector] public GameObject thePlayer;
    public SignalChange signalChange;

    public DF_ShowtapeManager manager;
    public FloatEvent characterEvent = new();
    

    [Space(20)]

    //Show Data
    [Header("Show Data")]
    private MacValves _mac;


    private void Awake()
    {
        //Initialize Objects
        thePlayer = GameObject.Find("Player");
        _mac = gameObject.GetComponentInChildren<MacValves>();
        videoplayer = GetComponent<VideoPlayer>();
        creator = GetComponent<DF_ShowtapeCreator>();

        //Start up stages
        for (int i = 0; i < stages.Length; i++) stages[i].Startup();

    }

    private void Update()
    {
        //Advances the tutorial if it is active
        if (manager.recordMovements && manager.referenceSpeaker.clip != null)
            if (manager.referenceSpeaker.time >= manager.speakerClip.length)
                SpecialSaveAs();

        //Run the Simulation
        UpdateAnims();
    }

    private void UpdateAnims()
    {
        //A special case for swapping signals around in realtime through the Live Editor
        switch (signalChange)
        {
            case SignalChange.PreCU:
                bool g = _mac.topDrawer[85];
                _mac.topDrawer[85] = _mac.topDrawer[80];
                _mac.topDrawer[80] = false;
                _mac.topDrawer[83] = g;
                _mac.topDrawer[92] = _mac.topDrawer[90];
                _mac.topDrawer[93] = _mac.topDrawer[91];
                _mac.bottomDrawer[79] = _mac.bottomDrawer[74];
                _mac.bottomDrawer[90] = _mac.bottomDrawer[74];
                _mac.bottomDrawer[89] = false;
                _mac.bottomDrawer[63] = true;
                _mac.topDrawer[25] = !_mac.topDrawer[25];
                _mac.topDrawer[26] = !_mac.topDrawer[26];
                break;
            case SignalChange.PrePTT:

                break;
        }

        //Update Portable Animatronics
        characterEvent.Invoke(Time.deltaTime * 60);

        //Update Lights
        for (int i = 0; i < stages[currentStage].lightValves.Length; i++)
            stages[currentStage].lightValves[i].UpdateLight();

        //Update Curtains
        if (stages[currentStage].curtainValves != null)
        {
            if (manager.referenceSpeaker.pitch < 0)
                stages[currentStage].curtainValves.CreateMovements(Time.deltaTime * 60, true);
            else
                stages[currentStage].curtainValves.CreateMovements(Time.deltaTime * 60, false);
        }

        //Update TV turn offs
        if (manager.videoPath != "")
            for (int i = 0; i < stages[currentStage].tvs.Length; i++)
            {
                bool bitOff = false;
                bool bitOn = false;
                if (stages[currentStage].tvs[i].drawer)
                {
                    if (_mac.bottomDrawer[stages[currentStage].tvs[i].bitOff]) bitOff = true;
                    if (_mac.bottomDrawer[stages[currentStage].tvs[i].bitOn]) bitOn = true;
                }
                else
                {
                    if (_mac.topDrawer[stages[currentStage].tvs[i].bitOff]) bitOff = true;
                    if (_mac.topDrawer[stages[currentStage].tvs[i].bitOn]) bitOn = true;
                }


                //Curtain check
                if (stages[currentStage].tvs[i].onWhenCurtain)
                {
                    //0 = Off First Frame
                    //1 = Off
                    //2 = On First Frame
                    //3 = On
                    //If Force Curtain
                    if (stages[currentStage].curtainValves.curtainOverride &&
                        stages[currentStage].tvs[i].curtainSubState == 1)
                        stages[currentStage].tvs[i].curtainSubState = 2;
                    if (!stages[currentStage].curtainValves.curtainOverride &&
                        stages[currentStage].tvs[i].curtainSubState == 3)
                        stages[currentStage].tvs[i].curtainSubState = 0;


                    //Apply
                    if (stages[currentStage].tvs[i].curtainSubState == 0)
                    {
                        for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                            stages[currentStage].tvs[i].tvs[e].material.SetColor("_BaseColor", Color.black);
                        stages[currentStage].tvs[i].curtainSubState = 1;
                        Debug.Log("Force Closed Curtain");
                    }
                    else if (stages[currentStage].tvs[i].curtainSubState == 2)
                    {
                        for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                            stages[currentStage].tvs[i].tvs[e].material.SetColor("_BaseColor", Color.white);
                        stages[currentStage].tvs[i].curtainSubState = 3;
                        Debug.Log("Force Opened Curtain");
                    }
                }

                switch (stages[currentStage].tvs[i].tvSettings)
                {
                    case ShowTV.tvSetting.onOnly:
                        if (!bitOn)
                            for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                                stages[currentStage].tvs[i].tvs[e].material.SetColor("_BaseColor", Color.black);
                        else
                            for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                                stages[currentStage].tvs[i].tvs[e].material.SetColor("_BaseColor", Color.white);

                        break;
                    case ShowTV.tvSetting.offOn:
                        //Reversed time curtain bits
                        if (stages[currentStage].tvs[i].onWhenCurtain && manager.referenceSpeaker.pitch < 0)
                        {
                            bool temp;
                            temp = bitOff;
                            bitOff = bitOn;
                            bitOn = temp;
                        }

                        if (bitOff)
                        {
                            Debug.Log("Closed Curtain");
                            for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                                stages[currentStage].tvs[i].tvs[e].material.SetColor("_BaseColor", Color.black);
                        }

                        if (bitOn)
                        {
                            Debug.Log("Opened Curtain");
                            for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                                stages[currentStage].tvs[i].tvs[e].material.SetColor("_BaseColor", Color.white);
                        }

                        break;
                    case ShowTV.tvSetting.none:
                        for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                            stages[currentStage].tvs[i].tvs[e].material.SetColor("_BaseColor", Color.white);
                        break;
                }
            }
    }
    

    /// <summary>
    ///     Loads audio and video from the showtape manager into the stage speakers.
    /// </summary>
    public void loadAudio()
    {
        manager.referenceSpeaker.clip = manager.speakerClip;
        for (int i = 0; i < speakerL.Length; i++) speakerL[i].clip = manager.speakerClip;
        for (int i = 0; i < speakerR.Length; i++) speakerR[i].clip = manager.speakerClip;
        if (manager.videoPath != "")
        {
            if (!manager.useVideoAsReference) videoplayer.url = manager.videoPath;
            videoplayer.Play();
            videoplayer.Pause();
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
            if (manager.videoPath != "") videoplayer.time = manager.referenceSpeaker.time;
            for (int i = 0; i < speakerL.Length; i++) speakerL[i].time = manager.referenceSpeaker.time;
            for (int i = 0; i < speakerR.Length; i++) speakerR[i].time = manager.referenceSpeaker.time;
        }
    }

    /// <summary>
    ///     Stops the showtape.
    /// </summary>
    public void Stop()
    {
        manager.referenceSpeaker.time = 0;
        manager.referenceVideo.time = 0;
        manager.Play(true, false);
        for (int i = 0; i < stages[currentStage].curtainValves.curtainbools.Length; i++)
            stages[currentStage].curtainValves.curtainbools[i] = false;
    }

    /// <summary>
    ///     Pauses or unpauses the showtape.
    ///  !!! Will open the showtape file prompt if no showtape is loaded!
    /// </summary>
    public void TogglePlayback()
    {
        if (manager.rshwData != null)
        {
            if ((manager.useVideoAsReference && manager.referenceVideo.isPlaying) ||
                (!manager.useVideoAsReference && manager.referenceSpeaker.isPlaying))
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
    ///     Saves a showtape while returning to the Create Recording menu.
    ///     This is called when creating a new showtape.
    /// </summary>
    /// <param name="input"></param>
    public void SpecialSaveAs()
    {
        creator.SaveRecordingAs();
    }

    /// <summary>
    ///     Pauses audio and video.
    /// </summary>
    public void AVPause()
    {
        Debug.Log("Audio Video Pause");
        if (manager.videoPath != "") videoplayer.Pause();
        manager.referenceSpeaker.Pause();
        for (int i = 0; i < speakerL.Length; i++) speakerL[i].Pause();
        for (int i = 0; i < speakerR.Length; i++) speakerR[i].Pause();
        syncAudio();
    }

    /// <summary>
    ///     Plays audio and video.
    /// </summary>
    public void AVPlay()
    {
        Debug.Log("Audio Video Pause");
        if (manager.videoPath != "") videoplayer.Play();
        manager.referenceSpeaker.Play();
        for (int i = 0; i < speakerL.Length; i++) speakerL[i].Play();
        for (int i = 0; i < speakerR.Length; i++) speakerR[i].Play();
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
            manager.referenceSpeaker.pitch = 1;
        else
            PitchForward();
        for (int i = 0; i < speakerL.Length; i++) speakerL[i].pitch = manager.referenceSpeaker.pitch;
        for (int i = 0; i < speakerR.Length; i++) speakerR[i].pitch = manager.referenceSpeaker.pitch;
        if (manager.videoPath != "") videoplayer.playbackSpeed = manager.referenceSpeaker.pitch;
        syncAudio();
    }

    /// <summary>
    ///     Pitches forward the audio and video by one setting.
    /// </summary>
    public void PitchForward()
    {
        if (!manager.playMovements)
        {
            manager.referenceSpeaker.pitch = 0;
            manager.Play(true, true);
        }

        switch (manager.referenceSpeaker.pitch)
        {
            case -100:
                manager.referenceSpeaker.pitch = -10;
                break;
            case -10:
                manager.referenceSpeaker.pitch = -5;
                break;
            case -5:
                manager.referenceSpeaker.pitch = -2;
                break;
            case -2:
                manager.referenceSpeaker.pitch = -1;
                break;
            case -1:
                manager.referenceSpeaker.pitch = -0.5f;
                break;
            case -0.5f:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case 0:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case 0.5f:
                manager.referenceSpeaker.pitch = 1f;
                break;
            case 1:
                manager.referenceSpeaker.pitch = 2f;
                break;
            case 2:
                manager.referenceSpeaker.pitch = 5f;
                break;
            case 5:
                manager.referenceSpeaker.pitch = 10f;
                break;
            case 10:
                manager.referenceSpeaker.pitch = 100f;
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
            manager.referenceSpeaker.pitch = 0;
            manager.Play(true, true);
        }

        switch (manager.referenceSpeaker.pitch)
        {
            case 100:
                manager.referenceSpeaker.pitch = 10;
                break;
            case 10:
                manager.referenceSpeaker.pitch = 5;
                break;
            case 5:
                manager.referenceSpeaker.pitch = 2;
                break;
            case 2:
                manager.referenceSpeaker.pitch = 1;
                break;
            case 1:
                manager.referenceSpeaker.pitch = 0.5f;
                break;
            case 0.5f:
                manager.referenceSpeaker.pitch = -0.5f;
                break;
            case 0:
                manager.referenceSpeaker.pitch = -0.5f;
                break;
            case -0.5f:
                manager.referenceSpeaker.pitch = -1f;
                break;
            case -1:
                manager.referenceSpeaker.pitch = -2f;
                break;
            case -2:
                manager.referenceSpeaker.pitch = -5f;
                break;
            case -5:
                manager.referenceSpeaker.pitch = -10f;
                break;
            case -10:
                manager.referenceSpeaker.pitch = -100f;
                break;
        }

        if (manager.videoPath != "") videoplayer.playbackSpeed = manager.referenceSpeaker.pitch;
        manager.syncTvsAndSpeakers.Invoke();
    }
}