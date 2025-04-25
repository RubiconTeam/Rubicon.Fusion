namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconCameraPoint3D : RubiconCameraPoint
{
    [Export] public Transform3D Transform
    {
        get => _transform;
        set
        {
            _transform = value;
            ReferenceObject?.SetTransform(_transform);
        }
    }
    
    [ExportGroup("References"), Export] public float CustomZoom = 70f;
    
    [Export] public Node3D ReferenceObject;
    
    private Transform3D _transform;

    public void UpdateTransform()
    {
        if (ReferenceObject == null)
            return;

        Transform = ReferenceObject.GlobalTransform;
    }
}