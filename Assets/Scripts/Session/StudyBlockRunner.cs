/// <summary>
/// The study/break clock. Plain C#, no Unity types: the caller supplies the
/// delta and the three facts that decide whether the clock may advance, so the
/// clock never has to look at the scene (Session must not depend on
/// Presentation) and can be exercised without entering Play mode.
/// </summary>
public class StudyBlockRunner
{
    private const float SecondsPerMinute = 60f;

    private float _remainingSeconds;
    private bool _breakElapsedReported;

    public StudyPhase Phase { get; private set; }
    public int StudyMinutes { get; private set; }
    public int BreakMinutes { get; private set; }
    public float RemainingSeconds => _remainingSeconds;

    public void StartStudy(int studyMinutes, int breakMinutes)
    {
        StudyMinutes = studyMinutes;
        BreakMinutes = breakMinutes;
        Phase = StudyPhase.Study;
        _remainingSeconds = studyMinutes * SecondsPerMinute;
        _breakElapsedReported = false;
    }

    public void Stop()
    {
        Phase = StudyPhase.Idle;
        _remainingSeconds = 0f;
        _breakElapsedReported = false;
    }

    /// <param name="studyModeActive">She is actually at the book.</param>
    /// <param name="activityRunning">Dance or the minigame owns her.</param>
    /// <param name="voiceBusy">Somebody is talking — the turn is not idle.</param>
    public StudyClockEvent Tick(float deltaSeconds, bool studyModeActive, bool activityRunning, bool voiceBusy)
    {
        switch (Phase)
        {
            case StudyPhase.Study:
                // Studying is the only thing that spends study time. A dance, a
                // game and a conversation are all things she is doing INSTEAD.
                if (!studyModeActive || activityRunning || voiceBusy) return StudyClockEvent.None;

                _remainingSeconds -= deltaSeconds;
                if (_remainingSeconds > 0f) return StudyClockEvent.None;

                Phase = StudyPhase.Break;
                _remainingSeconds = BreakMinutes * SecondsPerMinute;
                // A break with no length never rings by itself; the way back is
                // then whatever ends the break — an activity finishing, or the
                // user saying so.
                _breakElapsedReported = BreakMinutes <= 0;
                return StudyClockEvent.StudyBlockEnded;

            case StudyPhase.Break:
                // A dance lasts as long as a dance and a game lasts as long as
                // the player wants; while one runs the break clock means nothing.
                if (activityRunning || _breakElapsedReported) return StudyClockEvent.None;

                _remainingSeconds -= deltaSeconds;
                if (_remainingSeconds > 0f) return StudyClockEvent.None;

                // Reported once, and the phase does NOT advance: the break ends
                // when she asks and the user answers, never on this clock alone.
                _breakElapsedReported = true;
                return StudyClockEvent.BreakElapsed;
        }

        return StudyClockEvent.None;
    }
}

public enum StudyPhase
{
    Idle,
    Study,
    Break
}

public enum StudyClockEvent
{
    None,
    StudyBlockEnded,
    BreakElapsed
}
