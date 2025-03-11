using System.Collections;
using System.Linq;
using SFB;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class DF_ShowtapeManager : MonoBehaviour
{
    private GameObject _player;
    public enum LoopVers
    {
        noLoop,
        loopPlaylist,
        loopSong
    }

    //Inspector Objects
    [Header("Inspector Objects")] private MacValves _mac;

    private RR_SHW_Manager _creator;
    private DF_ShowController _showController;
    [HideInInspector] public float refSpeakerVol;
    public AudioClip speakerClip;
    public LoopVers songLoopSetting;
    public string wavPath;
    public string videoPath;
    public string fileExtention = "rshw";
    public string[] showtapeSegmentPaths = new string[1];
    public int currentShowtapeSegment = -1;
    public float dataStreamedFPS = 60;

    [Space(20)]

    //Events
    public UnityEvent audioVideoPlay;

    public UnityEvent audioVideoPause;
    public UnityEvent audioVideoGetData;
    public UnityEvent syncTvsAndSpeakers;

    [HideInInspector] public bool disableCharactersOnStart = true;
    
    [HideInInspector] public float timeSongStarted;
    [HideInInspector] public float timeSongOffset;
    [HideInInspector] public float timePauseStart;
    [HideInInspector] public float timeInputSpeedStart;
    [HideInInspector] public int previousFramePosition;
    [HideInInspector] public bool previousAnyButtonHeld;

    //Sim States
    public bool recordMovements;
    public bool playMovements;

    public bool useVideoAsReference;

    [Space(20)]

    //File Show MetaData
    [Header("File Show Metadata")]
    public BitArray[] rshwData;

    //Sync TV for Large Shows (Unity's Fault)
    private float syncTimer;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _mac = GameObject.FindGameObjectWithTag("Mac Valves").GetComponent<MacValves>();
        _creator = transform.root.GetComponentInChildren<RR_SHW_Manager>();
        _showController = GameObject.FindGameObjectWithTag("Show Controller").GetComponent<DF_ShowController>();
    }

    private void Update()
    {
        //Big Show Sync
        syncTimer += Time.deltaTime;
        if (syncTimer >= 30)
        {
            syncTimer = 0;
            syncTvsAndSpeakers.Invoke();
        }

        //Clear Drawers
            _mac.topDrawer = new bool[300];
            _mac.bottomDrawer = new bool[300];

            //Check for inputs and send to mack valves
                if (useVideoAsReference || (!useVideoAsReference && _showController.referenceAudio.clip != null))
                    if (rshwData != null)
                    {
                        //Show Code
                        //Being paused means the same frame of data will loop
                        //Being unpaused means deciding where to start next sim frame
                        int arrayDestination;
                        if (useVideoAsReference)
                            arrayDestination = (int)(_showController.referenceVideo.time * dataStreamedFPS);
                        else
                            arrayDestination = (int)(_showController.referenceAudio.time * dataStreamedFPS);


                        //Check if new frames need to be created
                        if (arrayDestination >= rshwData.Length && rshwData.Length != 0)
                        {
                            if (recordMovements)
                                while (arrayDestination + 1 > rshwData.Length)
                                    rshwData = rshwData.Append(new BitArray(300)).ToArray();
                            else
                                arrayDestination = rshwData.Length;
                        }


                        //Apply the current frame of simulation data to the Mack Valves
                        if (arrayDestination < rshwData.Length)
                            for (int i = 0; i < 150; i++)
                            {
                                if (rshwData[arrayDestination].Get(i)) _mac.topDrawer[i] = true;
                                if (rshwData[arrayDestination].Get(i + 150)) _mac.bottomDrawer[i] = true;
                            }

                        //Check if show is over
                        if ((!useVideoAsReference && _showController.referenceAudio.time >= speakerClip.length * speakerClip.channels)
                            || (useVideoAsReference && _showController.referenceVideo.time >=
                                _showController.referenceVideo.length * _showController.referenceVideo.GetAudioChannelCount(0)))
                            if (!recordMovements)
                            {
                                Debug.Log("Song is over. Queuing next song / stopping.");
                                Play(true, false);
                                if (songLoopSetting == LoopVers.loopSong)
                                {
                                    if (currentShowtapeSegment == -1)
                                    {
                                        _showController.referenceAudio.time = 0;
                                        _showController.referenceVideo.time = 0;
                                    }
                                    else
                                    {
                                        _creator.LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
                                    }
                                }
                                else
                                {
                                    //Check if multi showtape or single
                                    if (currentShowtapeSegment == -1)
                                    {
                                        _showController.referenceAudio.time = 0;
                                        _showController.referenceVideo.time = 0;
                                        Unload();
                                    }
                                    else
                                    {
                                        currentShowtapeSegment++;
                                        //Check if end of multi showtape or not
                                        if (currentShowtapeSegment >= showtapeSegmentPaths.Length)
                                        {
                                            if (songLoopSetting == LoopVers.loopPlaylist)
                                            {
                                                currentShowtapeSegment = 0;
                                                _creator.LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
                                            }
                                            else
                                            {
                                                Unload();
                                            }
                                        }
                                        else
                                        {
                                            _creator.LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
                                        }
                                    }
                                }
                            }

                        previousFramePosition = arrayDestination; 
                    }
    }

    public void Load()
    {
        CursorLockMode lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Load");
        if (_showController.referenceAudio != null) _showController.referenceAudio.time = 0;
        if (_showController.referenceVideo != null) _showController.referenceVideo.time = 0;

        //Call File Browser
        showtapeSegmentPaths = new string[1];
        string[] paths;
        if (fileExtention == "")
        {
            ExtensionFilter[] extensions;
            extensions = new[] { new ExtensionFilter("Show Files", "cshw", "sshw", "rshw") };

            paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", extensions, false);
        }
        else
        {
            paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", fileExtention, false);
        }

        if (paths.Length > 0)
        {
            if (paths[0].EndsWith("rshw") || paths[0].EndsWith("cshw") || paths[0].EndsWith("sshw"))
            {
                ControlUI controlUI = _player.GetComponentInChildren<ControlUI>();
                if (controlUI.DisplayWarning("The .*shw format is dangerous and should not be used. Only proceed if you trust the source of the showtape.\n\n(https://aka.ms/binaryformatter)"))
                {
                    showtapeSegmentPaths[0] = paths[0];
                    currentShowtapeSegment = 0;
                    _creator.LoadFromURL(paths[0]);
                }
            }
        }

        Cursor.lockState = lockState;
    }

    public void Play(bool force, bool onOff)
    {
        if (force)
            playMovements = onOff;
        else
            playMovements = !playMovements;
        syncTvsAndSpeakers.Invoke();
        
        audioVideoPlay.Invoke();

    }

    public void Unload()
    {
        videoPath = "";
        playMovements = false;
        recordMovements = false;
        _showController.referenceAudio.time = 0;
        _showController.referenceVideo.time = 0;
        currentShowtapeSegment = -1;
        showtapeSegmentPaths = new string[1];
        rshwData = new BitArray[0];
        audioVideoPause.Invoke();
    }
    public void PadAllBits(int padding)
    {
        if (rshwData != null)
        {
            int address;
            if (useVideoAsReference)
                address = (int)(_showController.referenceVideo.time * dataStreamedFPS);
            else
                address = (int)(_showController.referenceAudio.time * dataStreamedFPS);


            if (padding > 0)
            {
                //Create new space
                for (int i = 0; i < padding; i++) rshwData = rshwData.Append(new BitArray(300)).ToArray();

                int track = rshwData.Length;
                while (track > address)
                {
                    if (track + padding < rshwData.Length) rshwData[track + padding] = rshwData[track];
                    track--;
                }
            }
            else
            {
                while (address < rshwData.Length)
                {
                    if (address + padding > 0) rshwData[address + padding] = rshwData[address];
                    address++;
                }
            }
        }
    }
}