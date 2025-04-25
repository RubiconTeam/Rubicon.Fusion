using System.Linq;
using Godot.Collections;

namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconStage3D : Node3D
{
    /// <summary>
    /// The initial position the camera should start at.
    /// </summary>
    [Export] public RubiconCameraPointReference3D InitialFocus;
    
    /// <summary>
    /// A list of defined spawn points to use when spawning in characters.
    /// </summary>
    [Export] public RubiconCharacterSpawnPoint3D[] SpawnPoints = [];
    
    /// <summary>
    /// Defined camera points to use alongside <see cref="RubiconEnvironmentSpace.FocusOnStagePoint"/>.
    /// </summary>
    [Export] public Dictionary<StringName, RubiconCameraPointReference3D> CameraPoints = [];

    /// <summary>
    /// An optional node for the camera to parent.
    /// </summary>
    [ExportGroup("Extras"), Export] public Node3D CameraParent;
    
    /// <summary>
    /// Metadata that can define pretty much any extra info needed, for scripting purposes.
    /// </summary>
    [Export] public Dictionary<StringName, Variant> DefinedMetadata = [];
    
    /// <summary>
    /// Checks if there's any valid spawn point for this nickname.
    /// </summary>
    /// <param name="name">The character nickname to find.</param>
    /// <returns>True if there was a spawn point found, otherwise false.</returns>
    public bool HasSpawnPointForNickname(StringName name)
    {
        return SpawnPoints.Any(x => x.ValidNicknames.Contains(name));
    }
    
    /// <summary>
    /// Grabs a spawn point for a specified character name.
    /// </summary>
    /// <param name="name">The character nickname to find</param>
    /// <returns>The first spawn point that allows that character, otherwise null.</returns>
    public RubiconCharacterSpawnPoint3D GetSpawnPointForNickname(StringName name)
    {
        return SpawnPoints.FirstOrDefault(x => x.ValidNicknames.Contains(name));
    }

    /// <summary>
    /// Checks if there's a camera point of the provided name.
    /// </summary>
    /// <param name="name">The camera point name</param>
    /// <returns>True if there does exist one, false if not</returns>
    public bool HasCameraPoint(StringName name)
    {
        return CameraPoints.ContainsKey(name);
    }
}