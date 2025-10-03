using System;
using System.Collections.Generic;
using Koala.Simulation.Common;

namespace Koala.Simulation.Chronos
{
    /// <summary>
    /// Centralized time manager for simulation.
    /// 
    /// - Tracks game date and time (year, month, day, hour, minute, second).
    /// - Provides day/night cycle information.
    /// - Supports scheduled events (once, relative, recurring).
    /// - Exposes hooks (OnSecondPassed, OnMinutePassed, OnHourPassed, OnDayPassed).
    /// - Supports save/load integration (with Easy Save).
    /// 
    /// Singleton: Access via <c>SimulationTime.Instance</c>.
    /// Requires <c>SimulationManager</c> to be present in the scene.
    /// Only one instance of <c>SimulationManager</c> should exist. 
    /// </summary>
    /// <example>
    /// <code>
    /// // Access current time
    /// var now = SimulationTime.Instance.Current;
    /// Debug.Log(now); // "0001-01-01 06:00:00"
    /// 
    /// // Check day/night
    /// if (SimulationTime.Instance.IsDay)
    ///     Debug.Log("It's daytime!");
    /// 
    /// // Subscribe to hooks
    /// SimulationTime.Instance.OnHourPassed += () =>
    ///     Debug.Log("Another hour has passed.");
    /// 
    /// // Schedule a recurring event at 23:00
    /// var handle = SimulationTime.Instance.ScheduleRecurring(
    ///     () => SimulationTime.Instance.Pause(), 23);
    /// 
    /// // Cancel scheduled event later
    /// SimulationTime.Instance.CancelScheduledEvent(handle);
    /// 
    /// // Skip directly to next day
    /// SimulationTime.Instance.StartNextDay();
    /// 
    /// // Save and load
    /// SimulationTime.Instance.Save();
    /// SimulationTime.Instance.Load(dateOnly: true);
    /// </code>
    /// </example>
    public sealed class SimulationTime : IDisposable
    {
        #region Internal Data Structures

        internal class ScheduledEvent
        {
            public Action Action;
            public GameTime Target;
            public bool IsRecurring;
        }

        [Serializable]
        internal struct SimulationTimeSaveData
        {
            public int Year;
            public int Month;
            public int Day;
            public int Hour;
            public int Minute;
            public int Second;

            public bool Paused;
            public float Elapsed;
        }

        #endregion

        #region Singleton

        private const string SaveFile = "world.es3";
        private const string TimeKey = "SimulationTime";
        public static SimulationTime Instance { get; private set; }

        /// <summary>
        /// For internal use. Calling this incorrectly may lead to unexpected behavior.
        /// </summary>
        [DocfxIgnore]
        public void Dispose()
        {
            Instance = null;
        }

        #endregion

        #region Fields

        private readonly int _cycleLengthInMinutes;
        private readonly float _secondsPerGameDay;
        private readonly DayNightCycle _dayNight;

        private float _elapsed;
        private bool _paused;

        private int _year = 1;
        private int _month = 1;
        private int _day = 1;
        private int _hour = 0;
        private int _minute = 0;
        private int _second = 0;

        private readonly List<ScheduledEvent> _events = new();

        #endregion

        #region Properties

        public GameTime Current => new GameTime(_year, _month, _day, _hour, _minute, _second);

        public int CurrentYear => _year;
        public int CurrentMonth => _month;
        public int CurrentDay => _day;
        public int CurrentHour => _hour;
        public int CurrentMinute => _minute;
        public int CurrentSecond => _second;

        /// <summary>
        /// Current hour in 12-hour format (1–12).
        /// </summary>
        public int CurrentHour12h
        {
            get
            {
                int h = _hour % 12;
                return h == 0 ? 12 : h;
            }
        }

        /// <summary>
        /// AM or PM suffix depending on the current hour.
        /// </summary>
        public string CurrentAmPm => _hour < 12 ? "AM" : "PM";

        /// <summary>
        /// Current time as "HH:mm" in 24-hour format.  
        /// Example: "23:45"
        /// </summary>
        public string CurrentTime24h => $"{_hour:D2}:{_minute:D2}";

        /// <summary>
        /// Current time as "h:mm AM/PM" in 12-hour format.  
        /// Example: "11:05 AM"
        /// </summary>
        public string CurrentTime12h =>
            $"{CurrentHour12h:D2}:{_minute:D2} {CurrentAmPm}";

        /// <summary>
        /// Current time as "h:mm:ss AM/PM" in 12-hour format.  
        /// Example: "11:05:42 PM"
        /// </summary>
        public string CurrentTimeMinute12h =>
            $"{CurrentHour12h:D2}:{_minute:D2}:{_second:D2} {CurrentAmPm}";

        // Day/Night API
        public bool IsDay => _dayNight.IsDay;
        public bool IsNight => _dayNight.IsNight;
        public float DayProgress => _dayNight.DayProgress;
        public float NightProgress => _dayNight.NightProgress;

        /// <summary> Normalized 0-1 time of day (0=midnight, 0.5=noon). </summary>
        public float NormalizedTimeOfDay =>
            (CurrentHour + CurrentMinute / 60f + CurrentSecond / 3600f) / 24f;

        /// <summary> Blend factor (0=night, 1=day). </summary>
        public float DayNightSwitch =>
            IsDay ? DayProgress : 1f - NightProgress;

        #endregion

        #region Events (Hooks)

        /// <summary> Called every in-game second. </summary>
        public event Action OnSecondPassed;

        /// <summary> Called every in-game minute. </summary>
        public event Action OnMinutePassed;

        /// <summary> Called every in-game hour. </summary>
        public event Action OnHourPassed;

        /// <summary> Called every in-game day. </summary>
        public event Action OnDayPassed;

