using System;

[Serializable]
public class StudentProfile
{
    public int schemaVersion = 1;

    public string studentName;
    public string gradeOrClass;
    public string examTarget;
    public string usualStudyTime;
    public int typicalSessionMinutes;
    public int preferredBreakFrequencyMinutes;

    public int totalStudySessions;
    public float totalStudyMinutes;
    public string lastSessionUtc;
}
