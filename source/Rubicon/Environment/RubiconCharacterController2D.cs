namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconCharacterController2D : RubiconCharacterController
{
    /// <summary>
    /// Flips left and right animations.
    /// </summary>
    [ExportGroup("Internals"), Export] public bool FlipAnimations = false;
    
    /// <summary>
    /// The character this controller is acting on.
    /// </summary>
    [Export] public RubiconCharacter2D Character;
    
    /// <inheritdoc />
    public override void Sing(string direction, bool holding = false, bool miss = false, string customPrefix = null, string customSuffix = null)
    {
        direction = direction.ToUpper();
        if (FlipAnimations)
            direction = direction switch
            {
                "LEFT" => "RIGHT",
                "RIGHT" => "LEFT",
                _ => direction
            };
	    
        base.Sing(direction, holding, miss, customPrefix, customSuffix);
    }

    public override Node GetGenericCharacter()
    {
        return Character;
    }
}