        #endregion

        #region Initialization

        public SimulationTime(int cycleLengthInMinutes, int dayStartHour = 6, int nightStartHour = 18)
        {
            if (Instance != null)
                throw new InvalidOperationException("SimulationTime already initialized.");

            Instance = this;

            _cycleLengthInMinutes = cycleLengthInMinutes;
            _secondsPerGameDay = cycleLengthInMinutes * 60f;

            _dayNight = new DayNightCycle(this, dayStartHour, nightStartHour);

            _hour = dayStartHour;
            _minute = 0;
            _second = 0;
        }

        #endregion

        #region Controls

        public void Pause() => _paused = true;
        public void Resume() => _paused = false;

        /// <summary>
        /// Instantly skip to the next day at DayStartHour.
        /// </summary>
        public void StartNextDay()
        {
            _day++;
            if (_day > 30)
            {
                _day = 1;
                _month++;
                if (_month > 12)
                {
                    _month = 1;
                    _year++;
                }
            }

            _hour = _dayNight.DayStartHour;
            _minute = 0;
            _second = 0;

            OnDayPassed?.Invoke();
        }

        #endregion

        #region Update Loop

        public void Update(float deltaTime)
        {
            if (_paused) return;

            _elapsed += deltaTime;

            float gameSecondsPerRealSecond = 86400f / _secondsPerGameDay;
            float gameSecondsToAdvance = _elapsed * gameSecondsPerRealSecond;

            while (gameSecondsToAdvance >= 1f)
            {
                gameSecondsToAdvance -= 1f;
                AdvanceOneSecond();
            }

            _elapsed = gameSecondsToAdvance / gameSecondsPerRealSecond;
        }

        private void AdvanceOneSecond()
        {
            _second++;
            OnSecondPassed?.Invoke();

            if (_second >= 60)
            {
                _second = 0;
                _minute++;
                OnMinutePassed?.Invoke();

                if (_minute >= 60)
                {
                    _minute = 0;
                    _hour++;
                    OnHourPassed?.Invoke();

                    if (_hour >= 24)
                    {
                        _hour = 0;
                        _day++;
                        OnDayPassed?.Invoke();

                        if (_day > 30)
                        {
                            _day = 1;
                            _month++;
                            if (_month > 12)
                            {
                                _month = 1;
                                _year++;
                            }
                        }
                    }
                }
            }

            CheckEvents();
        }

        #endregion

        #region Scheduling API

        /// <summary> Run once at exact target time. </summary>
        public EventHandle ScheduleOnce(Action action, GameTime target)
        {
            var ev = new ScheduledEvent { Action = action, Target = target, IsRecurring = false };
            _events.Add(ev);
            return new EventHandle { Ref = ev };
        }

        /// <summary> Run once after offset from now. </summary>
        public EventHandle ScheduleRelative(Action action, TimeSpan offset)
        {
            var now = Current;
            var dt = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second).Add(offset);

            var target = new GameTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            var ev = new ScheduledEvent { Action = action, Target = target, IsRecurring = false };
            _events.Add(ev);
            return new EventHandle { Ref = ev };
        }

        /// <summary> Run every day at the given hour/minute/second. </summary>
        public EventHandle ScheduleRecurring(Action action, int hour, int minute = 0, int second = 0)
        {
            var target = new GameTime(_year, _month, _day, hour, minute, second);
            var ev = new ScheduledEvent { Action = action, Target = target, IsRecurring = true };
            _events.Add(ev);
            return new EventHandle { Ref = ev };
        }

        /// <summary> Cancel a specific scheduled event. </summary>
        public void CancelScheduledEvent(EventHandle handle)
        {
            if (handle?.Ref != null)
                _events.Remove(handle.Ref);
        }

        /// <summary> Cancel all scheduled events. </summary>
        public void CancelAllScheduledEvents()
        {
            _events.Clear();
        }

        private void CheckEvents()
        {
            for (int i = _events.Count - 1; i >= 0; i--)
            {
                var ev = _events[i];
                if (Matches(ev.Target))
                {
                    ev.Action?.Invoke();

                    if (ev.IsRecurring)
                    {
                        ev.Target = new GameTime(
                            _year, _month, _day + 1,
                            ev.Target.Hour, ev.Target.Minute, ev.Target.Second);
                    }
                    else
                    {
                        _events.RemoveAt(i);
                    }
                }
            }
        }

        private bool Matches(GameTime t)
        {
            return _year == t.Year &&
                   _month == t.Month &&
                   _day == t.Day &&
                   _hour == t.Hour &&
                   _minute == t.Minute &&
                   _second == t.Second;
        }

        #endregion

        #region Save / Load

        internal void Save()
        {
            var data = new SimulationTimeSaveData
            {
                Year = _year,
                Month = _month,
                Day = _day,
                Hour = _hour,
                Minute = _minute,
                Second = _second,
                Paused = _paused,
                Elapsed = _elapsed
            };

            ES3.Save(TimeKey, data, SaveFile);
        }

        internal void Load(bool dateOnly = false)
        {
            if (!ES3.KeyExists(TimeKey, SaveFile))
                return;

            var data = ES3.Load<SimulationTimeSaveData>(TimeKey, SaveFile);

            _year = data.Year;
            _month = data.Month;
            _day = data.Day;

            if (dateOnly)
            {
                _hour = _dayNight.DayStartHour;
                _minute = 0;
                _second = 0;
                _paused = false;
                _elapsed = 0f;
            }
            else
            {
                _hour = data.Hour;
                _minute = data.Minute;
                _second = data.Second;
                _paused = data.Paused;
                _elapsed = data.Elapsed;
            }
        }

        #endregion
    }
}