namespace Rubicon.Game;

[GlobalClass] public partial class HudBounce : Control
{
    [Export] public Vector2 TargetScale = Vector2.One;
    
    [Export] public float ReturnLerpWeight = 3.125f;

    [Export] public Vector2 BounceIntensity = Vector2.One * 0.015f;
    
    [Export] public BeatSyncer BeatSyncer;

    public override void _Ready()
    {
        base._Ready();
        
        SetAnchorsPreset(LayoutPreset.FullRect);

        BeatSyncer = new BeatSyncer();
        BeatSyncer.Name = "BeatSyncer";
        BeatSyncer.Value = 1f;
        BeatSyncer.Bumped += OnBumped;
        AddChild(BeatSyncer);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        PivotOffset = Size / 2f;

        Vector2 currentScale = Scale;
        Scale = currentScale.Lerp(TargetScale, ReturnLerpWeight * (float)delta);
    }
    
    public void Bounce(Vector2 intensity)
    {
        Scale += intensity;
    }

    private void OnBumped()
    {
        Bounce(BounceIntensity);
    }
}