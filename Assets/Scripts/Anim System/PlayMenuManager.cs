using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayMenuManager : MonoBehaviour
{
    public GameObject stopGraphic;
    public GameObject playGraphic;
    public GameObject loopGraphic;
    public GameObject loopPlayGraphic;
    public GameObject loopSongGraphic;
    public GameObject finishButton;
    public DF_ShowManager playUI;
    public Text nowTimeText;
    public Text endTimeText;
    public TMP_Text authorText;
    public TMP_Text titleText;

    public Slider playSlider;

    // Start is called before the first frame update
    public GameObject[] keepSizedList;

    private int endtexttimer;

    // Update is called once per frame
    private void LateUpdate()
    {
        if (playUI.manager.playMovements)
        {
            if (playUI.manager.referenceSpeaker.isPlaying)
            {
                if (stopGraphic.activeSelf) stopGraphic.SetActive(false);
                if (!playGraphic.activeSelf) playGraphic.SetActive(true);
            }
            else
            {
                if (!stopGraphic.activeSelf) stopGraphic.SetActive(true);
                if (playGraphic.activeSelf) playGraphic.SetActive(false);
            }

            switch (playUI.manager.songLoopSetting)
            {
                case DF_ShowtapeManager.LoopVers.noLoop:
                    if (!loopGraphic.activeSelf) loopGraphic.SetActive(true);
                    if (loopPlayGraphic.activeSelf) loopPlayGraphic.SetActive(false);
                    if (loopSongGraphic.activeSelf) loopSongGraphic.SetActive(false);
                    break;
                case DF_ShowtapeManager.LoopVers.loopPlaylist:
                    if (loopGraphic.activeSelf) loopGraphic.SetActive(false);
                    if (!loopPlayGraphic.activeSelf) loopPlayGraphic.SetActive(true);
                    if (loopSongGraphic.activeSelf) loopSongGraphic.SetActive(false);
                    break;
                case DF_ShowtapeManager.LoopVers.loopSong:
                    if (loopGraphic.activeSelf) loopGraphic.SetActive(false);
                    if (loopPlayGraphic.activeSelf) loopPlayGraphic.SetActive(false);
                    if (!loopSongGraphic.activeSelf) loopSongGraphic.SetActive(true);
                    break;
            }

            int nowtimer = (int)Mathf.Floor(playUI.manager.referenceSpeaker.time);
            if (playUI.manager.useVideoAsReference)
                playSlider.value = (float)playUI.manager.referenceVideo.time /
                    ((float)playUI.manager.referenceVideo.length /
                     playUI.manager.referenceVideo.GetAudioChannelCount(0)) * 100;
            else
                playSlider.value = playUI.manager.referenceSpeaker.time / playUI.manager.speakerClip.length * 100;

            if (nowtimer <= (float)endtexttimer)
                nowTimeText.text = Mathf.Floor(nowtimer / 60).ToString("00") + ":" + (nowtimer % 60).ToString("00");
        }
    }

    public void ClickSlider(int section)
    {
        if (playUI.manager.useVideoAsReference)
            playUI.manager.referenceVideo.time = section / 10.0f * ((float)playUI.manager.referenceVideo.length /
                                                                    playUI.manager.referenceVideo.GetAudioChannelCount(
                                                                        0));
        else
            playUI.manager.referenceSpeaker.time = section / 10.0f * playUI.manager.speakerClip.length;

        playUI.manager.syncTvsAndSpeakers.Invoke();
    }

    public void TextUpdate(bool record)
    {
        if (!playUI.manager.recordMovements)
        {
            transform.localScale = Vector3.one;
            foreach (Transform child in transform)
            {
                Button3D check = child.GetComponent<Button3D>();
                if (check != null) check.enabled = true;
                child.transform.localScale = Vector3.one;
            }

            finishButton.SetActive(false);
            if (playUI.manager.useVideoAsReference)
                endtexttimer = (int)((float)playUI.manager.referenceVideo.length /
                                     playUI.manager.referenceVideo.GetAudioChannelCount(0));
            else
                endtexttimer = (int)playUI.manager.speakerClip.length;
            endTimeText.text = Mathf.Floor(endtexttimer / 60.0f).ToString("00") + ":" +
                               (endtexttimer % 60).ToString("00");

            //Title
            if (playUI.manager.useVideoAsReference)
            {
                titleText.text = "Youtube Video";
                authorText.text = "Test";
            }
            else
            {
                string[] combined = Path
                    .GetFileName(playUI.manager.showtapeSegmentPaths[playUI.manager.currentShowtapeSegment])
                    .Split(new[] { " - " }, StringSplitOptions.None);

                if (combined.Length > 1)
                {
                    titleText.text = combined[0];
                    authorText.text = combined[1].Substring(0, combined[1].Length - 5);
                }
                else
                {
                    titleText.text = combined[0].Substring(0, combined[0].Length - 5);
                    authorText.text = "";
                }
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                bool fafa = false;
                for (int i = 0; i < keepSizedList.Length; i++)
                    if (child.gameObject == keepSizedList[i])
                        fafa = true;

                if (fafa)
                {
                    Button3D check = child.GetComponent<Button3D>();
                    if (check != null) check.enabled = false;
                    child.transform.localScale = Vector3.zero;
                }
            }

            endtexttimer = (int)playUI.manager.speakerClip.length;
            endTimeText.text = Mathf.Floor(endtexttimer / 60.0f).ToString("00") + ":" +
                               (endtexttimer % 60).ToString("00");

            string[] combined = Path
                .GetFileName(playUI.manager.showtapeSegmentPaths[playUI.manager.currentShowtapeSegment])
                .Split(new[] { " - " }, StringSplitOptions.None);

            if (combined.Length > 1)
            {
                titleText.text = combined[0];
                authorText.text = combined[1].Substring(0, combined[1].Length - 5);
            }
            else
            {
                titleText.text = combined[0].Substring(0, combined[0].Length - 5);
                authorText.text = "";
            }

            finishButton.SetActive(true);
        }
    }

    public void SwapIcon(bool pause)
    {
        if (!pause)
        {
            playGraphic.SetActive(true);
            stopGraphic.SetActive(false);
        }
        else
        {
            playGraphic.SetActive(false);
            stopGraphic.SetActive(true);
        }
    }

    public void SwapLoop(int ingoing)
    {
        if (ingoing == 0)
        {
            loopGraphic.SetActive(true);
            loopPlayGraphic.SetActive(false);
            loopSongGraphic.SetActive(false);
        }
        else if (ingoing == 1)
        {
            loopGraphic.SetActive(false);
            loopPlayGraphic.SetActive(true);
            loopSongGraphic.SetActive(false);
        }
        else
        {
            loopGraphic.SetActive(false);
            loopPlayGraphic.SetActive(false);
            loopSongGraphic.SetActive(true);
        }
    }
}