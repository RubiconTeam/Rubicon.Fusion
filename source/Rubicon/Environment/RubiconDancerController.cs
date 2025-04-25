using System.Linq;
using Rubicon.Data;

namespace Rubicon.Environment;

/// <summary>
/// A controller for RubiconDancers created at runtime.
/// </summary>
[GlobalClass] public partial class RubiconDancerController : Node
{
    /// <summary>
    /// The data referenced when handling dances.
    /// </summary>
    [Export] public RubiconDancerData Data;
    
    /// <summary>
    /// Gets added to the start of EVERY animation that plays after this is set.
    /// </summary>
    [Export] public string GlobalPrefix;

    /// <summary>
    /// Gets added to the end of EVERY animation that plays after this is set.
    /// </summary>
    [Export] public string GlobalSuffix;
    
    /// <summary>
    /// The animation controller for this dancer.
    /// </summary>
    [ExportGroup("References"), Export] public AnimationPlayer AnimationPlayer;
    
    /// <summary>
    /// The node that helps with dancing to the beat.
    /// </summary>
    [ExportGroup("Internals"), Export] public BeatSyncer BeatSyncer;
    
    /// <summary>
    /// The index for which dance animation in the <see cref="RubiconDancerData.DanceList"/> should play.
    /// </summary>
    [Export] public int DanceIndex = 0;
    
    /// <summary>
    /// Turning this to false will prevent this character from dancing, unless manually called.
    /// </summary>
    [Export] public bool FreezeDance = false;

    public override void _Ready()
    {
        base._Ready();

        BeatSyncer = new BeatSyncer();
        BeatSyncer.Name = "BeatSyncer";
        BeatSyncer.Bumped += TryDance;
        AddChild(BeatSyncer);
        
        UpdateBeatSyncer();
        Dance();
    }
    
    /// <summary>
    /// Updates the beat syncer to sync with <see cref="Data"/>.
    /// </summary>
    public void UpdateBeatSyncer()
    {
        BeatSyncer.Type = Data.TimeValue;
        BeatSyncer.Value = Data.TimeInterval;
    }
    
    /// <summary>
    /// Plays a dance animation for this dancer.
    /// </summary>
    /// <param name="customPrefix">An optional prefix to put in before the dance animation. Will be <see cref="GlobalPrefix"/> by default.</param>
    /// <param name="customSuffix">An optional suffix to put in after the dance animation. Will be <see cref="GlobalSuffix"/> by default.</param>
    public void Dance(string customPrefix = null, string customSuffix = null)
    {
        string prefix = customPrefix ?? GlobalPrefix;
        string suffix = customSuffix ?? GlobalSuffix;

        string animName = prefix + Data.DanceList[DanceIndex] + suffix;
        if (AnimationPlayer.HasAnimation(animName))
            AnimationPlayer.Play(animName);
	    
        if (Data.ResetAnimationProgress)
            AnimationPlayer.Seek(0f, true);
	    
        DanceIndex = (DanceIndex + 1) % Data.DanceList.Length;
    }
    
    /// <summary>
    /// Calls the dancing function, provided all the conditions are met.
    /// </summary>
    protected virtual void TryDance()
    {
        bool isCurrentAnimDance = AnimationPlayer.IsPlaying() && Data.DanceList.Any(x => x.Contains(AnimationPlayer.CurrentAnimation));
        bool isCurrentDanceDone = !isCurrentAnimDance || AnimationPlayer.CurrentAnimationPosition >= AnimationPlayer.CurrentAnimationLength;
        if ((Data.ForceDancing || !Data.ForceDancing && isCurrentDanceDone) && !FreezeDance)
            Dance();
    }
}