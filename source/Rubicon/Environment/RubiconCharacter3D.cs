using Rubicon.Data;

namespace Rubicon.Environment;

/// <summary>
/// Character data for 3D environments.
/// </summary>
[GlobalClass] public partial class RubiconCharacter3D : Node3D, IRubiconCharacter
{
    /// <summary>
    /// Data defining how a character behaves in-game.
    /// </summary>
    [Export] public RubiconCharacterData Data;
    
    /// <summary>
    /// The reference visual node.
    /// </summary>
    [ExportGroup("References"), Export] public Node3D VisualObject;
    
    /// <summary>
    /// The animation controller for this character.
    /// </summary>
    [Export] public AnimationPlayer AnimationPlayer;
    
    /// <summary>
    /// The node to reference its position and rotation initially when making <see cref="CameraPoint"/>.
    /// </summary>
    [Export] public RubiconCameraPointReference3D CameraPointReference;
    
    /// <summary>
    /// Flips left and right animations.
    /// </summary>
    [ExportGroup("Internals"), Export] public bool FlipAnimations = false;

    /// <summary>
    /// The node controlling this character.
    /// </summary>
    [Export] public RubiconCharacterController3D Controller;

    /// <summary>
    /// Defines the transform that <see cref="RubiconCameraController2D"/> references when pointing to this character.
    /// </summary>
    [Export] public RubiconCameraPoint3D CameraPoint;
    
    private bool _initialized = false;

    public override void _Ready()
    {
        base._Ready();
		
        if (!_initialized)
            Initialize();
    }
	
    public void Initialize()
    {
        _initialized = true;
        CameraPoint = CameraPointReference.GetPoint();
        
        Controller = new RubiconCharacterController3D();
        Controller.Name = "Controller";
        Controller.Character = this;
        Controller.Data = Data;
        Controller.AnimationPlayer = AnimationPlayer;
        AddChild(Controller);
    }
    
    public RubiconCharacterController GetGenericController()
    {
        return Controller;
    }
}
