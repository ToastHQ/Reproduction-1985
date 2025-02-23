using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class flowFormat
{
    public flowControls[] characters { get; set; }

    public void Save(string filePath)
    {
        BinaryFormatter formatter = new();
        using (FileStream stream = File.Open(filePath, FileMode.Create))
        {
            formatter.Serialize(stream, this);
        }
    }

    public static flowFormat ReadFromFile(string filepath)
    {
        BinaryFormatter formatter = new();
        using (FileStream stream = File.OpenRead(filepath))
        {
            return (flowFormat)formatter.Deserialize(stream);
        }
    }
}

[Serializable]
public class flowControls
{
    public string name;
    public int[] flowsIn { get; set; }
    public int[] flowsOut { get; set; }
    public int[] weightIn { get; set; }
    public int[] weightOut { get; set; }
}