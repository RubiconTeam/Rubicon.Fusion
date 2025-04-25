using Rubicon.Core.Data;

namespace Rubicon.Data;

[GlobalClass] public partial class ModuleDatabase : Resource
{
    [Export] public ModulePathData[] Modules = [];
}