using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class rshwFile
{
    public byte[] audioData { get; set; }
    public int[] signalData { get; set; }

    public void Save(string filePath)
    {
        BinaryFormatter formatter = new();
        using (FileStream stream = File.Open(filePath, FileMode.Create))
        {
            formatter.Serialize(stream, this);
        }
    }

    public static rshwFile ReadFromFile(string filepath)
    {
        BinaryFormatter formatter = new();
        using (FileStream stream = File.OpenRead(filepath))
        {
            return (rshwFile)formatter.Deserialize(stream);
        }
    }
}