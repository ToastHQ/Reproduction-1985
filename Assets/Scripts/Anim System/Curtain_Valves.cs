using System.Collections.Generic;
using UnityEngine;

public class Curtain_Valves : MonoBehaviour
{

    [Range(0.0001f, .01f)] public float[] flowControlOut;

    [Range(0.0001f, .01f)] public float[] flowControlIn;

    public bool[] curtainbools;
    public bool curtainOverride;
    private readonly List<int> cylindersBottom = new();
    private readonly List<int> cylindersTop = new();
    private MacValves bitChart;

    private Animator characterValves;

    private void Start()
    {
        characterValves = GetComponent<Animator>();
        bitChart = gameObject.transform.root.GetComponentInChildren<MacValves>();
        for (int e = 0; e < characterValves.layerCount; e++)
        {
            string temp = characterValves.GetLayerName(e);
            if (temp[temp.Length - 1] == 'T')
                cylindersTop.Add(int.Parse(temp.Substring(0, temp.Length - 1)));
            else
                cylindersBottom.Add(int.Parse(temp.Substring(0, temp.Length - 1)));
        }

        curtainbools = new bool[characterValves.layerCount];
    }

    public void CreateMovements(float num3, bool reverse)
    {
        //Loop through cylinders to update
        for (int i = 0; i < cylindersTop.Count; i++)
            //Get current animation value
            SetTime("T", cylindersTop[i], bitChart.topDrawer, i, num3, reverse);
        for (int i = 0; i < cylindersBottom.Count; i++)
            //Get current animation value
            SetTime("B", cylindersBottom[i], bitChart.bottomDrawer, i, num3, reverse);
    }

    private void SetTime(string drawername, int currentAnim, bool[] drawer, int lasti, float num3, bool reverse)
    {
        //Cycle through parameters to find matching code
        for (int e = 0; e < characterValves.parameters.Length; e++)
            if (characterValves.parameters[e].name.Substring(0, characterValves.parameters[e].name.Length - 1) ==
                currentAnim.ToString())
            {
                //Calculate next value of the parameter
                float nextTime = characterValves.GetFloat(currentAnim + drawername);

                //Check if animation is already done
                if (!curtainOverride)
                {
                    bool check = drawer[currentAnim - 1];
                    if (reverse) check = !check;

                    if (nextTime == 0 && !check) break;
                    if (nextTime == 1 && check) break;
                }

                //Set bool
                if (drawer[currentAnim - 1])
                {
                    if (reverse)
                        curtainbools[lasti] = false;
                    else
                        curtainbools[lasti] = true;
                }
                else if (drawer[currentAnim + 1 - 1])
                {
                    if (reverse)
                        curtainbools[lasti] = true;
                    else
                        curtainbools[lasti] = false;
                }

                //Set Curtain
                if (!curtainOverride)
                {
                    if (curtainbools[lasti])
                        nextTime += flowControlOut[e] * 1.25f * num3;
                    else
                        nextTime -= flowControlIn[e] * 1.25f * num3;
                }
                else
                {
                    nextTime += flowControlOut[e] * 1.25f * num3;
                }

                nextTime = Mathf.Min(Mathf.Max(nextTime, 0), 1);

                //Apply parameter
                characterValves.SetFloat(currentAnim + drawername, nextTime);
                break;
            }
    }
}