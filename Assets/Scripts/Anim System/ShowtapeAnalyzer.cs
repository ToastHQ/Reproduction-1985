using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ShowtapeAnalyzer : MonoBehaviour
{
    public DF_ShowtapeManager manager;
    public TimelineEditor editor;
    public GameObject loadingScreen;
    public GameObject analyzeScreen;
    public loading_screen loading;
    public Text analyzeHeader;
    public Text analyzeBody;
    public int exportAllSignals = 1;
    private readonly uint[] maplut = { 10, 11, 0, 1, 2, 3, 4, 12, 13, 5, 6, 7, 8, 9, 14, 15 };
    private readonly uint[] maplut2 = { 2, 4, 5, 0, 3, 1, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
    private bool cancelLoad;

    public void StartAnalysis(string type)
    {
        StartCoroutine(StartAnalysisCorou(type));
    }

    public void CancelLoad()
    {
        cancelLoad = true;
    }

    private IEnumerator StartAnalysisCorou(string type)
    {
        Debug.Log("Attempt analysis");
        loadingScreen.SetActive(true);
        loading.current = 0;
        loading.loadingMessage = "";
        loading.maximum = 1;
        yield return null;
        switch (type)
        {
            case "CompareTotalOn":
            {
                float[] results = new float[300];
                int[] bits = new int[300];
                IEnumerator coroutine = CompareTotalOn(manager.rshwData, results, bits);
                StartCoroutine(coroutine);
                while (loading.current < loading.maximum)
                {
                    if (cancelLoad) break;
                    yield return null;
                }

                if (cancelLoad)
                {
                    cancelLoad = false;
                    loading.current = 0;
                    loading.loadingMessage = "";
                    loading.maximum = 1;
                    loadingScreen.SetActive(false);
                    StopCoroutine(coroutine);
                    break;
                }

                yield return new WaitForSeconds(0.1f);
                string header = "Compare total amount of times a bit is on for \"" +
                                Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\".";
                string body = "";
                for (int i = 0; i < results.Length; i++)
                    body += editor.windowMaker.SearchBitChartName(bits[i]) + "  >  " + results[i] + " seconds\n";
                OpenWindow(header, body);
                break;
            }
            case "CompareLongestOn":
            {
                float[] results = new float[300];
                int[] bits = new int[300];
                IEnumerator coroutine = CompareLongestOn(manager.rshwData, results, bits);
                StartCoroutine(coroutine);
                while (loading.current < loading.maximum)
                {
                    if (cancelLoad) break;
                    yield return null;
                }

                if (cancelLoad)
                {
                    cancelLoad = false;
                    loading.current = 0;
                    loading.loadingMessage = "";
                    loading.maximum = 1;
                    loadingScreen.SetActive(false);
                    StopCoroutine(coroutine);
                    break;
                }

                yield return new WaitForSeconds(0.1f);
                string header = "Compare longest time a bit is on for \"" +
                                Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\".";
                string body = "";
                for (int i = 0; i < results.Length; i++)
                    body += editor.windowMaker.SearchBitChartName(bits[i]) + "  >  " + results[i] + " seconds\n";
                OpenWindow(header, body);
                break;
            }
            case "CompareTimesOn":
            {
                float[] results = new float[300];
                int[] bits = new int[300];
                IEnumerator coroutine = CompareTimesOn(manager.rshwData, results, bits);
                StartCoroutine(coroutine);
                while (loading.current < loading.maximum)
                {
                    if (cancelLoad) break;
                    yield return null;
                }

                if (cancelLoad)
                {
                    cancelLoad = false;
                    loading.current = 0;
                    loading.loadingMessage = "";
                    loading.maximum = 1;
                    loadingScreen.SetActive(false);
                    StopCoroutine(coroutine);
                    break;
                }

                yield return new WaitForSeconds(0.1f);
                string header = "Compare every time a bit is on for \"" +
                                Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\".";
                string body = "";
                for (int i = 0; i < results.Length; i++)
                    body += editor.windowMaker.SearchBitChartName(bits[i]) + "  >  " + results[i] + " times pressed\n";
                OpenWindow(header, body);
                break;
            }
            case "ExportAllSignalsInBit":
            {
                string body = "";
                loading.loadingMessage = "(0%)";
                int e = 0;
                loading.maximum = manager.rshwData.Length;
                for (int i = 0; i < manager.rshwData.Length; i++)
                {
                    body += manager.rshwData[i].Get(exportAllSignals) ? 1 : 0;
                    loading.current = i;
                    if (i % (manager.rshwData.Length / 100) == 1)
                    {
                        e++;
                        loading.loadingMessage = "(" + e + "%)";
                        yield return null;
                    }
                }

                loading.loadingMessage = "(100%)";
                yield return null;
                if (cancelLoad)
                {
                    cancelLoad = false;
                    loading.current = 0;
                    loading.loadingMessage = "";
                    loading.maximum = 1;
                    loadingScreen.SetActive(false);
                    break;
                }

                yield return new WaitForSeconds(0.1f);
                analyzeBody.text = body;
                loadingScreen.SetActive(false);
                SaveWindowText();
                break;
            }
            case "Extra Info":
            {
                string header = "Extra info for \"" + Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\".";
                string body = "";
                loading.loadingMessage = "(0%)";
                yield return null;
                for (int i = 0; i < 4; i++)
                {
                    loading.loadingMessage = "(" + i / (float)5 + "%)";
                    switch (i)
                    {
                        case 0:
                            body += "Name: " + Path.GetFileName(manager.showtapeSegmentPaths[0]) + "\n";
                            break;
                        case 1:
                            body += "Signals Per Second: " + manager.dataStreamedFPS + "\n";
                            break;
                        case 2:
                            body += "Audio Length: " + editor.audioLengthMax + "\n";
                            break;
                        case 3:
                            body += "Average Button Presses Per Second: " +
                                    AverageBitsPerSecond(manager.rshwData, editor.audioLengthMax) + "\n";
                            break;
                    }

                    if (cancelLoad) break;
                    yield return null;
                }

                if (cancelLoad)
                {
                    cancelLoad = false;
                    loading.current = 0;
                    loading.loadingMessage = "";
                    loading.maximum = 1;
                    loadingScreen.SetActive(false);
                    break;
                }

                yield return new WaitForSeconds(0.1f);
                OpenWindow(header, body);
                break;
            }
            case "Import APS":
            {
                IEnumerator coroutine = ImportAPS();
                StartCoroutine(coroutine);
                while (loading.current < loading.maximum)
                {
                    if (cancelLoad) break;
                    yield return null;
                }

                if (cancelLoad)
                {
                    cancelLoad = false;
                    loading.current = 0;
                    loading.loadingMessage = "";
                    loading.maximum = 1;
                    loadingScreen.SetActive(false);
                    StopCoroutine(coroutine);
                    break;
                }

                yield return new WaitForSeconds(0.1f);
                loadingScreen.SetActive(false);
                editor.RepaintTimeline();
                break;
            }
            case "Import RR":
            {
                IEnumerator coroutine = ImportRR();
                StartCoroutine(coroutine);
                while (loading.current < loading.maximum)
                {
                    if (cancelLoad) break;
                    yield return null;
                }

                if (cancelLoad)
                {
                    cancelLoad = false;
                    loading.current = 0;
                    loading.loadingMessage = "";
                    loading.maximum = 1;
                    loadingScreen.SetActive(false);
                    StopCoroutine(coroutine);
                    break;
                }

                yield return new WaitForSeconds(0.1f);
                loadingScreen.SetActive(false);
                editor.RepaintTimeline();
                break;
            }
            case "CEC Rosetta":
            {
                IEnumerator coroutine = CECRosetta(false);
                StartCoroutine(coroutine);
                while (loading.current < loading.maximum)
                {
                    if (cancelLoad) break;
                    yield return null;
                }

                if (cancelLoad)
                {
                    cancelLoad = false;
                    loading.current = 0;
                    loading.loadingMessage = "";
                    loading.maximum = 1;
                    loadingScreen.SetActive(false);
                    StopCoroutine(coroutine);
                    break;
                }

                yield return new WaitForSeconds(0.1f);
                loadingScreen.SetActive(false);
                editor.RepaintTimeline();
                break;
            }
        }
    }

    private float AverageBitsPerSecond(BitArray[] showTape, float length)
    {
        float times = 0;
        for (int i = 0; i < 300; i++)
        {
            bool previous = false;
            for (int e = 0; e < showTape.Length; e++)
                if (showTape[e].Get(i))
                {
                    if (!previous) times++;
                    previous = true;
                }
                else
                {
                    previous = false;
                }
        }

        return times / length;
        ;
    }

    private IEnumerator CompareTotalOn(BitArray[] showTape, float[] results, int[] bits)
    {
        loading.maximum = 300;
        loading.text.text = "(0/2)";
        for (int i = 0; i < 300; i++)
        {
            float frames = 0;
            for (int e = 0; e < showTape.Length; e++)
                if (showTape[e].Get(i))
                    frames++;

            bits[i] = i + 1;
            results[i] = frames / manager.dataStreamedFPS;
            loading.current = i + 1;
            if (i == 150)
            {
                loading.text.text = "(1/2)";
                yield return null;
            }
        }

        Array.Sort(results, bits);
        Array.Reverse(results);
        Array.Reverse(bits);
        loading.text.text = "(2/2)";
        yield return null;
        loading.current = loading.maximum;
        yield return null;
    }

    private IEnumerator CompareLongestOn(BitArray[] showTape, float[] results, int[] bits)
    {
        loading.maximum = 300;
        loading.text.text = "(0/2)";
        for (int i = 0; i < 300; i++)
        {
            float current = 0;
            float longest = 0;
            for (int e = 0; e < showTape.Length; e++)
                if (showTape[e].Get(i))
                {
                    current++;
                }
                else
                {
                    if (current > longest) longest = current;
                    current = 0;
                }

            bits[i] = i + 1;
            results[i] = longest / manager.dataStreamedFPS;
            loading.current = i + 1;
            if (i == 150)
            {
                loading.text.text = "(1/2)";
                yield return null;
            }
        }

        Array.Sort(results, bits);
        Array.Reverse(results);
        Array.Reverse(bits);
        loading.text.text = "(2/2)";
        yield return null;
        loading.current = loading.maximum;
        yield return null;
    }

    private IEnumerator CompareTimesOn(BitArray[] showTape, float[] results, int[] bits)
    {
        loading.maximum = 300;
        loading.text.text = "(0/2)";
        for (int i = 0; i < 300; i++)
        {
            float times = 0;
            bool previous = false;
            for (int e = 0; e < showTape.Length; e++)
                if (showTape[e].Get(i))
                {
                    if (!previous) times++;
                    previous = true;
                }
                else
                {
                    previous = false;
                }

            bits[i] = i + 1;
            results[i] = times;
            loading.current = i + 1;
            if (i == 150)
            {
                loading.text.text = "(1/2)";
                yield return null;
            }
        }

        Array.Sort(results, bits);
        Array.Reverse(results);
        Array.Reverse(bits);
        loading.text.text = "(2/2)";
        yield return null;
        loading.current = loading.maximum;
        yield return null;
    }

    private IEnumerator ImportAPS()
    {
        Debug.Log("APS Import");
        loading.maximum = 300;
        loading.current = 0;
        loading.text.text = "(0/2)";

        string[] paths = StandaloneFileBrowser.OpenFilePanel("Browse APS File", "", "", false);
        if (paths.Length > 0)
            if (paths[0] != "")
            {
                //Load File
                loading.text.text = "Loading APS";
                yield return null;
                byte[] aps = File.ReadAllBytes(paths[0]);
                byte[] newaps = new byte[aps.Length];
                loading.text.text = "Converting Loot Table";
                yield return null;

                //Shift through the secret RAE bitswap
                for (int k = 1024; k < aps.Length - 1024; k += 16)
                for (int b = 0; b < 16; b++)
                    newaps[k + b] = aps[k + maplut[b]];

                //Shift through the secret RAE bitswap TWICE
                for (int k = 1024; k < aps.Length - 1024; k += 16)
                for (int b = 0; b < 16; b++)
                    aps[k + b] = newaps[k + maplut2[b]];

                newaps = aps;
                loading.text.text = "Loading Bits";
                yield return null;

                //Convert from byte array to bit array
                BitArray bits = new(newaps);

                //Chop off the beginning 4096 bits and erase all 31 and 32 "frame"
                BitArray newBits = new(bits.Length);
                int x = 0;
                for (int i = 0; i * 256 < bits.Length - 4096; i++)
                    if (i % 32 < 30)
                        for (int signal = 0; signal < 256; signal++)
                        {
                            newBits.Set(x, bits[i * 256 + signal + 4096]);
                            x++;
                        }

                bits = newBits;

                //Begin the conversion
                loading.text.text = "(0/" + (int)(newBits.Length * 0.9375f) + ")";
                loading.maximum = (int)(newBits.Length * 0.9375f);
                yield return null;
                int frame = 0;

                bool done = false;
                while (!done)
                {
                    //What's the exact current APS showtape frame we're on
                    int dataspot = Mathf.RoundToInt(frame / manager.dataStreamedFPS * 29.97f);

                    //Fix for if the current show's signal array
                    //is smaller than what needs to be imported.
                    while (frame > manager.rshwData.Length - 1)
                        manager.rshwData = manager.rshwData.Append(new BitArray(300)).ToArray();

                    //What bit in the APS file are we at
                    int bitSpot = dataspot * 256;

                    //Loading screen yield
                    if (frame % 10000 == 0)
                    {
                        loading.text.text = "(" + bitSpot + "/" + (int)(bits.Length * 0.9375f) + ")";
                        loading.current = frame;
                        yield return null;
                    }

                    if (bitSpot > bits.Length - 1)
                        //Break out of loop once APS file is done
                        done = true;
                    else
                        //Apply the APS bit to the current frame
                        for (int e = 0; e < 256; e++)
                            if (e <= 128)
                                //Top Drawer
                                manager.rshwData[frame].Set(e, bits.Get(bitSpot + e));
                            else
                                //Bottom Drawer
                                manager.rshwData[frame].Set(e + 22, bits.Get(bitSpot + e));


                    frame++;
                }

                yield return null;
            }

        loading.text.text = "(100/100)";
        yield return null;
        loading.current = loading.maximum;
        yield return null;
    }

    private IEnumerator ImportRR()
    {
        Debug.Log("RR Import");
        loading.maximum = 300;
        loading.current = 0;
        loading.text.text = "(0/2)";

        //Call File Browser
        string[] paths;
        if (manager.fileExtention == "")
        {
            ExtensionFilter[] extensions;

            extensions = new[] { new ExtensionFilter("Show Files", "cshw", "sshw", "rshw", "nshw") };

            paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", extensions, false);
        }
        else
        {
            paths = StandaloneFileBrowser.OpenFilePanel("Browse Showtape Audio", "", manager.fileExtention, false);
        }

        if (paths.Length > 0)
            if (paths[0] != "")
            {
                //Load File
                rshwFormat thefile = rshwFormat.ReadFromFile(paths[0]);
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

                //Begin the conversion
                loading.text.text = "(0/" + newSignals.Count + ")";
                loading.maximum = newSignals.Count;
                yield return null;
                int frame = (int)(manager.referenceSpeaker.time * manager.dataStreamedFPS);
                int importFrame = 0;
                bool done = false;
                while (!done)
                {
                    //Fix for if the current show's signal array
                    //is smaller than what needs to be imported.
                    while (frame > manager.rshwData.Length - 1)
                        manager.rshwData = manager.rshwData.Append(new BitArray(300)).ToArray();

                    //Loading screen yield
                    if (frame % 10000 == 0)
                    {
                        loading.text.text = "(" + importFrame + "/" + newSignals.Count + ")";
                        loading.current = frame;
                        yield return null;
                    }

                    if (importFrame > newSignals.Count - 1)
                        //Break out of loop once APS file is done
                        done = true;
                    else
                        manager.rshwData[frame] = newSignals[importFrame];

                    importFrame++;
                    frame++;
                }

                yield return null;
            }

        loading.text.text = "(100/100)";
        yield return null;
        loading.current = loading.maximum;
        yield return null;
    }

    private IEnumerator CECRosetta(bool reverse)
    {
        Debug.Log("CEC Rosetta");
        loading.maximum = manager.rshwData.Length;
        loading.current = 0;
        loading.text.text = "(Processing)";

        //Lut Table, 0 = not used, negative numbers mean invert the bit
        int[] lootTop =
        {
            1, -6, 0, 8, 5, 2, 3, -4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 0, 35, 0, 0, 0, 34, -38, 0, 40, 37, 33, 39, 0, 0,
            0, 36, 0, 0, 0, 0, 0, -54, 0, 56, 53, 49, 0, 0, 0, 10, 0,
            0, 0, 0, 50, 51, 0, 0, 0, 52, 55, 0, 0, 0, 0, 0, 0, 110, 108, 0, 0, 101, 106, 111, 0, 0, 0, 123, 0, 0, 0,
            123, 107, 0, 100, 109, 102, 122, 104
        };
        int[] lootBottom =
        {
            -22, 0, 0, 0, 0, 18, 19, -20, 0, 0, 0, 0, 23, 0, 0, 17, 0, 0, 71, 0, 0, 0, 0, 0, 0, 0, 0, 66, 67, -68, -70,
            0, 72, 69, 65, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 73, 0, 0,
            0, 0, 0, 103, 119, 97, 117, 118, 121, 0, 0, 0, 0, 0, 0, 113, 0, 115, 114, 0, 0, 0, 0, 124, 99, 0, 0, 105,
            125, 0
        };
        BitArray temp = new(300);
        for (int i = 0; i < manager.rshwData.Length; i++)
        {
            temp.SetAll(false);
            for (int e = 0; e < lootTop.Length; e++)
                if (lootTop[e] != 0)
                {
                    bool set = manager.rshwData[i].Get(e);
                    if (lootTop[e] < 0) set = !set;
                    temp.Set(Mathf.Abs(lootTop[e]) - 1, set);
                }

            for (int e = 0; e < lootBottom.Length; e++)
                if (lootBottom[e] != 0)
                {
                    bool set = manager.rshwData[i].Get(e + 150);
                    if (lootBottom[e] < 0) set = !set;
                    temp.Set(Mathf.Abs(lootBottom[e]) - 1, set);
                }

            //Jasper's eyes, since its a double assinged bit
            temp.Set(23, manager.rshwData[i].Get(155));
            temp.Set(20, manager.rshwData[i].Get(156));

            for (int e = 0; e < 300; e++) manager.rshwData[i].Set(e, temp.Get(e));

            if (i % 10000 == 0) loading.current = i;
        }

        yield return null;

        loading.text.text = "(100/100)";
        yield return null;
        loading.current = loading.maximum;
        yield return null;
    }

    private void OpenWindow(string header, string body)
    {
        analyzeScreen.SetActive(true);
        loadingScreen.SetActive(false);
        analyzeHeader.text = header;
        analyzeBody.text = body;
    }

    public void CloseWindow()
    {
        analyzeScreen.SetActive(false);
    }

    public void SaveWindowText()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Save Analysis", "", "Analysis", "txt");
        if (!string.IsNullOrEmpty(path))
        {
            Debug.Log("Analysis Saved: " + Path.GetDirectoryName(path) + " >>>> " + Path.GetFileName(path));

            WriteFile(Path.GetDirectoryName(path) + "/", Path.GetFileName(path), analyzeBody.text);
        }
    }

    public static bool WriteFile(string path, string fileName, string data)
    {
        bool retValue = false;
        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText(path + fileName, data);
            retValue = true;
        }
        catch (Exception ex)
        {
            string ErrorMessages = "File Write Error\n" + ex.Message;
            retValue = false;
            Debug.LogError(ErrorMessages);
        }

        return retValue;
    }
}