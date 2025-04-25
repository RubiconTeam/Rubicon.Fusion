using Godot.Collections;

namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconCameraPoint2D : RubiconCameraPoint
{
    [Export] public Transform2D Transform;
    
    [Export] public Vector2 CustomZoom = Vector2.One;
    
    [ExportGroup("References"), Export] public Node2D ReferenceObject;

    public void UpdateTrasnform()
    {
        if (ReferenceObject == null)
            return;
        
        // Update transform
        Transform2D globalTrans = ReferenceObject.GetGlobalTransform();
        
        // Fix global positioning due to Parallax2D
        globalTrans.Origin = ReferenceObject.GetGlobalPositionExcludeParallax();
        
        Transform = globalTrans;
    }
}