namespace Rubicon.Environment;

/// <summary>
/// <see cref="Camera2D"/> utility that manages smooth positioning, tweening, and zooming. 
/// </summary>

[GlobalClass] public partial class RubiconCameraController2D : RubiconCameraControllerGeneric<Camera2D>
{
    /// <summary>
    /// The final position to smoothly move to.
    /// </summary>
    [Export] public Vector2 TargetPosition = Vector2.Zero;
    
    /// <summary>
    /// Offset added to <see cref="TargetPosition"/> when calculating a new position.
    /// Similar to <see cref="Camera2D.Offset"/>, except it also gets smoothed.
    /// </summary>
    [Export] public Vector2 OffsetPosition = Vector2.Zero;
    
    /// <summary>
    /// The final rotation to smoothly move to.
    /// </summary>
    [Export] public float TargetRotation = 0f;
    
    /// <summary>
    /// Offset added to <see cref="TargetRotation"/> when calculating a new rotation.
    /// </summary>
    [Export] public float OffsetRotation = 0f;
    
    /// <summary>
    /// The final zoom to smoothly move to.
    /// </summary>
    [Export] public Vector2 TargetZoom = Vector2.One;
    
    /// <summary>
    /// Offset added to <see cref="TargetZoom"/> when calculating a new zoom value.
    /// </summary>
    [Export] public Vector2 OffsetZoom = Vector2.Zero;

    /// <summary>
    /// The amount to bump the camera's zoom when <see cref="Bump"/> is called.
    /// </summary>
    [ExportGroup("Bumping"), Export] public Vector2 BumpAmount = Vector2.One * 0.03f;

    public override void _Ready()
    {
        base._Ready();
        
        Camera?.SetZoom(TargetZoom + OffsetZoom);
    }

    public override void FocusOnPoint(RubiconCameraPoint point, bool snap = false)
    {
        if (!IsInsideTree() || Camera == null || point is not RubiconCameraPoint2D point2d) 
            return;

        Node closestParent = Camera.GetClosest2DParent();
        Vector2 globalPos = Vector2.Zero;
        float globalRot = 0f;

        if (closestParent is Node2D node2D)
        {
            globalPos = node2D.GetGlobalPositionExcludeParallax();
            globalRot = node2D.GlobalRotation;
        }
        else if (closestParent is Control control)
        {
            globalPos = control.GetGlobalPositionExcludeParallax();
            globalRot = control.GetGlobalRotation();
        }

        point2d.UpdateTrasnform();
        Transform2D pointTransform = point2d.Transform;
        
        TargetPosition = pointTransform.Origin - globalPos;
        TargetRotation = pointTransform.Rotation - globalRot;
        OffsetZoom = Vector2.Zero;
        
        if (point.HasCustomZoom)
            OffsetZoom = point2d.CustomZoom - TargetZoom;

        if (!snap)
            return;

        Camera.Position = TargetPosition + OffsetPosition;
        Camera.Rotation = TargetRotation + OffsetRotation;
        Camera.Zoom = TargetZoom + OffsetZoom;
    }

    public override void SetInitialPoint(RubiconCameraPoint point)
    {
        if (Camera == null || point is not RubiconCameraPoint2D point2d) 
            return;

        Node closestParent = Camera.GetClosest2DParent();
        Vector2 globalPos = Vector2.Zero;
        float globalRot = 0f;

        if (closestParent is Node2D node2D)
        {
            globalPos = node2D.GetGlobalPositionExcludeParallax();
            globalRot = node2D.GlobalRotation;
        }
        else if (closestParent is Control control)
        {
            globalPos = control.GetGlobalPositionExcludeParallax();
            globalRot = control.GetGlobalRotation();
        }

        point2d.UpdateTrasnform();
        Transform2D pointTransform = point2d.Transform;
        
        Camera.Position = TargetPosition = pointTransform.Origin - globalPos;
        Camera.Rotation = TargetRotation = pointTransform.Rotation - globalRot;
        Camera.Zoom = TargetZoom = point2d.CustomZoom;
    }

    public override void Bump()
    {
        if (Camera == null)
            return;
        
        Camera.Zoom += BumpAmount;
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
        Camera.Zoom = TargetZoom + OffsetZoom;
    }

    protected override void HandlePositionInterpolation(float delta)
    {
        Vector2 curPos = Camera.Position;
        Vector2 targetPos = TargetPosition + OffsetPosition;
        Camera.Position = curPos.Lerp(targetPos, PositionMotionData.LerpWeight * delta);
    }

    protected override void HandleRotationInterpolation(float delta)
    {
        float curRotation = Camera.Rotation;
        float targetRotation = TargetRotation + OffsetRotation;
        Camera.Rotation = Mathf.Lerp(curRotation, targetRotation, RotationMotionData.LerpWeight * delta);
    }

    protected override void HandleZoomInterpolation(float delta)
    {
        Vector2 curZoom = Camera.Zoom;
        Vector2 targetZoom = TargetZoom + OffsetZoom;
        Camera.Zoom = curZoom.Lerp(targetZoom, ZoomMotionData.LerpWeight * delta);
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

        Tween tween = Camera.CreateTween();
        tween.TweenProperty(Camera, "rotation", TargetRotation + OffsetRotation, data.TweenDuration)
            .SetTrans(data.TweenTrans)
            .SetEase(data.TweenEase);

        return tween;
    }

    protected override Tween CreateZoomTween()
    {
        CameraMotionData data = ZoomMotionData;
        
        Tween tween = Camera.CreateTween();
        tween.TweenProperty(Camera, "zoom", TargetZoom + OffsetZoom, data.TweenDuration)
            .SetTrans(data.TweenTrans)
            .SetEase(data.TweenEase);

        return tween;
    }
}
