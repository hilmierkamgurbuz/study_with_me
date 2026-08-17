using System;
using Newtonsoft.Json.Linq;

public static class ProfileMerger
{
    public static void ApplyToolCallArgs(StudentProfile profile, JObject args)
    {
        if (args["studentName"] != null) profile.studentName = args["studentName"].ToString();
        if (args["gradeOrClass"] != null) profile.gradeOrClass = args["gradeOrClass"].ToString();
        if (args["examTarget"] != null) profile.examTarget = args["examTarget"].ToString();
        if (args["usualStudyTime"] != null) profile.usualStudyTime = args["usualStudyTime"].ToString();
        if (args["typicalSessionMinutes"] != null) profile.typicalSessionMinutes = args["typicalSessionMinutes"].Value<int>();
        if (args["preferredBreakFrequencyMinutes"] != null) profile.preferredBreakFrequencyMinutes = args["preferredBreakFrequencyMinutes"].Value<int>();
    }

    public static void ApplySessionStats(StudentProfile profile, double minutesElapsed)
    {
        profile.totalStudyMinutes += (float)minutesElapsed;
        profile.totalStudySessions += 1;
        profile.lastSessionUtc = DateTime.UtcNow.ToString("o");
    }
}
