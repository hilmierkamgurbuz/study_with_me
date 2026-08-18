/// <summary>
/// Decides whether a break gets an activity offer, and which one. Plain C#, no
/// Unity types. It is the single place "don't ask every break" and "stop asking
/// after three no's" are expressed — the prompt describes the offer, this owns
/// whether one happens at all.
/// </summary>
public class BreakOfferPolicy
{
    private readonly int _offerEveryNthBreak;
    private readonly int _declineLimit;

    private ActivityInterest _danceInterest;
    private ActivityInterest _gameInterest;
    private int _danceNotNowStreak;
    private int _gameNotNowStreak;

    private int _breakCount;
    private BreakActivity _lastOffered;

    public BreakOfferPolicy(int offerEveryNthBreak, int declineLimit)
    {
        _offerEveryNthBreak = offerEveryNthBreak < 1 ? 1 : offerEveryNthBreak;
        _declineLimit = declineLimit < 1 ? 1 : declineLimit;
    }

    /// <summary>A stated like/dislike — this one is persisted in the profile.</summary>
    public void SetInterest(BreakActivity activity, ActivityInterest interest)
    {
        if (activity == BreakActivity.Dance) _danceInterest = interest;
        else if (activity == BreakActivity.Game) _gameInterest = interest;

        // Saying yes clears the "not right now" run: three earlier passes should
        // not keep an activity they have just asked for off the table.
        if (interest == ActivityInterest.Likes) ResetStreak(activity);
    }

    /// <summary>"Not right now" — a pass, not a dislike. Session-lived.</summary>
    public void RecordNotNow(BreakActivity activity)
    {
        if (activity == BreakActivity.Dance) _danceNotNowStreak++;
        else if (activity == BreakActivity.Game) _gameNotNowStreak++;
    }

    /// <summary>Call once per break, when the study block ends.</summary>
    public BreakOffer NextOffer()
    {
        _breakCount++;
        if (_breakCount % _offerEveryNthBreak != 0) return default;

        // Alternate, so two breaks in a row never propose the same thing.
        BreakActivity first = _lastOffered == BreakActivity.Dance ? BreakActivity.Game : BreakActivity.Dance;
        BreakActivity second = first == BreakActivity.Dance ? BreakActivity.Game : BreakActivity.Dance;

        if (CanOffer(first)) return Offer(first);
        if (CanOffer(second)) return Offer(second);
        return default;
    }

    private bool CanOffer(BreakActivity activity)
    {
        return InterestOf(activity) != ActivityInterest.Dislikes && StreakOf(activity) < _declineLimit;
    }

    private BreakOffer Offer(BreakActivity activity)
    {
        _lastOffered = activity;
        return new BreakOffer
        {
            Activity = activity,
            // Never propose doing something together before knowing whether they
            // even like it — ask that first, and propose on a later break.
            AskInterestFirst = InterestOf(activity) == ActivityInterest.Unknown
        };
    }

    private ActivityInterest InterestOf(BreakActivity activity)
    {
        return activity == BreakActivity.Dance ? _danceInterest : _gameInterest;
    }

    private int StreakOf(BreakActivity activity)
    {
        return activity == BreakActivity.Dance ? _danceNotNowStreak : _gameNotNowStreak;
    }

    private void ResetStreak(BreakActivity activity)
    {
        if (activity == BreakActivity.Dance) _danceNotNowStreak = 0;
        else if (activity == BreakActivity.Game) _gameNotNowStreak = 0;
    }
}

public struct BreakOffer
{
    public BreakActivity Activity;
    public bool AskInterestFirst;

    public bool HasOffer => Activity != BreakActivity.None;
}

public enum BreakActivity
{
    None,
    Dance,
    Game
}

public enum ActivityInterest
{
    Unknown,
    Likes,
    Dislikes
}
