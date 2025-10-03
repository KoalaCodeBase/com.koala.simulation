namespace Koala.Simulation.Chronos
{
    /// <summary>
    /// Immutable snapshot of simulation time.
    /// 
    /// Represents a specific point in game time 
    /// (year, month, day, hour, minute, second).
    /// </summary>
    /// <example>
    /// <code>
    /// // Get current time from SimulationTime
    /// GameTime now = SimulationTime.Instance.Current;
    /// Debug.Log(now.ToString()); 
    /// // Output: "0001-01-01 06:00:00"
    /// 
    /// // Create a custom GameTime
    /// var custom = new GameTime(5, 3, 15, 12, 30, 0);
    /// Debug.Log(custom); 
    /// // Output: "0005-03-15 12:30:00"
    /// 
    /// // Compare GameTimes
    /// if (now.Day == custom.Day)
    ///     Debug.Log("Same day!");
    /// </code>
    /// </example>
    public struct GameTime
    {
        /// <summary> Year (starts at 1). </summary>
        public int Year;

        /// <summary> Month (1-12). </summary>
        public int Month;

        /// <summary> Day (1-30, fixed length). </summary>
        public int Day;

        /// <summary> Hour (0-23). </summary>
        public int Hour;

        /// <summary> Minute (0-59). </summary>
        public int Minute;

        /// <summary> Second (0-59). </summary>
        public int Second;

        /// <summary>
        /// Construct a new GameTime snapshot.
        /// </summary>
        public GameTime(int year, int month, int day, int hour, int minute, int second)
        {
            Year = year;
            Month = month;
            Day = day;
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        /// <summary>
        /// Returns the formatted string representation: "YYYY-MM-DD HH:MM:SS".
        /// </summary>
        public override string ToString()
        {
            return $"{Year:D4}-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}:{Second:D2}";
        }
    }
}