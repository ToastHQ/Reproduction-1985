using System;
using UnityEngine;

[Serializable]
public class StageSelector
{
    public enum StageT
    {
        Standard,
        Mini,
        Retrofit,
        Unofficial
    }

    [Header("Stage")] public string stageName;

    public GameObject stage;

    [Header("Description")] public string stageDesc;

    public string stageDate;
    public Sprite stageIcon;
    public StageT stageType;
    
    [Header("Special Objects")] public GameObject curtain;

    [HideInInspector] public Curtain_Valves curtainValves;

    public GameObject lights;

    public GameObject animatronics;

    [HideInInspector] public LightController[] lightValves;

    public TurntableController[] tableValves;
    public TextureController texController;
    public ShowTV[] tvs;

    public void Startup()
    {
        //Curtains
        if (curtain != null)
        {
            curtain.SetActive(true); // Enable curtain if you forgot to enable it lol
            curtainValves = curtain.GetComponent<Curtain_Valves>();
        }

        // Set Lights
        lightValves = lights.transform.GetComponentsInChildren<LightController>();
    }
}


[Serializable]
public class ShowTV
{
    public enum tvSetting
    {
        offOnly,
        onOnly,
        offOn,
        none
    }

    public tvSetting tvSettings;
    public bool onWhenCurtain;
    public bool drawer;
    public int bitOff;
    public int bitOn;
    public MeshRenderer[] tvs;

    public int curtainSubState;
}