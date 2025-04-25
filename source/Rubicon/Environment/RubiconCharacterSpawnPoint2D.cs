using System.Linq;

namespace Rubicon.Environment;

/// <summary>
/// This class serves as a spawn point for certain character or character group.
/// Determines its position and whether it should be flipped or not.
/// </summary>
[GlobalClass] public partial class RubiconCharacterSpawnPoint2D : Node2D
{
    /// <summary>
    /// Array of valid character nicknames.
    /// </summary>
    [Export] public StringName[] ValidNicknames = [];
    
    /// <summary>
    /// Spawn point is facing the Z+ axis. 
    /// </summary>
    [Export] public bool LeftFacing;

    /// <summary>
    /// The container for this spawn point.
    /// </summary>
    [ExportGroup("Internals"), Export] public RubiconCharacter2D[] Characters = [];

    public void AddCharacter(RubiconCharacter2D character)
    {
        if (Characters.Contains(character))
            return;
        
        Characters = [..Characters, character];
        AddChild(character);
        
        if (LeftFacing == character.LeftFacing)
            return;
        
        Vector2 scale = character.Scale;
        scale.X *= -1f;
        character.Scale = scale;

        character.Controller.FlipAnimations = true;
    }
}