namespace Rubicon.Environment;

/// <summary>
/// <see cref="Camera3D"/> utility that manages smooth positioning, tweening, and FOV changes. 
/// </summary>
[GlobalClass] public partial class RubiconCameraController3D : RubiconCameraControllerGeneric<Camera3D>
{
    /// <summary>
    /// The final position to smoothly move to.
    /// </summary>
    [Export] public Vector3 TargetPosition = Vector3.Zero;
    
    /// <summary>
    /// Offset added to <see cref="TargetPosition"/> when calculating a new position.
    /// Similar to <see cref="Camera2D.Offset"/>, except it also gets smoothed.
    /// </summary>
    [Export] public Vector3 OffsetPosition = Vector3.Zero;
    
    /// <summary>
    /// The final rotation to smoothly move to.
    /// </summary>
    [Export] public Vector3 TargetRotation = Vector3.Zero;
    
    /// <summary>
    /// Offset added to <see cref="TargetRotation"/> when calculating a new rotation.
    /// </summary>
    [Export] public Vector3 OffsetRotation = Vector3.Zero;
    
    /// <summary>
    /// The target FOV for this camera.
    /// </summary>
    [Export] public float TargetZoom = 0f;
    
    /// <summary>
    /// Offset added to <see cref="TargetZoom"/> when calculating a new FOV value.
    /// </summary>
    [Export] public float OffsetZoom = 0f;

    /// <summary>
    /// The amount to bump the camera's zoom when <see cref="Bump"/> is called.
    /// </summary>
    [ExportGroup("Bumping"), Export] public float BumpAmount = -2f;

    public override void _Ready()
    {
        base._Ready();
        
        Camera?.SetFov(TargetZoom + OffsetZoom);
    }

    public override void FocusOnPoint(RubiconCameraPoint point, bool snap = false)
    {
        if (!IsInsideTree() || Camera == null || point is not RubiconCameraPoint3D point3d) 
            return;

        Node3D closestParent = Camera.GetClosest3DParent();

        point3d.UpdateTransform();
        Transform3D pointTransform = point3d.Transform;
        
        TargetPosition = pointTransform.Origin - closestParent.GlobalPosition;
        TargetRotation = pointTransform.Basis.GetEuler() - closestParent.GlobalRotation;
        OffsetZoom = 0f;
        
        if (point.HasCustomZoom)
            OffsetZoom = point3d.CustomZoom - TargetZoom;
        
        if (!snap)
            return;

        Camera.Position = TargetPosition + OffsetPosition;
        Camera.Rotation = TargetRotation + OffsetRotation;
        Camera.Fov = TargetZoom + OffsetZoom;
    }

    public override void SetInitialPoint(RubiconCameraPoint point)
    {
        if (Camera == null || point is not RubiconCameraPoint3D point3d) 
            return;

        Node3D closestParent = Camera.GetClosest3DParent();

        point3d.UpdateTransform();
        Transform3D pointTransform = point3d.Transform;
        
        Camera.Position = TargetPosition = pointTransform.Origin - closestParent.GlobalPosition;
        Camera.Rotation = TargetRotation = pointTransform.Basis.GetEuler() - closestParent.GlobalRotation;
        Camera.Fov = TargetZoom = point3d.CustomZoom;
    }

    public override void Bump()
    {
        if (Camera == null)
            return;
        
        Camera.Fov += BumpAmount;
    }

    protected override void SetPositionInstant()
    {
        Camera.Position = TargetPosition + OffsetPosition;
    }

    protected override void SetRotationInstant()
    {
        Camera.Rotation = TargetRotation + OffsetRotation;
    }

    protected override void SetZoomInstant()
    {
        Camera.Fov = TargetZoom + OffsetZoom;
    }

    protected override void HandlePositionInterpolation(float delta)
    {
        Vector3 curPos = Camera.Position;
        Vector3 targetPos = TargetPosition + OffsetPosition;
        Camera.Position = curPos.Lerp(targetPos, PositionMotionData.LerpWeight * delta);
    }

    protected override void HandleRotationInterpolation(float delta)
    {
        Basis curBasis = Camera.Basis;
        Basis targetBasis = Basis.FromEuler(TargetRotation + OffsetRotation);
        Camera.Basis = curBasis.Slerp(targetBasis, RotationMotionData.LerpWeight * delta);
    }

    protected override void HandleZoomInterpolation(float delta)
    {
        float curFov = Camera.Fov;
        float targetFov = TargetZoom + OffsetZoom;
        Camera.Fov = Mathf.Lerp(curFov, targetFov, ZoomMotionData.LerpWeight * delta);
    }
    
    protected override Tween CreatePositionTween()
    {
        CameraMotionData data = PositionMotionData;
        
        Tween tween = Camera.CreateTween();
        tween.TweenProperty(Camera, "position", TargetPosition + OffsetPosition, data.TweenDuration)
            .SetTrans(data.TweenTrans)
            .SetEase(data.TweenEase);

        return tween;
    }
    
    protected override Tween CreateRotationTween()
    {
        CameraMotionData data = RotationMotionData;

        Basis targetBasis = Basis.FromEuler(TargetRotation + OffsetRotation);
        Tween tween = Camera.CreateTween();
        tween.TweenProperty(Camera, "basis", targetBasis, data.TweenDuration)
            .SetTrans(data.TweenTrans)
            .SetEase(data.TweenEase);

        return tween;
    }

    protected override Tween CreateZoomTween()
    {
        CameraMotionData data = ZoomMotionData;
        
        Tween tween = Camera.CreateTween();
        tween.TweenProperty(Camera, "fov", TargetZoom + OffsetZoom, data.TweenDuration)
            .SetTrans(data.TweenTrans)
            .SetEase(data.TweenEase);

        return tween;
    }
}
