using System;
using System.Linq;
using Show;
using UnityEngine;
using Light = Show.Light;

[Serializable]
public class Stage
{

    public string name;
    public GameObject stageGameObject;
    
    [HideInInspector] public AnimatronicData[] animatronics;
    [HideInInspector] public PropData[] props;
    [HideInInspector] public Curtains curtains;
    [HideInInspector] public AudioSource[] speakers;

    public ShowTV[] tvs;

    public void Startup()
    {
        // Setup Curtains
        curtains = stageGameObject.GetComponentInChildren<Curtains>(true);
        if (curtains != null)   
            curtains.gameObject.SetActive(true);

        // Setup Animatronics
        {
            GameObject animatronicsContainer = stageGameObject.transform.Find("Animatronics").gameObject;
    
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
        

        // Setup Props
        GameObject propsContainer = stageGameObject.transform.Find("Stage/Props").gameObject;
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
        
        // Setup Speakers
        speakers = stageGameObject.GetComponentsInChildren<AudioSource>()
            .Where(t => t.CompareTag("Speaker"))
            .ToArray();

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