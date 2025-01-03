using Discord;

namespace OpenRoadHelper.Quests
{
    public class Availability
    {
        private readonly ulong _gm;
        private readonly List<Timeframe> _times;
        private DateTime _lastUpdate;
        private DateTime _lastClear;

        public Availability(IUser gm, List<Timeframe> availability)
        {
            _gm = gm.Id;
            _times = availability;
            _lastUpdate = DateTime.Now;
            _lastClear = DateTime.Now;
        }

        public Availability(IUser gm, Timeframe availability)
        {
            _gm = gm.Id;
            _times = new() { availability };
            _lastUpdate = DateTime.Now;
            _lastClear = DateTime.Now;
        }

        public Availability(IUser gm)
        {
            _gm = gm.Id;
            _times = new();
            _lastUpdate = DateTime.Now;
            _lastClear = DateTime.Now;
        }

        public void AddTimeframe(Timeframe time)
        {
            CheckShouldClearPast();
            _times.Add(time);
            _lastUpdate = DateTime.Now;
        }

        public void RemoveTimeframe(Timeframe time)
        {
            CheckShouldClearPast();
            foreach (Timeframe timeframe in _times)
            {
                if (timeframe.IsEqual(time))
                {
                    _times.Remove(timeframe);
                }
            }
            _lastUpdate = DateTime.Now;
        }

        public void RemoveTimeframe(int index)
        {
            CheckShouldClearPast();
            if (index < _times.Count - 1)
            {
                _times.Remove(_times[index]);
                _lastUpdate = DateTime.Now;
            }
        }

        public void RemoveTimeframe(Guid id)
        {
            CheckShouldClearPast();
            foreach (Timeframe timeframe in _times)
            {
                if (timeframe.Id == id)
                {
                    _times.Remove(timeframe);
                }
            }
            _lastUpdate = DateTime.Now;
        }

        public Timeframe? GetTimeframe(Guid id)
        {
            foreach (Timeframe timeframe in _times)
            {
                if (timeframe.Id == id)
                {
                    return timeframe;
                }
            }
            return null;
        }

        public void ReplaceTimeframe(Timeframe newTimeframe)
        {
            Timeframe? timeframe = GetTimeframe(newTimeframe.Id);
            if (timeframe != null)
            {
                AddTimeframe(newTimeframe);
                RemoveTimeframe((Timeframe)timeframe);
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
            foreach (Timeframe timeframe in _times)
            {
                if (timeframe.EarliestStart.CompareTo(DateTime.Now.AddHours(-8)) <= 0)
                {
                    _times.Remove(timeframe);
                }
            }
            _lastClear = DateTime.Now;
        }

        public bool IsAvailable(DateTime time)
        {
            foreach (Timeframe timeframe in _times)
            {
                if (timeframe.IsAvailableTime(time))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsAvailable(uint time)
        {
            foreach (Timeframe timeframe in _times)
            {
                if (timeframe.IsAvailableTime(Generic.DateTimeFromUnixSeconds(time)))
                {
                    return true;
                }
            }
            return false;
        }

        public ulong GM
        {
            get => _gm;
        }

        public List<Timeframe> Times
        {
            get => _times;
        }

        public DateTime Updated
        {
            get => _lastUpdate;
        }
    }

    public struct Timeframe
    {
        private readonly Guid _id;
        public readonly Guid Id
        {
            get => _id;
        }
        public DateTime EarliestStart;
        public DateTime LatestStart;
        public DateTime Cutoff;

        public Timeframe()
        {
            _id = Guid.NewGuid();
        }

        public readonly bool IsEqual(Timeframe timeframe)
        {
            return timeframe.Id == _id
                || (
                    timeframe.EarliestStart == EarliestStart
                    && timeframe.LatestStart == LatestStart
                    && timeframe.Cutoff == Cutoff
                );
        }

        public readonly bool IsAvailableTime(DateTime time)
        {
            return EarliestStart.CompareTo(time) <= 0 && LatestStart.CompareTo(time) >= 0;
        }
    }
}
