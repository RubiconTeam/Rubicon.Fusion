using System.Linq;

namespace Rubicon.Environment;

/// <summary>
/// This class serves as a spawn point for certain character or character group.
/// </summary>
[GlobalClass] public partial class RubiconCharacterSpawnPoint3D : Node3D
{
    /// <summary>
    /// Array of valid character nicknames.
    /// </summary>
    [Export] public StringName[] ValidNicknames = [];
    
    /// <summary>
    /// The container for this spawn point.
    /// </summary>
    [ExportGroup("Internals"), Export] public RubiconCharacter3D[] Characters = [];
    
    public void AddCharacter(RubiconCharacter3D character)
    {
        if (Characters.Contains(character))
            return;
        
        Characters = [..Characters, character];
        AddChild(character);
    }
}
