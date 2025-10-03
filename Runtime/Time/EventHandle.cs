namespace Koala.Simulation.Chronos
{
    /// <summary>
    /// Represents a reference to a scheduled event.
    /// 
    /// <note type="tip">
    /// Always store the <see cref="EventHandle"/> returned from 
    /// <c>ScheduleOnce</c>, <c>ScheduleRelative</c> or <c>ScheduleRecurring</c>.  
    /// Without the handle, you cannot cancel or manage that specific event later.  
    /// If you ignore it, your only option will be <c>CancelAllScheduledEvents()</c>,
    /// which removes every event.
    /// 
    /// > <para>Recommended Usage:</para>
    /// > - Save the handle in a private field if you only need one event.  
    /// > - Save it in a dictionary or list if you need to track multiple events.  
    /// > - Cancel the event with <c>SimulationTime.Instance.CancelScheduledEvent(handle)</c>.  
    /// 
    /// > <para>Why it's important:</para>
    /// > - Prevents memory leaks by letting you unregister unneeded events.  
    /// > - Gives explicit control to pause, replace or clear individual schedules.  
    /// > - Makes your simulation systems deterministic and debuggable.  
    /// </note>
    /// 
    /// </summary>
    /// <example>
    /// <code>
    /// // Schedule a recurring event at 23:00 every game day
    /// EventHandle nightHandle = SimulationTime.Instance.ScheduleRecurring(
    ///     () => Debug.Log("It's 23:00!"), 23);
    /// 
    /// // Store it (e.g., as a field in your system)
    /// private EventHandle _nightEvent;
    /// _nightEvent = nightHandle;
    /// 
    /// // Later, cancel just that event
    /// SimulationTime.Instance.CancelScheduledEvent(_nightEvent);
    /// 
    /// // Or clear everything (not recommended unless resetting)
    /// SimulationTime.Instance.CancelAllScheduledEvents();
    /// </code>
    /// </example>
    public sealed class EventHandle
    {
        internal SimulationTime.ScheduledEvent Ref;
    }
}