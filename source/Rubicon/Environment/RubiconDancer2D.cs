using Rubicon.Data;

namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconDancer2D : Node2D
{
    /// <summary>
    /// Data referenced when controlling this dancer.
    /// </summary>
    [Export] public RubiconDancerData Data;

    /// <summary>
    /// The reference visual node.
    /// </summary>
    [ExportGroup("References"), Export] public Node2D Sprite;

    /// <summary>
    /// The node that controls this dancer.
    /// </summary>
    [ExportGroup("Internals"), Export] public RubiconDancerController Controller;
}