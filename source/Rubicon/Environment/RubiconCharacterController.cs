using System.Collections.Generic;
using Rubicon.Core;
using Rubicon.Data;
using Rubicon.Enums;

namespace Rubicon.Environment;

[GlobalClass] public abstract partial class RubiconCharacterController : RubiconDancerController
{
    /// <summary>
    /// How long the singing animation should last in steps before idling back.
    /// </summary>
    [Export] public float SingDuration = 4;

    /// <summary>
    /// A timer that determines if the animation should be finished or not.
    /// </summary>
    [Export] public float SingTimer;
    
    /// <summary>
    /// The animation that is currently being played.
    /// </summary>
    [Export] public SpecialAnimation CurrentSpecialParameters = null;

    /// <summary>
    /// Whether this character is currently singing or not.
    /// </summary>
    [Export] public bool Singing = false;
    
    /// <summary>
    /// Marks whether this character is holding a note.
    /// </summary>
    [Export] public bool Holding = false;

    /// <summary>
    /// Marks whether this character has missed.
    /// </summary>
    [Export] public bool Missed = false;

    /// <summary>
    /// Whether this character should freeze after holding (and not go to idle)
    /// </summary>
    [Export] public bool FreezeSinging = false;
    
    private int _lastStep = -int.MaxValue;
    private Dictionary<string, bool> _directionsHolding = new();

    public override void _Ready()
    {
	    AnimationPlayer.AnimationFinished += AnimationFinished;
	    
	    base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
	    
        // Hacky hold method, should work for now
        int curStep = Mathf.FloorToInt(Conductor.CurrentStep);
        if (Holding)
            HandleHoldAnimations();

        SingTimer += (float)delta;
        _lastStep = curStep;

        if (CurrentSpecialParameters == null)
            return;

        if (AnimationPlayer.CurrentAnimationPosition > AnimationPlayer.CurrentAnimationLength)
            CurrentSpecialParameters = null;
    }
    
    /// <summary>
    /// Plays a sing animation for this character.
    /// </summary>
    /// <param name="direction">Marks the direction to sing at</param>
    /// <param name="holding">Marks this as a holding animation</param>
    /// <param name="miss">Marks this as a miss animation</param>
    /// <param name="customPrefix">An optional prefix to put in before the sing animation. Will be <see cref="RubiconDancerController.GlobalPrefix"/> by default.</param>
    /// <param name="customSuffix">An optional suffix to put in after the sing animation. Will be <see cref="RubiconDancerController.GlobalSuffix"/> by default.</param>
    public virtual void Sing(string direction, bool holding = false, bool miss = false, string customPrefix = null, string customSuffix = null)
    {
	    if (CurrentSpecialParameters != null && CurrentSpecialParameters.OverrideSing)
		    return;

	    direction = direction.ToUpper();
	    bool wasHolding = _directionsHolding.ContainsKey(direction) && _directionsHolding[direction];
	    _directionsHolding[direction] = holding;
	    bool shouldBeHolding = _directionsHolding.ContainsValue(true);
	    
	    SingTimer = 0f;
	    Singing = true;
	    Holding = shouldBeHolding;
	    Missed = miss && !shouldBeHolding;
	    
	    bool isHoldEnding = wasHolding && !holding;
	    if (isHoldEnding && !Missed)
		    return;

	    string animName = $"sing{direction.ToUpper()}";
	    
	    string prefix = customPrefix ?? GlobalPrefix;
	    string suffix = customSuffix ?? GlobalSuffix;
	    
	    string finalName = prefix + animName + suffix;
	    if (Missed && AnimationPlayer.HasAnimation(prefix + animName + "miss" + suffix))
		    finalName = prefix + animName + "miss" + suffix;
		    
	    if (AnimationPlayer.HasAnimation(finalName))
		    AnimationPlayer.Play(finalName);
	    
	    if (Data.ResetAnimationProgress)
		    AnimationPlayer.Seek(0f, true);
    }

    /// <summary>
    /// Plays a special animation. Allows for overriding singing and dancing.
    /// </summary>
    /// <param name="anim">The special animation data</param>
    public void PlaySpecialAnimation(SpecialAnimation anim)
    {
	    CurrentSpecialParameters = anim;
	    
	    if (AnimationPlayer.HasAnimation(anim.Name))
		    AnimationPlayer.Play(anim.Name);
	    
	    AnimationPlayer.Seek(anim.StartTime, true);
    }

    /// <summary>
    /// Gets the character being controlled in its most generic form.
    /// </summary>
    /// <returns>The character</returns>
    public abstract Node GetGenericCharacter();

    private void HandleHoldAnimations()
    {
	    CharacterHold holdType = CharacterHold.Freeze;
	    double repeatLoopPoint = 0;
	    if (Data is RubiconCharacterData characterData)
	    {
		    holdType = characterData.HoldType;
		    repeatLoopPoint = characterData.RepeatLoopPoint;
	    }
	    
	    switch (holdType)
	    {
		    case CharacterHold.None:
			    SingTimer = 0;
			    break;
		    case CharacterHold.StepRepeat:
			    int curStep = Mathf.FloorToInt(Conductor.CurrentStep);
			    if (_lastStep != curStep)
			    {
				    AnimationPlayer.Seek(0f);
				    SingTimer = 0;
			    }
			    
			    _lastStep = curStep;
			    break;
		    case CharacterHold.Repeat:
			    double currentTime = AnimationPlayer.CurrentAnimationPosition;
			    if (currentTime < repeatLoopPoint)
				    break;
			    
			    AnimationPlayer.Seek(currentTime % repeatLoopPoint);
			    SingTimer = 0;
			    break;
		    case CharacterHold.Freeze:
			    AnimationPlayer.Seek(0f);
			    SingTimer = 0;
			    break;
	    }	
    }
    
    protected override void TryDance()
    {
	    bool isNotSinging = !Singing || (Singing && !FreezeSinging && SingTimer >= Conductor.GetCurrentTimeChange().GetStepValue() * 0.001f * SingDuration);
	    bool overrideDancing = CurrentSpecialParameters != null && CurrentSpecialParameters.OverrideDance;
	    if (overrideDancing || !isNotSinging)
		    return;
	    
	    base.TryDance();
    }

    private void AnimationFinished(StringName anim)
    {
	    CurrentSpecialParameters = null;
    }
}