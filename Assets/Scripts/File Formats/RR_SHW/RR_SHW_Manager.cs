using System.Collections;
using System.Collections.Generic;
using System.IO;
using eToile;
using SFB;
using UnityEngine;

/// <summary>
/// Handles all functionality related to the deprecated .*shw format
/// </summary>
public class RR_SHW_Manager : MonoBehaviour
{
    private DF_ShowtapeManager _manager;
    private DF_ShowController _showController;

    private void Awake()
    {
        _manager = GameObject.FindGameObjectWithTag("Showtape Manager").GetComponent<DF_ShowtapeManager>();
        _showController = GameObject.FindGameObjectWithTag("Show Controller").GetComponent<DF_ShowController>();
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
        _manager.disableCharactersOnStart = false;
        _manager.playMovements = false;
        //Check if null
        if (url != "")
        {
            _showController.referenceAudio.volume = _manager.refSpeakerVol;
            _showController.referenceAudio.time = 0;
            _manager.useVideoAsReference = false;
            _showController.referenceVideo.time = 0;
            _manager.timeSongStarted = 0;
            _manager.timeSongOffset = 0;
            _manager.timePauseStart = 0;
            _manager.timeInputSpeedStart = 0;
            yield return null;
            //Add code for opening .rshw file
            yield return null;
            rshwFormat thefile = rshwFormat.ReadFromFile(url);
            yield return null;
            _manager.speakerClip = OpenWavParser.ByteArrayToAudioClip(thefile.audioData);
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

            _manager.rshwData = newSignals.ToArray();
            yield return null;

            //Video
            if (File.Exists(url.Remove(url.Length - Mathf.Max(_manager.fileExtention.Length, 4)) + "mp4"))
            {
                Debug.Log("Video Found for Showtape.");
                _manager.videoPath = url.Remove(url.Length - Mathf.Max(_manager.fileExtention.Length, 4)) + "mp4";
            }
            else
            {
                _manager.videoPath = "";
            }

            _manager.audioVideoGetData.Invoke();
            yield return null;

            //Recording
            if (_manager.recordMovements)
                Debug.Log("Recording Showtape: " + url + " (Length: " + countlength / _manager.dataStreamedFPS + ")");
            else
                Debug.Log("Playing Showtape: " + url + " (Length: " + countlength / _manager.dataStreamedFPS + ")");
            yield return null;

            //Finalize
            _manager.timeSongStarted = Time.time;
            _manager.syncTvsAndSpeakers.Invoke();
            Debug.Log("Length = " + _showController.referenceAudio.clip.length + " Channels = " +
                      _showController.referenceAudio.clip.channels + " Total = " + _showController.referenceAudio.clip.length /
                      _showController.referenceAudio.clip.channels);
        }
    }
}