using Godot.Collections;
using Rubicon.Core.Rulesets;

namespace Rubicon.Data;

[GlobalClass] public partial class RuleSetDatabase : Resource
{
    [Export] public Dictionary<StringName, RuleSet> Data { get; set; } = new();
}