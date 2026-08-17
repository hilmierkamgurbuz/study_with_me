using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class LocalJsonProfileRepository : IProfileRepository
{
    private static string PathOnDisk => Path.Combine(Application.persistentDataPath, "profile.json");

    public StudentProfile Load()
    {
        if (!File.Exists(PathOnDisk)) return null;
        return JsonConvert.DeserializeObject<StudentProfile>(File.ReadAllText(PathOnDisk));
    }

    public void Save(StudentProfile profile)
    {
        File.WriteAllText(PathOnDisk, JsonConvert.SerializeObject(profile, Formatting.None));
    }
}
