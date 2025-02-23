using System.Collections;
using System.Collections.Generic;
using System.IO;
using eToile;
using SFB;
using UnityEngine;

public class UI_RshwCreator : MonoBehaviour
{
    public enum addWavResult
    {
        none,
        noSource,
        uncompressed
    }

    private UI_ShowtapeManager manager;
    private UI_PlayRecord playRecord;

    private void Awake()
    {
        playRecord = GetComponent<UI_PlayRecord>();
        manager = GetComponent<UI_ShowtapeManager>();
    }

    public addWavResult AddWav()
    {
        manager.speakerClip = null;
        CursorLockMode lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Adding Wav");
        //Call File Browser
        manager.wavPath = "";
        manager.showtapeSegmentPaths[0] = "";
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", "wav", false);
        if (paths.Length > 0)
        {
            if (paths[0] != "")
            {
                manager.wavPath = paths[0];
                manager.speakerClip = OpenWavParser.ByteArrayToAudioClip(File.ReadAllBytes(paths[0]));
                manager.audioVideoGetData.Invoke();
                CreateBitArray();
                if (manager.speakerClip == null) return addWavResult.uncompressed;

                return addWavResult.noSource;
            }

            return addWavResult.none;
        }

        return addWavResult.noSource;
        Cursor.lockState = lockState;
    }

    public void AddWavSpecial()
    {
        CursorLockMode lockState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Adding Wav");
        //Call File Browser
        manager.wavPath = "";
        manager.showtapeSegmentPaths[0] = "";
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", "wav", false);
        if (paths.Length > 0)
            if (paths[0] != "")
            {
                manager.wavPath = paths[0];
                manager.speakerClip = OpenWavParser.ByteArrayToAudioClip(File.ReadAllBytes(paths[0]));
                CreateBitArray();
            }

        Cursor.lockState = lockState;
    }

    public void StartNewShow()
    {
        Debug.Log("Starting New Show");
        manager.disableCharactersOnStart = false;
        manager.recordMovements = true;
        if (manager.wavPath != "") CreateBitArray();
    }

    private void CreateBitArray()
    {
        manager.rshwData = new BitArray[100];
        for (int i = 0; i < manager.rshwData.Length; i++) manager.rshwData[i] = new BitArray(300);
    }

    public void SaveRecording()
    {
        //Stop Show
        if (manager.rshwData != null)
        {
            manager.audioVideoPause.Invoke();
            manager.recordMovements = false;
            manager.playMovements = false;
            rshwFormat shw = new() { audioData = OpenWavParser.AudioClipToByteArray(manager.speakerClip) };
            var converted = new List<int>();
            for (int i = 0; i < manager.rshwData.Length; i++)
            {
                converted.Add(0);
                for (int e = 0; e < 300; e++)
                    if (manager.rshwData[i].Get(e))
                        converted.Add(e + 1);
            }

            shw.signalData = converted.ToArray();
            shw.Save(manager.showtapeSegmentPaths[0]);
            Debug.Log("Showtape Saved");
        }
        else
        {
            Debug.Log("No Showtape. Did not save.");
        }
    }

    public bool SaveRecordingAs()
    {
        if (manager.rshwData != null && manager.speakerClip != null)
        {
            CursorLockMode lockState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            //Stop Show
            manager.audioVideoPause.Invoke();
            manager.recordMovements = false;
            manager.playMovements = false;
            if (manager.speakerClip != null)
            {
                //Save to file
                string path =
                    StandaloneFileBrowser.SaveFilePanel("Save Showtape", "", "MyShowtape", manager.fileExtention);
                Debug.Log("Showtape Saved: " + path);
                if (!string.IsNullOrEmpty(path))
                {
                    manager.showtapeSegmentPaths = new string[1];
                    manager.showtapeSegmentPaths[0] = path;
                    rshwFormat shw = new() { audioData = OpenWavParser.AudioClipToByteArray(manager.speakerClip) };
                    var converted = new List<int>();
                    for (int i = 0; i < manager.rshwData.Length; i++)
                    {
                        converted.Add(0);
                        for (int e = 0; e < 300; e++)
                            if (manager.rshwData[i].Get(e))
                                converted.Add(e + 1);
                    }

                    shw.signalData = converted.ToArray();
                    shw.Save(path);
                }
                else
                {
                    Debug.Log("No Showtape. Did not save.");
                    AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
                    sc.volume = 1;
                    sc.PlayOneShot((AudioClip)Resources.Load("Deny"));
                    Cursor.lockState = lockState;
                    return false;
                }
            }

            Cursor.lockState = lockState;
            return true;
        }

        {
            Debug.Log("No Showtape. Did not save.");
            AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
            sc.volume = 1;
            sc.PlayOneShot((AudioClip)Resources.Load("Deny"));
            return false;
        }
    }

    public void LoadFromURL(string url)
    {
        StartCoroutine(LoadRoutineA(url));
    }

