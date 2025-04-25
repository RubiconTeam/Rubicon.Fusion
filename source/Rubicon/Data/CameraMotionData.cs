using Rubicon.Enums;

namespace Rubicon;

/// <summary>
/// A class that holds useful data for camera motion, such as Lerping, Tweening and Smoothing.
/// </summary>
public partial class CameraMotionData : Resource
{
    /// <summary>
    /// Motion update type.
    /// See <see cref="CameraUpdate"/> for all the update types.
    /// </summary>
    [Export] public CameraUpdate UpdateType = CameraUpdate.Interpolation;
    
    /// <summary>
    /// Weight value when calculating motion lerp.
    /// Only affects <see cref="CameraUpdate.Interpolation"/> update type.
    /// </summary>
    [ExportGroup("Interpolation"), Export] public float LerpWeight = 2.4f;

    /// <summary>
    /// Duration of the motion tween.
    /// Only affects <see cref="CameraUpdate.Tween"/> update type.
    /// </summary>
    [ExportGroup("Tweening"), Export] public float TweenDuration = 0.5f;
    
    [Export] public Tween.TransitionType TweenTrans = Tween.TransitionType.Cubic;
    
    [Export] public Tween.EaseType TweenEase = Tween.EaseType.Out;
    
    [ExportGroup("Interals")] public Tween Tween;

    public bool IsTweenRunning() => Tween != null && Tween.IsRunning();

    public void KillTween()
    {
        Tween?.Kill();
        Tween = null;
    }
}