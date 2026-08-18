using System;
using System.Collections.Generic;

/// <summary>
/// What is remembered between sessions — and deliberately little else. There
/// are no cumulative study statistics here: how many minutes or sessions have
/// been spent is not something this app keeps. Memory is the last session and
/// the handful of things worth mentioning from the last few days.
/// </summary>
[Serializable]
public class StudentProfile
{
    public const int CurrentSchemaVersion = 2;

    public const string InterestUnknown = "";
    public const string InterestLikes = "likes";
    public const string InterestDislikes = "dislikes";

    public int schemaVersion = CurrentSchemaVersion;

    public string studentName;
    public string gradeOrClass;

    public string likesDancing = InterestUnknown;
    public string likesGames = InterestUnknown;

    public string lastSessionUtc;
    public string lastSessionSummary;

    public List<ProfileEvent> recentEvents = new List<ProfileEvent>();
}

[Serializable]
public class ProfileEvent
{
    public string dateUtc;
    public string text;
}
