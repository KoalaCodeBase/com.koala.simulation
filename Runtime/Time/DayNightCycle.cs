namespace Koala.Simulation.Chronos
{
    internal sealed class DayNightCycle
    {
        private readonly SimulationTime _time;
        private readonly int _dayStartHour;
        private readonly int _nightStartHour;

        public DayNightCycle(SimulationTime time, int dayStartHour, int nightStartHour)
        {
            _time = time;
            _dayStartHour = dayStartHour;
            _nightStartHour = nightStartHour;
        }

        internal int DayStartHour => _dayStartHour;
        internal int NightStartHour => _nightStartHour;

        public bool IsDay => _time.CurrentHour >= _dayStartHour && _time.CurrentHour < _nightStartHour;
        public bool IsNight => !IsDay;

        public float DayProgress
        {
            get
            {
                if (!IsDay) return 0f;
                float total = _nightStartHour - _dayStartHour;
                float passed = _time.CurrentHour + _time.CurrentMinute / 60f + _time.CurrentSecond / 3600f - _dayStartHour;
                return passed / total;
            }
        }

        public float NightProgress
        {
            get
            {
                if (!IsNight) return 0f;

                int total;
                float passed;

                if (_time.CurrentHour >= _nightStartHour)
                {
                    total = (24 - _nightStartHour) + _dayStartHour;
                    passed = _time.CurrentHour + _time.CurrentMinute / 60f + _time.CurrentSecond / 3600f - _nightStartHour;
                }
                else
                {
                    total = (24 - _nightStartHour) + _dayStartHour;
                    passed = (24 - _nightStartHour) + (_time.CurrentHour + _time.CurrentMinute / 60f + _time.CurrentSecond / 3600f);
                }

                return passed / total;
            }
        }

        public float NormalizedTimeOfDay
        {
            get
            {
                return (_time.CurrentHour + _time.CurrentMinute / 60f + _time.CurrentSecond / 3600f) / 24f;
            }
        }

        public float DayNightSwitch
        {
            get
            {
                if (IsDay)
                    return DayProgress;
                else
                    return 1f - NightProgress;
            }
        }
    }
}