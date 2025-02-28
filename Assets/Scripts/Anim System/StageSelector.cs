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

    public GameObject animatronicsContainer;
    public GameObject propsContainer;
    
    public AnimatronicData[] animatronics;
    
    public PropData[] props;

    [HideInInspector] public LightController[] lightValves;

    public TurntableController[] tableValves;
    public TextureController texController;
    public ShowTV[] tvs;

    public void Startup()
    {
        // Setup Curtains
        if (curtain != null)
        {
            curtain.SetActive(true); // Enable curtain if you forgot to enable it lol
            curtainValves = curtain.GetComponent<Curtain_Valves>();
        }

        // Setup Animatronics
        {
            int animatronicCount = animatronicsContainer.transform.childCount;
            animatronics = new AnimatronicData[animatronicCount];

            for (int i = 0; i < animatronicCount; i++)
            {
                Transform animatronic = animatronicsContainer.transform.GetChild(i);

                var skinnedMeshRenderers = animatronic.GetComponentsInChildren<SkinnedMeshRenderer>(true);

                AnimatronicData animatronicData = new()
                {
                    name = animatronic.name,
                    currentCostume = 0,
                    gameObject = animatronic.gameObject,
                    costumes = new GameObject[skinnedMeshRenderers.Length] // initialize costumes array
                };

                for (int j = 0; j < skinnedMeshRenderers.Length; j++)
                {
                    animatronicData.costumes[j] = skinnedMeshRenderers[j].gameObject;
                }

                animatronics[i] = animatronicData;

                animatronic.GetComponentInChildren<Animatronic>().StartUp(); // Start up the character once setup
            }
        }
        
        // Set Lights
        lightValves = lights.transform.GetComponentsInChildren<LightController>();

        // Setup Props
        props = new PropData[propsContainer.transform.childCount];
        for (int i = 0; i < propsContainer.transform.childCount; i++)
        {
            Transform targetProp = propsContainer.transform.GetChild(i);

            var meshRenderers = targetProp.GetComponentsInChildren<MeshRenderer>(true);

            var targetPropData = new PropData()
            {
                name = targetProp.name,
                gameObject = targetProp.gameObject,
                variants = new GameObject[meshRenderers.Length]  // initialize the variants array
            };

            for (int v = 0; v < meshRenderers.Length; v++) 
            {
                targetPropData.variants[v] = meshRenderers[v].gameObject;
            }

            props[i] = targetPropData;
        }
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

[Serializable]
public class AnimatronicData
{
    public string name;
    public GameObject gameObject;
    public int currentCostume;
    public GameObject[] costumes;
    
    /// <summary>
    /// Adds all costumes to the costume array as GameObjects
    /// </summary>
    public void RefreshCostumes()
    {
        Array.Clear(costumes, 0, costumes.Length);
        
        var skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            costumes[i] = skinnedMeshRenderers[i].gameObject;
        }
    }
}

[Serializable]
public class PropData
{
    public string name;
    public GameObject gameObject;
    public int currentVariant;
    public GameObject[] variants;
}