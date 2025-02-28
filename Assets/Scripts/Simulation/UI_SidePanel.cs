using System.Collections;
using SFB;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UI_SidePanel : MonoBehaviour
{
    //Windows
    public GameObject WindowMenu;
    public GameObject WindowSound;
    public GameObject WindowShow;
    public GameObject WindowCamera;
    public GameObject WindowFlows;
    public GameObject WindowBitVis;

    //Text
    public Text psiText;
    public Text volumeText;
    public Text spacialText;
    public Text curtainText;
    public Text upperLightText;
    public Text camSmoothText;
    public Text soundvolumeText;
    public Text stagevolumeText;
    public Text signalSwapText;

    public string FileExtention;

    //Other
    public GameObject areaLights;
    public DF_ShowManager showPanelUI;
    public Static staticUI;

    public float[] copyPasteValues = new float[8];
    private int flowNumber;

    //Flow Controls
    private int flowProfile;

    private bool hidepanels;

    private void Awake()
    {
        showPanelUI.thePlayer = GameObject.Find("Player");
        AudioSource gg = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        soundvolumeText.GetComponent<Text>().text = Mathf.Ceil(gg.volume * 100).ToString();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (hidepanels)
            {
                hidepanels = !hidepanels;
                transform.parent.localPosition -= Vector3.one * 100;
                showPanelUI.transform.parent.localPosition -= Vector3.one * 100;
            }
            else
            {
                hidepanels = !hidepanels;
                transform.parent.localPosition += Vector3.one * 100;
                showPanelUI.transform.parent.localPosition += Vector3.one * 100;
            }
        }

        if (Input.GetKeyDown(KeyCode.BackQuote)) showPanelUI.pauseSong();
    }

    public void Upperlights(int input)
    {
        AudioSource sc = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        Resources.Load("ting");
        sc.clip = (AudioClip)Resources.Load("Tech Lights");
        sc.pitch = Random.Range(0.95f, 1.05f);
        sc.Play();
        if (areaLights.activeSelf)
        {
            upperLightText.GetComponent<Text>().text = "Off";
            areaLights.SetActive(false);
        }
        else
        {
            upperLightText.GetComponent<Text>().text = "On";
            areaLights.SetActive(true);
        }
    }

    public void SwapWindow(int input)
    {
        staticUI.flash = true;
        WindowCamera.SetActive(false);
        WindowMenu.SetActive(false);
        WindowShow.SetActive(false);
        WindowSound.SetActive(false);
        WindowFlows.SetActive(false);
        WindowBitVis.SetActive(false);
        switch (input)
        {
            case 0:
                WindowMenu.SetActive(true);
                break;
            case 1:
                WindowSound.SetActive(true);
                break;
            case 2:
                WindowShow.SetActive(true);
                break;
            case 3:
                WindowCamera.SetActive(true);
                break;
            case 5:
                WindowFlows.SetActive(true);
                break;
            case 6:
                WindowBitVis.SetActive(true);
                break;
        }
    }

    public void StageVolume(int input)
    {
        var yea = GetComponentsInChildren<InstrumentSound>();
        for (int i = 0; i < yea.Length; i++)
        {
            yea[i].volume = Mathf.Min(Mathf.Max(yea[i].volume + input * .05f, 0), 1);
            stagevolumeText.text = Mathf.Ceil(yea[i].volume * 100).ToString();
        }
    }

    public void SignalSwap(int input)
    {
        if (showPanelUI.signalChange == DF_ShowManager.SignalChange.normal)
        {
            signalSwapText.text = "On";
            switch (input)
            {
                case 0:
                    showPanelUI.signalChange = DF_ShowManager.SignalChange.PreCU;
                    break;
                case 1:
                    showPanelUI.signalChange = DF_ShowManager.SignalChange.PrePTT;
                    break;
            }
        }
        else
        {
            signalSwapText.text = "Off";
            showPanelUI.signalChange = DF_ShowManager.SignalChange.normal;
        }
    }

    public void SetSmoothCam(int input)
    {
        if (showPanelUI.thePlayer.GetComponent<Player>().enableCamSmooth)
        {
            camSmoothText.text = "Off";
            showPanelUI.thePlayer.GetComponent<Player>().enableCamSmooth = false;
        }
        else
        {
            camSmoothText.text = "On";
            showPanelUI.thePlayer.GetComponent<Player>().enableCamSmooth = true;
        }
    }

    public void AutoCurtains(int input)
    {
        for (int i = 0; i < showPanelUI.stages.Length; i++)
            if (showPanelUI.stages[i].curtainValves != null)
            {
                if (showPanelUI.stages[i].curtainValves.curtainOverride)
                {
                    curtainText.text = "Off";
                    showPanelUI.stages[i].curtainValves.curtainOverride = false;
                }
                else
                {
                    curtainText.text = "On";
                    showPanelUI.stages[i].curtainValves.curtainOverride = true;
                }
            }
    }

    public void PSIChange(int input)
    {
        showPanelUI.mackValves.GetComponent<MacValves>().PSI =
            Mathf.Max(5, showPanelUI.mackValves.GetComponent<MacValves>().PSI + input);
        psiText.text = showPanelUI.mackValves.GetComponent<MacValves>().PSI + " PSI";
    }

    public void MusicVolumeChange(int input)
    {
        for (int i = 0; i < showPanelUI.speakerR.Length; i++)
            showPanelUI.speakerR[i].GetComponent<AudioSource>().volume += input * .05f;
        for (int i = 0; i < showPanelUI.speakerL.Length; i++)
            showPanelUI.speakerL[i].GetComponent<AudioSource>().volume += input * .05f;
        volumeText.GetComponent<Text>().text =
            Mathf.Ceil(showPanelUI.speakerR[0].GetComponent<AudioSource>().volume * 100).ToString();
    }


    public void SoundVolumeChange(int input)
    {
        AudioSource gg = GameObject.Find("GlobalAudio").GetComponent<AudioSource>();
        AudioSource ga = GameObject.Find("GlobalAmbience").GetComponent<AudioSource>();
        gg.volume += input * .05f;
        ga.volume += input * .05f;
        soundvolumeText.GetComponent<Text>().text = Mathf.Ceil(gg.volume * 100).ToString();
    }

    public void SpacialToggle(int input)
    {
        if (showPanelUI.speakerR[0].GetComponent<AudioSource>().spatialBlend == 0)
        {
            for (int i = 0; i < showPanelUI.speakerR.Length; i++)
                showPanelUI.speakerR[i].GetComponent<AudioSource>().spatialBlend = 1;
            for (int i = 0; i < showPanelUI.speakerL.Length; i++)
                showPanelUI.speakerL[i].GetComponent<AudioSource>().spatialBlend = 1;
            spacialText.GetComponent<Text>().text = "On";
        }
        else
        {
            for (int i = 0; i < showPanelUI.speakerR.Length; i++)
                showPanelUI.speakerR[i].GetComponent<AudioSource>().spatialBlend = 0;
            for (int i = 0; i < showPanelUI.speakerL.Length; i++)
                showPanelUI.speakerL[i].GetComponent<AudioSource>().spatialBlend = 0;
            spacialText.GetComponent<Text>().text = "Off";
        }
    }
}