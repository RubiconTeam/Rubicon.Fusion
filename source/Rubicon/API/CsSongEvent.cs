using Godot.Collections;
using Rubicon.Core.Rulesets;
using Rubicon.Game;

namespace Rubicon.API;

/// <summary>
/// A template for a song event in C#. Must be inherited.
/// </summary>
[GlobalClass] public abstract partial class CsSongEvent : Node
{
    private bool _initialized = false;

    /// <summary>
    /// Used to initialize this event for the first time.
    /// </summary>
    public override void _Ready()
    {
        base._Ready();
        
        if (_initialized)
            return;
        
        RubiconGame.EventController.EventCalled += OnEventCalled;
        _initialized = true;
    }

    /// <summary>
    /// This function is called when the event controller reaches it. Should be inherited.
    /// </summary>
    /// <param name="time">The time it was called on, in measures.</param>
    /// <param name="args">The arguments that came with the event.</param>
    public abstract void CallEvent(float time, Dictionary<StringName, Variant> args);
    
    private void OnEventCalled(StringName eventName, float time, Dictionary<StringName, Variant> args)
    {
        if (eventName != Name)
            return;

        CallEvent(time, args);
    }
}