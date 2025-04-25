using Rubicon.Enums;

namespace Rubicon.Data;

[GlobalClass] public partial class RubiconCharacterData : RubiconDancerData
{
    /// <summary>
    /// The icon data for this character.
    /// </summary>
    [Export] public CharacterIconData Icon;
    
    /// <summary>
    /// Determines how holding is handled animation-wise.
    /// </summary>
    [Export] public CharacterHold HoldType = CharacterHold.Freeze;

    /// <summary>
    /// The interval on the AnimationPlayer when the animation will loop, if <see cref="HoldType"/> is set to <see cref="CharacterHold.Repeat"/>.
    /// </summary>
    [Export] public double RepeatLoopPoint = 0.125;
}