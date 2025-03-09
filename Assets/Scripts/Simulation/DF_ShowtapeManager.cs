using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using eToile;
using SFB;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class FloatEvent : UnityEvent<float>
{
}

public class DF_ShowtapeManager : MonoBehaviour
{
    public enum LoopVers
    {
        noLoop,
        loopPlaylist,
        loopSong
    }

    //Inspector Objects
    [Header("Inspector Objects")] public MacValves mack;

    [HideInInspector] public DF_ShowtapeCreator creator;
    public AudioSource referenceSpeaker;
    [HideInInspector] public float refSpeakerVol;
    public VideoPlayer referenceVideo;
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
    public UnityEvent curtainClose;
    public UnityEvent curtainOpen;
    public UnityEvent syncTvsAndSpeakers;

    [HideInInspector] public bool disableCharactersOnStart = true;

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
        creator = GetComponent<DF_ShowtapeCreator>();
        referenceVideo = GetComponent<VideoPlayer>();
        refSpeakerVol = referenceSpeaker.volume;
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
            mack.topDrawer = new bool[300];
            mack.bottomDrawer = new bool[300];

            //Check for inputs and send to mack valves
            if (mack != null)
                if (useVideoAsReference || (!useVideoAsReference && referenceSpeaker.clip != null))
                    if (rshwData != null)
                    {
                        //Show Code
                        //Being paused means the same frame of data will loop
                        //Being unpaused means deciding where to start next sim frame
                        int arrayDestination;
                        if (useVideoAsReference)
                            arrayDestination = (int)(referenceVideo.time * dataStreamedFPS);
                        else
                            arrayDestination = (int)(referenceSpeaker.time * dataStreamedFPS);


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
                                if (rshwData[arrayDestination].Get(i)) mack.topDrawer[i] = true;
                                if (rshwData[arrayDestination].Get(i + 150)) mack.bottomDrawer[i] = true;
                            }

                        //Check if show is over
                        if ((!useVideoAsReference && referenceSpeaker.time >= speakerClip.length * speakerClip.channels)
                            || (useVideoAsReference && referenceVideo.time >=
                                referenceVideo.length * referenceVideo.GetAudioChannelCount(0)))
                            if (!recordMovements)
                            {
                                Debug.Log("Song is over. Queuing next song / stopping.");
                                Play(true, false);
                                if (songLoopSetting == LoopVers.loopSong)
                                {
                                    if (currentShowtapeSegment == -1)
                                    {
                                        referenceSpeaker.time = 0;
                                        referenceVideo.time = 0;
                                    }
                                    else
                                    {
                                        creator.LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
                                    }
                                }
                                else
                                {
                                    //Check if multi showtape or single
                                    if (currentShowtapeSegment == -1)
                                    {
                                        referenceSpeaker.time = 0;
                                        referenceVideo.time = 0;
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
                                                creator.LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
                                            }
                                            else
                                            {
                                                Unload();
                                            }
                                        }
                                        else
                                        {
                                            creator.LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
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
        if (referenceSpeaker != null) referenceSpeaker.time = 0;
        if (referenceVideo != null) referenceVideo.time = 0;

        //Call File Browser
        showtapeSegmentPaths = new string[1];
        string[] paths;
        if (fileExtention == "")
        {
            ExtensionFilter[] extensions;
            extensions = new[] { new ExtensionFilter("Show Files", "cshw", "sshw", "rshw", "nshw") };

            paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", extensions, false);
        }
        else
        {
            paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", fileExtention, false);
        }

        if (paths.Length > 0)
        {
            showtapeSegmentPaths[0] = paths[0];
            currentShowtapeSegment = 0;
            creator.LoadFromURL(paths[0]);
        }

        Cursor.lockState = lockState;
    }

    public void LoadFolder()
    {
        CursorLockMode lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        referenceSpeaker.time = 0;
        referenceVideo.time = 0;
        //Call File Browser
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder of Showtapes", "", false);
        if (paths.Length > 0)
        {
            showtapeSegmentPaths = Directory.GetFiles(paths[0], "*." + fileExtention);
            currentShowtapeSegment = 0;
            creator.LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
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

    public void SwapLoop()
    {
        if (songLoopSetting == LoopVers.loopPlaylist)
            songLoopSetting = LoopVers.loopSong;
        else if (songLoopSetting == LoopVers.loopSong)
            songLoopSetting = LoopVers.noLoop;
        else
            songLoopSetting = LoopVers.loopPlaylist;
    }

    public void SkipSong(int skip)
    {
        playMovements = false;
        referenceSpeaker.time = 0;
        referenceVideo.time = 0;
        if (songLoopSetting == LoopVers.noLoop || songLoopSetting == LoopVers.loopPlaylist)
            currentShowtapeSegment += skip;

        if (currentShowtapeSegment < 0)
        {
            currentShowtapeSegment = 0;
        }
        else if (currentShowtapeSegment >= showtapeSegmentPaths.Length)
        {
            if (songLoopSetting == LoopVers.loopPlaylist)
                currentShowtapeSegment = 0;
            else
                currentShowtapeSegment = showtapeSegmentPaths.Length - 1;
        }

        creator.LoadFromURL(showtapeSegmentPaths[currentShowtapeSegment]);
    }

    public void Unload()
    {
        videoPath = "";
        playMovements = false;
        recordMovements = false;
        referenceSpeaker.time = 0;
        referenceVideo.time = 0;
        currentShowtapeSegment = -1;
        showtapeSegmentPaths = new string[1];
        rshwData = new BitArray[0];
        audioVideoPause.Invoke();
        curtainClose.Invoke();
    }
    public void PadAllBits(int padding)
    {
        if (rshwData != null)
        {
            int address;
            if (useVideoAsReference)
                address = (int)(referenceVideo.time * dataStreamedFPS);
            else
                address = (int)(referenceSpeaker.time * dataStreamedFPS);


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