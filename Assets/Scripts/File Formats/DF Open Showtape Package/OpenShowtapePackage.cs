using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// OpenShowtapePackage JSON Class
/// </summary>
public class OpenShowtapePackage
{
    [JsonProperty("metadata")]
    public OspMetadata metadata { get; set; }
    
    [JsonProperty("audio_data")]
    public OspAudioData audioData { get; set; }
    
    [JsonProperty("video_data")]
    public OspVideoData videoData { get; set; }
    
    [JsonProperty("frame_rate")]
    public int frameRate { get; set; }
    
    [JsonProperty("frames")]
    public int[][] frames { get; set; }
}

public class OspMetadata
{
    [JsonProperty("title")]
    public string title { get; set; } // Title of the showtape
    
    [JsonProperty("author")]
    public string author { get; set; } // Author or creator of the showtape
    
    [JsonProperty("type")]
    public string type { get; set; } // Type of showtape (see documents for examples)
    
    [JsonProperty("description")]
    public string description { get; set; } // Brief description of the showtape content
    
    [JsonProperty("date_created")]
    public string dateCreated { get; set; } // Creation date of the showtape
    
    [JsonProperty("date_updated")]
    public string dateUpdated { get; set; } // Last updated date of the showtape
    
    [JsonProperty("additional_info")]
    public string additionalInfo { get; set; } // Any extra information related to the showtape
}
public class OspAudioData
{
    [JsonProperty("data")]
    public string data { get; set; } // Encoded audio data in base64
    
    [JsonProperty("format")]
    public string format { get; set; } // Audio file format (e.g., "mp3", "wav")
    
    [JsonProperty("sample_rate")]
    public string sampleRate { get; set; } // Sample rate of the audio (e.g., "44100Hz")
    
    [JsonProperty("channels")]
    public string channels { get; set; } // Number of audio channels (e.g., "stereo", "mono")
    
    [JsonProperty("bit_rate")]
    public string bitRate { get; set; } // Bitrate of the audio (e.g., "192kbps")
}

public class OspVideoData
{
    [JsonProperty("data")]
    public string data { get; set; } // Encoded video data in base64
    
    [JsonProperty("format")]
    public string format { get; set; } // Video file format (e.g., "mp4", "avi")
}