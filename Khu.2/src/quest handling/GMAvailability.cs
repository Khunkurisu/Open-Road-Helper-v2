using Discord;

namespace Bot.Quests
{
    public class GMAvailability
    {
        private readonly IUser _gm;
        private readonly List<Timeframe> _availability;
        private DateTime _lastUpdate;
        private DateTime _lastClear;

        public GMAvailability(IUser gm, List<Timeframe> availability)
        {
            _gm = gm;
            _availability = availability;
            _lastUpdate = DateTime.Now;
            _lastClear = DateTime.Now;
        }

        public GMAvailability(IUser gm, Timeframe availability)
        {
            _gm = gm;
            _availability = new() { availability };
            _lastUpdate = DateTime.Now;
            _lastClear = DateTime.Now;
        }

        public GMAvailability(IUser gm)
        {
            _gm = gm;
            _availability = new();
            _lastUpdate = DateTime.Now;
            _lastClear = DateTime.Now;
        }

        public void AddTimeframe(Timeframe time)
        {
            CheckShouldClearPast();
            _availability.Add(time);
            _lastUpdate = DateTime.Now;
        }

        public void RemoveTimeframe(Timeframe time)
        {
            CheckShouldClearPast();
            foreach (Timeframe timeframe in _availability)
            {
                if (timeframe.IsEqual(time))
                {
                    _availability.Remove(timeframe);
                }
            }
            _lastUpdate = DateTime.Now;
        }

        public void RemoveTimeframe(int index)
        {
            CheckShouldClearPast();
            if (index < _availability.Count)
            {
                _availability.Remove(_availability[index]);
                _lastUpdate = DateTime.Now;
            }
        }

        public void CheckShouldClearPast()
        {
            DateTime now = DateTime.Now;
            if (_lastClear.CompareTo(now.AddHours(-1)) <= 0)
            {
                ClearPastAvailability();
            }
        }

        private void ClearPastAvailability()
        {
            foreach (Timeframe timeframe in _availability)
            {
                if (timeframe.EarliestStart.CompareTo(DateTime.Now.AddHours(-8)) <= 0)
                {
                    _availability.Remove(timeframe);
                }
            }
            _lastClear = DateTime.Now;
        }

        public IUser GM
        {
            get => _gm;
        }

        public List<Timeframe> Availability
        {
            get => _availability;
        }

        public DateTime Updated
        {
            get => _lastUpdate;
        }
    }

    public struct Timeframe
    {
        public DateTime EarliestStart;
        public DateTime LatestStart;
        public DateTime Cutoff;

        public readonly bool IsEqual(Timeframe timeframe)
        {
            return timeframe.EarliestStart == EarliestStart
                && timeframe.LatestStart == LatestStart
                && timeframe.Cutoff == Cutoff;
        }
    }
}
