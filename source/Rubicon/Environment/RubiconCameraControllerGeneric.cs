namespace Rubicon.Environment;

public abstract partial class RubiconCameraControllerGeneric<T> : RubiconCameraController where T : Node
{
    /// <summary>
    /// Represents this object in world space.
    /// </summary>
    public T Camera;
    
    public override void _Process(double delta)
    {
        if (Camera is null)
            return;
        
        base._Process(delta);
    }
}