using System;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Light controller script. This allows many properties of a Light to be manipulated by the show controller.
/// </summary>

[RequireComponent(typeof(Light))]
public class LightController : MonoBehaviour
{
    [FormerlySerializedAs("lightBit")]
    public int bit;
    public char topOrBottom;
    
    [Range(0.01f, 1f)] public float fadeSpeed;

    
    public float intensity;
    

    public enum SpecialMode
    {
        None = 0,
        Helicopter,
        CuStar,
        EmissiveMaterial,
    }
    public SpecialMode specialMode;
    
    public bool strobe;
    public bool flash;

    public bool materialLight;
    public GameObject emissiveObject;
    public string emmissiveMatName;
    public float emissiveMultiplier = 1;
    public Color emissiveMatColor = Color.white;
    public bool invertBit;
    public bool materialStars;
    public Texture2D[] starCookies;
    private float acceleration;
    private MacValves bitChart;
    private Light currentLight;
    private int currentTextureSet;

    private Material emissiveTexture;

    //Values
    private bool flashCheck;
    private float nextTime;
    private float speed;
    private bool textureSet;

    private bool errorOccured;

    
    [Header("Deprecated - Use Intensity instead")]
    [Obsolete("Intensity Multiplier should not be used, use Intensity instead")]
    public float intensityMultiplier;


    private void Start()
    {
        bitChart = transform.root.Find("Mac Valves").GetComponent<MacValves>();
        if (!materialLight)
            currentLight = GetComponent<Light>();
        else
            foreach (Material matt in emissiveObject.GetComponent<MeshRenderer>().materials)
                if (matt.name == emmissiveMatName)
                {
                    emissiveTexture = matt;
                    emissiveTexture.EnableKeyword("_EMISSIVE_COLOR_MAP");
                }

        if (materialStars) emissiveObject.SetActive(false);
        
        if (intensityMultiplier != 0)
        {
            intensity = intensity * intensityMultiplier;
        }
    }

    public void CreateMovements(float num3)
    {
        if (errorOccured == false)
        {
            try
            {
                bool onOff = false;
                if (topOrBottom == 'T' && bitChart.topDrawer[bit - 1])
                    onOff = true;
                else if (topOrBottom == 'B' && bitChart.bottomDrawer[bit - 1]) onOff = true;
                if (invertBit) onOff = !onOff;
                if (flash)
                {
                    if (onOff)
                    {
                        if (!flashCheck)
                        {
                            flashCheck = true;
                            nextTime = 1;
                        }
                        else
                        {
                            nextTime -= fadeSpeed * num3;
                        }
                    }
                    else
                    {
                        if (flashCheck) flashCheck = false;
                        nextTime -= fadeSpeed * num3;
                    }
                }
                else if (strobe)
                {
                    if (onOff)
                    {
                        if (nextTime != 0)
                            nextTime -= fadeSpeed * 2 * num3;
                        else
                            nextTime = 1;
                    }
                    else
                    {
                        nextTime -= fadeSpeed * 2 * num3;
                    }
                }
                else
                {
                    if (onOff)
                        nextTime += fadeSpeed * num3;
                    else
                        nextTime -= fadeSpeed * num3;
                }

                nextTime = Mathf.Min(Mathf.Max(nextTime, 0), 1);
                if (!materialLight)
                {
                    currentLight.intensity = intensity * nextTime;
                }
                else if (!materialStars)
                {
                    emissiveTexture.SetColor("_EmissiveColor",
                        emissiveMatColor * nextTime * emissiveMultiplier);
                }
                else
                {
                    if (onOff && !emissiveObject.activeSelf) emissiveObject.SetActive(true);
                    if (!onOff && emissiveObject.activeSelf) emissiveObject.SetActive(false);
                }

                if (specialMode == SpecialMode.Helicopter)
                {
                    if (onOff)
                    {
                        acceleration = Mathf.Max(50, acceleration + Time.deltaTime);
                    }
                    else
                    {
                        if (speed > 0)
                            acceleration = Mathf.Min(-50, acceleration + Time.deltaTime / 8.0f);
                        else
                            acceleration = 0;
                    }

                    speed = Mathf.Clamp(acceleration * Time.deltaTime + speed, 0, 40);
                    transform.Rotate(new Vector3(0, 0, speed * Time.deltaTime * 8.0f));
                }

                if (specialMode == SpecialMode.CuStar)
                {
                    //Acceleration = time between patterns
                    //Flashcheck = pattern swap
                    //speed = cookie check
                    if (onOff) acceleration += Time.deltaTime;


                    if (acceleration > 10.0f)
                    {
                        acceleration = 0;
                        flashCheck = !flashCheck;
                        textureSet = false;
                    }

                    //Flash or Burst pattern?
                    if (!flashCheck)
                    {
                        //(int)Speed = Texture Frame
                        if (speed > 4) speed = 0;
                        //Move to next texture frame when speed > current image
                        if (currentTextureSet != Mathf.FloorToInt(speed) + 1)
                        {
                            currentLight.cookie = starCookies[Mathf.FloorToInt(speed) + 1];
                            currentTextureSet = Mathf.FloorToInt(speed) + 1;
                        }

                        //Modulate light intensity by sine wave
                        currentLight.intensity = Mathf.Clamp(Mathf.Sin(speed * Mathf.PI * 2 - Mathf.PI / 2.0f) + 1, 0.5f, 1) *
                                                 intensity * nextTime;

                        //Advance Speed
                        speed += Time.deltaTime * 15;
                    }
                    else
                    {
                        //Advance Speed
                        speed += Time.deltaTime;
                        //Change to flash frame if not
                        if (!textureSet) currentLight.cookie = starCookies[0];
                        textureSet = true;
                        //Modulate light intensity by sine wave
                        currentLight.intensity =
                            (Mathf.Sin(speed * 40 - 0.5f) + 1) * intensity * nextTime;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Light Controller error at " + gameObject.name + ": " + ex.Message);
                errorOccured = true;
            }
        }
    }
}