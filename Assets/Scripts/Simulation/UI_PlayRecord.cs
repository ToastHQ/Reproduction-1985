using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_PlayRecord : MonoBehaviour
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

    private UI_WindowMaker _uiWindowMaker;

    //Inspector Objects
    [Header("Inspector Objects")]
    public AudioSource[] speakerR;

    public AudioSource[] speakerL;
    public Sprite[] icons;
    [HideInInspector] public GameObject mackValves;
    [HideInInspector] public UI_RshwCreator creator;
    public UI_SidePanel sidePanel;
    public Text ffSpeed;
    public Text AddSource;
    public Text Uncompress;
    public Text ticketText;
    [HideInInspector] public VideoPlayer videoplayer;
    [HideInInspector] public GameObject thePlayer;
    public SignalChange signalChange;

    public UI_ShowtapeManager manager;
    public FloatEvent characterEvent = new();

    private InputHandler inputHandlercomp;
    

    [Space(20)]

    //Show Data
    [Header("Show Data")]
    private Mack_Valves mack;


    private void Awake()
    {
        //Initialize Objects
        thePlayer = GameObject.Find("Player");
        inputHandlercomp = mackValves.GetComponent<InputHandler>();
        mack = mackValves.GetComponent<Mack_Valves>();
        manager.inputHandler = inputHandlercomp;
        videoplayer = GetComponent<VideoPlayer>();
        creator = GetComponent<UI_RshwCreator>();
        _uiWindowMaker = gameObject.GetComponent<UI_WindowMaker>();

        //Start up stages
        for (int i = 0; i < stages.Length; i++) stages[i].Startup();

        //Spawn in current Characters
        RecreateAllAnimatronics();

        SwitchWindow(1);
    }

    private void Update()
    {
        //Advances the tutorial if it is active
        if (manager.recordMovements && manager.referenceSpeaker.clip != null)
            if (manager.referenceSpeaker.time >= manager.speakerClip.length)
                SpecialSaveAs(11);

        //Run the Simulation
        UpdateAnims();
    }

    private void UpdateAnims()
    {
        //A special case for swapping signals around in realtime through the Live Editor
        switch (signalChange)
        {
            case SignalChange.PreCU:
                bool g = mack.topDrawer[85];
                mack.topDrawer[85] = mack.topDrawer[80];
                mack.topDrawer[80] = false;
                mack.topDrawer[83] = g;
                mack.topDrawer[92] = mack.topDrawer[90];
                mack.topDrawer[93] = mack.topDrawer[91];
                mack.bottomDrawer[79] = mack.bottomDrawer[74];
                mack.bottomDrawer[90] = mack.bottomDrawer[74];
                mack.bottomDrawer[89] = false;
                mack.bottomDrawer[63] = true;
                mack.topDrawer[25] = !mack.topDrawer[25];
                mack.topDrawer[26] = !mack.topDrawer[26];
                break;
            case SignalChange.PrePTT:

                break;
        }

        //Update Portable Animatronics
        characterEvent.Invoke(Time.deltaTime * 60);

        //Update Lights
        for (int i = 0; i < stages[currentStage].lightValves.Length; i++)
            stages[currentStage].lightValves[i].CreateMovements(Time.deltaTime * 60);

        //Update Curtains
        if (stages[currentStage].curtainValves != null)
        {
            if (manager.referenceSpeaker.pitch < 0)
                stages[currentStage].curtainValves.CreateMovements(Time.deltaTime * 60, true);
            else
                stages[currentStage].curtainValves.CreateMovements(Time.deltaTime * 60, false);
        }

        //Update Turntables
        for (int i = 0; i < stages[currentStage].tableValves.Length; i++)
            stages[currentStage].tableValves[i].CreateMovements(Time.deltaTime * 60);

        //Update AudioController
        if (stages[currentStage].texController != null) stages[currentStage].texController.CreateTex();

        //Update TV turn offs
        if (manager.videoPath != "")
            for (int i = 0; i < stages[currentStage].tvs.Length; i++)
            {
                bool bitOff = false;
                bool bitOn = false;
                if (stages[currentStage].tvs[i].drawer)
                {
                    if (mack.bottomDrawer[stages[currentStage].tvs[i].bitOff]) bitOff = true;
                    if (mack.bottomDrawer[stages[currentStage].tvs[i].bitOn]) bitOn = true;
                }
                else
                {
                    if (mack.topDrawer[stages[currentStage].tvs[i].bitOff]) bitOff = true;
                    if (mack.topDrawer[stages[currentStage].tvs[i].bitOn]) bitOn = true;
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
                    case ShowTV.tvSetting.offOnly:
                        break;
                        if (bitOff)
                            for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                                stages[currentStage].tvs[i].tvs[e].material.SetColor("_BaseColor", Color.black);
                        else
                            for (int e = 0; e < stages[currentStage].tvs[i].tvs.Length; e++)
                                stages[currentStage].tvs[i].tvs[e].material.SetColor("_BaseColor", Color.white);
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
    ///     Switches which window is displayed on the main UI panel.
    /// </summary>
    /// <param name="thewindow"></param>
    public void SwitchWindow(int thewindow)
    {
        try
        {
            _uiWindowMaker.Loading.SetActive(true);
            switch (thewindow)
            {
                case 0:
                    // Unused
                    break;
                case 1:
                    //Main Screen
                    _uiWindowMaker.MakeThreeWindow(icons[0], icons[1], icons[2], 0, 8, 6, 3, "Customize",
                        "Play", "Record");
                    WindowSwitchDisable(true);
                    creator.EraseShowtape();
                    break;
                case 2:
                    // Unused
                    break;
                case 3:
                    //Record Screen
                    _uiWindowMaker
                        .MakeTwoWindow(icons[2], icons[3], 1, 5, 4, "New Recording", "Edit Recording");
                    WindowSwitchDisable(true);
                    creator.EraseShowtape();
                    break;
                case 4:
                    //Edit Recording Screen
                    _uiWindowMaker
                        .MakeTwoWindow(icons[3], icons[5], 3, 34, 21, "Edit Segment", "Add to Segment");
                    WindowSwitchDisable(true);
                    creator.EraseShowtape();
                    break;
                case 5:
                    //New Recording Screen
                    _uiWindowMaker.MakeNewRecordWindow();
                    creator.EraseShowtape();
                    break;
                case 6:
                    //Player Menu (Single)
                    manager.Load();
                    if (manager.rshwData != null) _uiWindowMaker.MakePlayWindow(false);
                    break;
                case 7:
                    //Player Menu (Folder)
                    manager.LoadFolder();
                    if (manager.rshwData != null) _uiWindowMaker.MakePlayWindow(false);
                    break;
                case 8:
                    //Customize Screen
                    _uiWindowMaker
                        .MakeThreeWindow(icons[8], icons[9], icons[9], 1, 16, 9, 28, "Edit Stage", "Edit Characters", "Edit Props");
                    break;
                case 9:
                    //Edit Character Icons Screen
                    _uiWindowMaker.MakeCharacterCustomizeIconsWindow();
                    break;
                case 11:
                    //Recording Groups Screen (Or New Recording Screen)
                    _uiWindowMaker.MakeRecordIconsWindow();
                    WindowSwitchDisable(false);
                    creator.EraseShowtape();
                    break;
                case 16:
                    //Stage Customize Menu
                    StageCustomMenu();
                    break;
                case 17:
                    if (manager.rshwData != null) _uiWindowMaker.MakePlayWindow(false);
                    break;
                case 21:
                    //Recording Groups Screen (Standalone)
                    _uiWindowMaker.MakeRecordIconsWindow();
                    WindowSwitchDisable(false);
                    creator.EraseShowtape();
                    break;
                case 22:
                    //Delete Movement Screen 1
                    _uiWindowMaker.MakeDeleteMoveMenu(0);
                    break;
                case 23:
                    //Delete Movement Back 1
                    _uiWindowMaker.MakeDeleteMoveMenu(-1);
                    break;
                case 24:
                    //Delete Movement Forward 1
                    _uiWindowMaker.MakeDeleteMoveMenu(1);
                    break;
                case 28:
                    // Edit Prop Icons Screen
                    _uiWindowMaker.MakePropCustomizeIconsWindow();
                    break;
                case 29:
                    //Segment Window 1

                    break;
                case 30:
                    //Segment Window -1

                    break;
                case 31:
                    // Unused
                    break;
                case 32:
                    // Unused

                    break;
                case 33:
                    // Unused
                    break;
                case 34:
                    _uiWindowMaker
                        .MakeTwoWindow(icons[6], icons[5], 4, 22, 35, "Delete Bits", "Replace Audio");
                    break;
                case 35:
                    creator.ReplaceShowAudio();
                    break;
            }
            _uiWindowMaker.Loading.SetActive(false);
        }
        catch(Exception e)
        {
            _uiWindowMaker.MakeErrorWindow(e);
        }
    }

    /// <summary>
    ///     Starts new show.
    /// </summary>
    /// <param name="input"></param>
    public void StartNewShow(int input)
    {
        Debug.Log("Starting New Show");
        manager.Load();
        if (manager.speakerClip != null) SwitchWindow(input);
    }

    /// <summary>
    ///     Load a customize window for a particular character.
    /// </summary>
    /// <param name="input"></param>
    public void CharacterCustomMenu(int input)
    {
        _uiWindowMaker.MakeCharacterCustomizeWindow(input);
    }

    /// <summary>
    ///     Load a customize stage window for a particular stage.
    /// </summary>
    public void StageCustomMenu()
    {
        _uiWindowMaker.MakeStageCustomizeWindow(stages, currentStage);
    }

    /// <summary>
    ///     Index up the current costume of a character. Costume 0 is no character on stage.
    /// </summary>
    /// <param name="input"></param>
    public void CostumeUp(int input)
    {
        if (stages[currentStage].animatronics[input].currentCostume > -1)
        {
            stages[currentStage].animatronics[input].currentCostume--;
            _uiWindowMaker.MakeCharacterCustomizeWindow(input);
        }
    }

    /// <summary>
    ///     Index down the current costume of a character.
    /// </summary>
    /// <param name="input"></param>
    public void CostumeDown(int input)
    {
        if (stages[currentStage].animatronics[input].currentCostume < stages[currentStage].animatronics[input].costumes.Length - 1)
        {
            stages[currentStage].animatronics[input].currentCostume++;
            _uiWindowMaker.MakeCharacterCustomizeWindow(input);
        }
    }

    /// <summary>
    ///     Index up the current stage presented.
    /// </summary>
    /// <param name="input"></param>
    public void StageUp(int input)
    {
        if (input > 0)
        {
            currentStage--;
            for (int i = 0; i < stages.Length; i++)
                if (i != currentStage)
                {
                    if (stages[i].stage.activeSelf) stages[i].stage.SetActive(false);
                }
                else
                {
                    if (!stages[i].stage.activeSelf) stages[i].stage.SetActive(true);
                }

            RecreateAllAnimatronics();
            _uiWindowMaker.MakeStageCustomizeWindow(stages, currentStage);
        }
    }

    /// <summary>
    ///     Index down the current stage presented.
    /// </summary>
    /// <param name="input"></param>
    public void StageDown(int input)
    {
        if (input < stages.Length - 1)
        {
            currentStage++;
            for (int i = 0; i < stages.Length; i++)
                if (i != currentStage)
                {
                    if (stages[i].stage.activeSelf) stages[i].stage.SetActive(false);
                }
                else
                {
                    if (!stages[i].stage.activeSelf) stages[i].stage.SetActive(true);
                }

            RecreateAllAnimatronics();
            _uiWindowMaker.MakeStageCustomizeWindow(stages, currentStage);
        }
    }

    
    
    /// <summary>
    /// Setup all animatronics in a stage
    /// </summary>
    public void RecreateAllAnimatronics()
    {

        sidePanel.FlowLoad(-1);
    }

    /// <summary>
    ///     Open a window for the current movment group to be used.
    /// </summary>
    /// <param name="input"></param>
    public void RecordingGroupMenu(int input)
    {
        _uiWindowMaker.MakeMoveTestWindow(input);
    }

    /// <summary>
    ///     Stops the recording during a window switch.
    /// </summary>
    /// <param name="curtainStop"></param>
    private void WindowSwitchDisable(bool curtainStop)
    {
        manager.recordMovements = false;
        inputHandlercomp.valveMapping = 0;
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
        SwitchWindow(17);
        _uiWindowMaker.playMenuManager.TextUpdate(false);
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
        SwitchWindow(1);
        for (int i = 0; i < stages[currentStage].curtainValves.curtainbools.Length; i++)
            stages[currentStage].curtainValves.curtainbools[i] = false;
    }

    /// <summary>
    ///     Pauses or unpauses the showtape.
    /// </summary>
    public void pauseSong()
    {
        if ((manager.useVideoAsReference && manager.referenceVideo.isPlaying) ||
            (!manager.useVideoAsReference && manager.referenceSpeaker.isPlaying))
            AVPause();
        else
            AVPlay();
    }

    /// <summary>
    ///     Saves a showtape while returning to the Create Recording menu.
    ///     This is called when creating a new showtape.
    /// </summary>
    /// <param name="input"></param>
    public void SpecialSaveAs(int input)
    {
        if (creator.SaveRecordingAs()) SwitchWindow(input);
        if (input == 11) SwitchWindow(input);
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