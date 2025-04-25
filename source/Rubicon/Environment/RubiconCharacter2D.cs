using Rubicon.Data;

namespace Rubicon.Environment;

/// <summary>
/// Character data for 2D environments.
/// </summary>
[GlobalClass] public partial class RubiconCharacter2D : Node2D, IRubiconCharacter
{
	/// <summary>
	/// Data defining how a character behaves in-game.
	/// </summary>
	[Export] public RubiconCharacterData Data;
	
	/// <summary>
	/// Determines whether the character is facing left or not.
	/// </summary>
	[Export] public bool LeftFacing = false;
	
    /// <summary>
    /// The reference visual node.
    /// </summary>
    [ExportGroup("References"), Export] public Node2D Sprite;
    
    /// <summary>
    /// The animation controller for this character.
    /// </summary>
    [Export] public AnimationPlayer AnimationPlayer;

    /// <summary>
    /// The node to reference its position and rotation initially when making <see cref="CameraPoint"/>.
    /// </summary>
    [Export] public RubiconCameraPointReference2D CameraPointReference;
    
    /// <summary>
    /// The node controlling this character.
    /// </summary>
    [ExportGroup("Internals"), Export] public RubiconCharacterController2D Controller;

	/// <summary>
	/// Defines the transform that <see cref="RubiconCameraController2D"/> references when pointing to this character.
	/// </summary>
    [Export] public RubiconCameraPoint2D CameraPoint;

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

	    Controller = new RubiconCharacterController2D();
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
