using Rubicon.Enums;

namespace Rubicon.Data;

[GlobalClass] public partial class RubiconDancerData : Resource
{
    /// <summary>
    /// A string array containing the sequence of idle/dance animations to be played.
    /// </summary>
    [Export] public string[] DanceList = ["idle"];

    /// <summary>
    /// The time of type to go by with <see cref="TimeInterval"/>.
    /// </summary>
    [Export] public TimeValue TimeValue;

    /// <summary>
    /// How often to dance.
    /// </summary>
    [Export] public float TimeInterval;
    
    /// <summary>
    /// If toggled off, the character will wait for the current dance animation to finish before dancing again. 
    /// </summary>
    [Export] public bool ForceDancing = true;

    /// <summary>
    /// If toggled, resets the animation player's position back to 0 when dancing.
    /// </summary>
    [Export] public bool ResetAnimationProgress = true;
}