using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

/// <summary>
/// The only place a StudentProfile field is ever assigned. Everything that
/// wants to change what is remembered comes through here — tool calls, activity
/// preferences, remembered events, the end-of-session summary and the pruning
/// that keeps memory to the last few days.
/// </summary>
public static class ProfileMerger
{
    private const string IsoFormat = "o";

    /// <summary>
    /// Brings a profile read off disk up to the current schema. Version 1 held
    /// cumulative study statistics and self-reported study habits; those fields
    /// simply do not exist any more, so deserialization drops them and this
    /// stamps the version they were dropped at.
    /// </summary>
    public static void Migrate(StudentProfile profile)
    {
        if (profile == null) return;

        if (profile.recentEvents == null) profile.recentEvents = new List<ProfileEvent>();
        if (profile.likesDancing == null) profile.likesDancing = StudentProfile.InterestUnknown;
        if (profile.likesGames == null) profile.likesGames = StudentProfile.InterestUnknown;

        profile.schemaVersion = StudentProfile.CurrentSchemaVersion;
    }

    public static void ApplyToolCallArgs(StudentProfile profile, JObject args)
    {
        if (profile == null || args == null) return;

        string name = ReadString(args, "studentName");
        if (name != null) profile.studentName = name;

        string grade = ReadString(args, "gradeOrClass");
        if (grade != null) profile.gradeOrClass = grade;
    }

    public static void SetDanceInterest(StudentProfile profile, string interest)
    {
        if (profile == null) return;
        profile.likesDancing = interest ?? StudentProfile.InterestUnknown;
    }

    public static void SetGameInterest(StudentProfile profile, string interest)
    {
        if (profile == null) return;
        profile.likesGames = interest ?? StudentProfile.InterestUnknown;
    }

    /// <summary>
    /// Records something worth mentioning again — an exam, a birthday, a trip.
    /// Repeats are dropped: the same fact told twice in one conversation should
    /// not take two slots in a memory this small.
    /// </summary>
    public static void AddEvent(StudentProfile profile, string text, DateTime nowUtc)
    {
        if (profile == null || string.IsNullOrEmpty(text)) return;
        if (profile.recentEvents == null) profile.recentEvents = new List<ProfileEvent>();

        for (int i = 0; i < profile.recentEvents.Count; i++)
        {
            if (string.Equals(profile.recentEvents[i].text, text, StringComparison.OrdinalIgnoreCase)) return;
        }

        profile.recentEvents.Add(new ProfileEvent
        {
            dateUtc = nowUtc.ToString(IsoFormat, CultureInfo.InvariantCulture),
            text = text
        });
    }

    public static void SetSessionSummary(StudentProfile profile, string summary, DateTime nowUtc)
    {
        if (profile == null) return;
        if (!string.IsNullOrEmpty(summary)) profile.lastSessionSummary = summary;
        StampSession(profile, nowUtc);
    }

    public static void StampSession(StudentProfile profile, DateTime nowUtc)
    {
        if (profile == null) return;
        profile.lastSessionUtc = nowUtc.ToString(IsoFormat, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Drops events older than the memory window. An entry whose date cannot be
    /// parsed is kept rather than silently thrown away — a bad timestamp is a
    /// reason to look, not a reason to lose what the user said.
    /// </summary>
    public static void PruneEvents(StudentProfile profile, int keepDays, DateTime nowUtc)
    {
        if (profile?.recentEvents == null || keepDays <= 0) return;

        DateTime oldest = nowUtc.AddDays(-keepDays);
        for (int i = profile.recentEvents.Count - 1; i >= 0; i--)
        {
            var entry = profile.recentEvents[i];
            if (entry == null)
            {
                profile.recentEvents.RemoveAt(i);
                continue;
            }

            DateTime when;
            bool parsed = DateTime.TryParse(entry.dateUtc, CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out when);
            if (parsed && when.ToUniversalTime() < oldest) profile.recentEvents.RemoveAt(i);
        }
    }

    private static string ReadString(JObject args, string key)
    {
        var token = args[key];
        if (token == null || token.Type == JTokenType.Null) return null;

        string value = token.ToString();
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
