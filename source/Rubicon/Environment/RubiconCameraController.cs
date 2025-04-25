using Rubicon.Data;
using Rubicon.Enums;

namespace Rubicon.Environment;

[GlobalClass] public abstract partial class RubiconCameraController : Node
{
    /// <summary>
    /// Values used when updating the camera's position.
    /// </summary>
    [Export] public CameraMotionData PositionMotionData = new();
    
    /// <summary>
    /// Values used when updating the camera's rotation.
    /// </summary>
    [Export] public CameraMotionData RotationMotionData = new();
    
    /// <summary>
    /// Values used when updating the camera's zoom.
    /// </summary>
    [Export] public CameraMotionData ZoomMotionData = new();
    
    public override void _Process(double delta)
    {
        if (PositionMotionData == null && RotationMotionData == null && ZoomMotionData == null)
            return;
        
        float deltaF = (float)delta;
        
        if (PositionMotionData != null && PositionMotionData.UpdateType != CameraUpdate.None)
            UpdatePosition(deltaF);
        
        if (RotationMotionData != null && RotationMotionData.UpdateType != CameraUpdate.None)
            UpdateRotation(deltaF);
        
        if (ZoomMotionData != null && ZoomMotionData.UpdateType != CameraUpdate.None)
            UpdateZoom(deltaF);
    }

    /// <summary>
    /// Changes the target to the provided point's transform.
    /// </summary>
    /// <param name="point">The provided point</param>
    /// <param name="snap">Whether to teleport immediately to the point.</param>
    public abstract void FocusOnPoint(RubiconCameraPoint point, bool snap = false);
    
    /// <summary>
    /// Sets the initial position, rotation, and zoom for this camera.
    /// </summary>
    /// <remarks>The main difference between this and <see cref="FocusOnPoint"/> is that this sets the target zoom.</remarks>
    /// <param name="point">The point to set it to.</param>
    public abstract void SetInitialPoint(RubiconCameraPoint point);

    /// <summary>
    /// Increases the camera's zoom temporarily to create a "bounce" effect.
    /// </summary>
    public abstract void Bump();

    /// <summary>
    /// Updates the camera's position depending on the position's <see cref="CameraMotionData"/>.
    /// </summary>
    protected void UpdatePosition(float delta)
    {
        CameraMotionData data = PositionMotionData;
        switch (data.UpdateType)
        {
            case CameraUpdate.Instant:
                SetPositionInstant();
                break;
            case CameraUpdate.Interpolation:
                HandlePositionInterpolation(delta);
                break;
            case CameraUpdate.Tween:
                if (data.IsTweenRunning())
                    break;

                data.KillTween();
                data.Tween = CreatePositionTween();
                break;
        }
    }

    /// <summary>
    /// Updates the camera's rotation depending on the rotation's <see cref="CameraMotionData"/>.
    /// </summary>
    protected void UpdateRotation(float delta)
    {
        CameraMotionData data = RotationMotionData;
        switch (data.UpdateType)
        {
            case CameraUpdate.Instant:
                SetRotationInstant();
                break;
            case CameraUpdate.Interpolation:
                HandleRotationInterpolation(delta);
                break;
            case CameraUpdate.Tween:
                if (data.IsTweenRunning())
                    break;

                data.KillTween();
                data.Tween = CreateRotationTween();
                break;
        }
    }

    /// <summary>
    /// Updates the camera's zoom depending on the zoom's <see cref="CameraMotionData"/>.
    /// </summary>
    protected void UpdateZoom(float delta)
    {
        CameraMotionData data = ZoomMotionData;
        switch (data.UpdateType)
        {
            case CameraUpdate.Instant:
                SetZoomInstant();
                break;
            case CameraUpdate.Interpolation:
                HandleZoomInterpolation(delta);
                break;
            case CameraUpdate.Tween:
                if (data.IsTweenRunning())
                    break;

                data.KillTween();
                data.Tween = CreateZoomTween();
                break;
        }
    }

    protected abstract void SetPositionInstant();
    
    protected abstract void SetRotationInstant();
    
    protected abstract void SetZoomInstant();

    protected abstract void HandlePositionInterpolation(float delta);
    
    protected abstract void HandleRotationInterpolation(float delta);
    
    protected abstract void HandleZoomInterpolation(float delta);

    protected abstract Tween CreatePositionTween();
    
    protected abstract Tween CreateRotationTween();
    
    protected abstract Tween CreateZoomTween();
}