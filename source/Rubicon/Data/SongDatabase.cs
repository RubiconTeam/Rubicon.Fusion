using Godot.Collections;
using Rubicon.Core.Meta;

namespace Rubicon.Data;

[GlobalClass] public partial class SongDatabase : Resource
{
    [Export] public Dictionary<StringName, SongMeta> Data = [];
}