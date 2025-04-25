namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconCameraPointReference3D : Camera3D
{
    [Export] public bool UseCustomZoom = false;

    [Export] public float CustomZoom = 40f;

    private RubiconCameraPoint3D _point = new();
    
    public RubiconCameraPoint3D GetPoint()
    {
        _point.ReferenceObject = this;
        _point.HasCustomZoom = UseCustomZoom;
        _point.CustomZoom = CustomZoom;
        return _point;
    }
}