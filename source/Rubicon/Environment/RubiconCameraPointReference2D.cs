namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconCameraPointReference2D : Marker2D
{
    [Export] public bool UseCustomZoom = false;

    [Export] public Vector2 CustomZoom = Vector2.One;
    
    private RubiconCameraPoint2D _point = new();

    public RubiconCameraPoint2D GetPoint()
    {
        _point.ReferenceObject = this;
        _point.HasCustomZoom = UseCustomZoom;
        _point.CustomZoom = CustomZoom;
        
        return _point;
    }
}