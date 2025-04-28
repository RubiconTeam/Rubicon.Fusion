using System.Collections.Generic;
using Rubicon.Core;
using Rubicon.Core.Meta;
using Rubicon.Core.Rulesets;

namespace Rubicon.Game;

/// <summary>
/// A class that helps to load, track and execute any song event.
/// </summary>
[GlobalClass] public partial class SongEventController : Node
{
    /// <summary>
    /// The index from the current event list.
    /// </summary>
    [Export] public int Index = 0;
    
    /// <summary>
    /// A signal that gets called everytime an event is executed.
    /// </summary>
    /// <param name="eventName">The name of the event being executed.</param>
    /// <param name="time">The time when the event has been executed.</param>
    /// <param name="args">The arguments of the event.</param>
    [Signal] public delegate void EventCalledEventHandler(StringName eventName, float time, Godot.Collections.Dictionary<StringName, Variant> args);

    [Export] private EventData[] _events = [];
    
    /// <summary>
    /// Sets up every event in the <see cref="EventMeta"/> file of the song.
    /// </summary>
    /// <param name="eventMeta">The data of every event in the song.</param>
    /// <param name="playField">The current <see cref="PlayField"/>.</param>
    public void Setup(EventMeta eventMeta, PlayField playField)
    {
        _events = eventMeta.Events;
        List<StringName> eventsInitialized = [];
        for (int i = 0; i < _events.Length; i++)
        {
            StringName eventName = _events[i].Name;
            if (eventsInitialized.Contains(eventName))
                return;
            
            eventsInitialized.Add(eventName);
            PackedScene eventScene = ResourceLoader.Load<PackedScene>(RubiconEngine.Events.Paths[eventName].Path);
            Node @event = eventScene.Instantiate();
            
            AddChild(@event);
            playField.InitializeGodotScript(@event);
        }
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Index >= _events.Length)
            return;
        
        EventData curEvent = _events[Index];
        if (Conductor.Time * 1000f >= curEvent.MsTime)
        {
            EmitSignalEventCalled(curEvent.Name, curEvent.Time, curEvent.Arguments);
            Index++;
        }
    }

    /// <summary>
    /// Resets the event list as well as its index.
    /// </summary>
    public void Reset()
    {
        Index = 0;
        _events = [];
    }
}