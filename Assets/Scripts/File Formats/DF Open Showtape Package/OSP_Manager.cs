using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eToile;
using UnityEngine;
using Newtonsoft.Json;

public class OSP_Manager : MonoBehaviour
{

    /// <summary>
    /// Writes an OpenShowtapePackage into a JSON encoded file
    /// </summary>
    /// <param name="filePath">Where the file should be written</param>
    /// <param name="showtape">The OpenShowtapePackage to use</param>
    public void Save(string filePath, OpenShowtapePackage showtape)
    {
        JsonSerializer serializer = new()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        using StreamWriter sw = new(filePath);
        using JsonWriter writer = new JsonTextWriter(sw);
        serializer.Serialize(writer, showtape);
    }

    /// <summary>
    /// Attempts to convert filePath into an OpenShowtapePackage
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="progress"></param>
    /// <returns></returns>
    public async Task<OpenShowtapePackage> ConvertFileAsync(string filePath, IProgress<float> progress = null)
{
    OpenShowtapePackage showtape = new();
    if (File.Exists(filePath))
    {
        string ex = Path.GetExtension(filePath);
        if (ex == ".rshw" || ex == ".cshw" || ex == ".sshw") // RR .*shw Converter
        {
            progress?.Report(1);
            showtape.metadata = new OspMetadata
            {
                title = Path.GetFileNameWithoutExtension(filePath),
                additionalInfo = $"Converted file from {ex}"
            };

            rshwFormat shw = await Task.Run(() => rshwFormat.Read(filePath));
            progress?.Report(10);
            showtape.audioData = new OspAudioData
            {
                data = Convert.ToBase64String(shw.audioData)
            };

            // Process signals with progress update
            showtape.frames = await Task.Run(() =>
            {
                var newSignals = new List<List<int>>();
                int countLength = 0;
                int totalSignals = shw.signalData.Length;

                if (totalSignals > 0 && shw.signalData[0] != 0)
                {
                    countLength = 1;
                    newSignals.Add(new List<int>());
                }

                for (int i = 0; i < totalSignals; i++)
                {
                    if (shw.signalData[i] == 0)
                    {
                        countLength += 1;
                        newSignals.Add(new List<int>());
                    }
                    else
                    {
                        newSignals[countLength - 1].Add(shw.signalData[i] - 1);
                    }

                    // Report progress (starts at 10%)
                    if (progress != null && i >= totalSignals / 10 && i % (totalSignals / 100) == 0)
                    {
                        float percentage = (float)i / totalSignals * 100;
                        progress.Report(percentage);
                    }
                }

                return newSignals.Select(lst => lst.ToArray()).ToArray();
            });

            return showtape;
        }

        return null;
    }

    return null;
    }
}
