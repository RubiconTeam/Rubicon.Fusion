using Rubicon.Core.Meta;

namespace Rubicon.Game;

/// <summary>
/// An object that helps in defining what song to load in <see cref="RubiconGameInstance"/>.
/// </summary>
[GlobalClass]
public partial class RubiconGameLoadContext : RefCounted
{
    /// <summary>
    /// The name of the song to load.
    /// </summary>
    [Export] public string Name = ProjectSettings.GetSetting("rubicon/general/fallback/song").AsString();
    
    /// <summary>
    /// The difficulty to load.
    /// </summary>
    [Export] public string Difficulty = ProjectSettings.GetSetting("rubicon/general/fallback/difficulty").AsString();

    /// <summary>
    /// The rule set to play with.
    /// </summary>
    [Export] public string RuleSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();

    /// <summary>
    /// Defines which bar line is chosen in <see cref="SongMeta.PlayableCharts"/>. Chooses the first index by default.
    /// </summary>
    [Export] public int TargetIndex = 0;

    /// <summary>
    /// Instantiate the stage and characters.
    /// </summary>
    [Export] public bool AutoGenerate = true;

    /// <summary>
    /// Checks for any errors and reports any errors to the Godot console if any are found.
    /// </summary>
    /// <returns>Whether the context is fully valid.</returns>
    public bool IsValid()
    {
        // TODO: Make this function probably
        return true;
    }
}