    private IEnumerator LoadRoutineA(string url)
    {
        yield return StartCoroutine(LoadRoutineB(url));
    }

    private IEnumerator LoadRoutineB(string url)
    {
        manager.disableCharactersOnStart = false;
        manager.playMovements = false;
        //Check if null
        if (url != "")
        {
            manager.referenceSpeaker.volume = manager.refSpeakerVol;
            manager.referenceSpeaker.time = 0;
            manager.useVideoAsReference = false;
            manager.referenceVideo.time = 0;
            manager.timeSongStarted = 0;
            manager.timeSongOffset = 0;
            manager.timePauseStart = 0;
            manager.timeInputSpeedStart = 0;
            yield return null;
            //Add code for opening .rshw file
            manager.curtainOpen.Invoke();
            yield return null;
            rshwFormat thefile = rshwFormat.ReadFromFile(url);
            yield return null;
            manager.speakerClip = OpenWavParser.ByteArrayToAudioClip(thefile.audioData);
            yield return null;
            var newSignals = new List<BitArray>();
            int countlength = 0;
            if (thefile.signalData[0] != 0)
            {
                countlength = 1;
                BitArray bit = new(300);
                newSignals.Add(bit);
            }

            for (int i = 0; i < thefile.signalData.Length; i++)
                if (thefile.signalData[i] == 0)
                {
                    countlength += 1;
                    BitArray bit = new(300);
                    newSignals.Add(bit);
                }
                else
                {
                    newSignals[countlength - 1].Set(thefile.signalData[i] - 1, true);
                }

            manager.rshwData = newSignals.ToArray();
            yield return null;

            //Video
            if (File.Exists(url.Remove(url.Length - Mathf.Max(manager.fileExtention.Length, 4)) + "mp4"))
            {
                Debug.Log("Video Found for Showtape.");
                manager.videoPath = url.Remove(url.Length - Mathf.Max(manager.fileExtention.Length, 4)) + "mp4";
            }
            else
            {
                manager.videoPath = "";
            }

            manager.audioVideoGetData.Invoke();
            yield return null;

            //Recording
            if (manager.recordMovements)
                Debug.Log("Recording Showtape: " + url + " (Length: " + countlength / manager.dataStreamedFPS + ")");
            else
                Debug.Log("Playing Showtape: " + url + " (Length: " + countlength / manager.dataStreamedFPS + ")");
            yield return null;

            //Finalize
            manager.timeSongStarted = Time.time;
            manager.syncTvsAndSpeakers.Invoke();
            Debug.Log("Length = " + manager.referenceSpeaker.clip.length + " Channels = " +
                      manager.referenceSpeaker.clip.channels + " Total = " + manager.referenceSpeaker.clip.length /
                      manager.referenceSpeaker.clip.channels);
        }
    }

    public void EraseShowtape()
    {
        manager.disableCharactersOnStart = false;
        manager.playMovements = false;
        manager.referenceSpeaker.time = 0;
        manager.timeSongStarted = 0;
        manager.timeSongOffset = 0;
        manager.timePauseStart = 0;
        manager.timeInputSpeedStart = 0;
        manager.curtainOpen.Invoke();
        manager.speakerClip = null;
        manager.rshwData = null;
        manager.videoPath = "";
        manager.referenceSpeaker.clip = null;
    }

    public void ReplaceShowAudio()
    {
        //Call File Browser
        manager.showtapeSegmentPaths = new string[1];
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape", "", manager.fileExtention, false);
        if (paths.Length > 0)
        {
            manager.showtapeSegmentPaths[0] = paths[0];
            manager.currentShowtapeSegment = 0;
            manager.referenceSpeaker.time = 0;
            manager.playMovements = false;
            //Check if null
            if (manager.showtapeSegmentPaths[0] != "")
            {
                rshwFormat thefile = rshwFormat.ReadFromFile(manager.showtapeSegmentPaths[0]);
                var newSignals = new List<BitArray>();
                int countlength = 0;
                if (thefile.signalData[0] != 0)
                {
                    countlength = 1;
                    BitArray bit = new(300);
                    newSignals.Add(bit);
                }

                for (int i = 0; i < thefile.signalData.Length; i++)
                    if (thefile.signalData[i] == 0)
                    {
                        countlength += 1;
                        BitArray bit = new(300);
                        newSignals.Add(bit);
                    }
                    else
                    {
                        newSignals[countlength - 1].Set(thefile.signalData[i] - 1, true);
                    }

                manager.rshwData = newSignals.ToArray();

                paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", "wav", false);
                if (paths.Length > 0)
                    if (paths[0] != "")
                    {
                        manager.wavPath = paths[0];
                        manager.speakerClip = OpenWavParser.ByteArrayToAudioClip(File.ReadAllBytes(paths[0]));
                        SaveRecording();
                    }
            }
        }
    }
}