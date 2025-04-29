using System.Linq;
using Godot.Collections;
using Rubicon.Core.Meta;
using Camera2D = Godot.Camera2D;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconEnvironmentSpace : Node
{
    /// <summary>
    /// The 2D camera controller for viewing 2D stages.
    /// </summary>
    [Export] public RubiconCameraController2D Camera2D;
    
    /// <summary>
    /// The 3D camera controller for viewing 3D stages.
    /// </summary>
    [Export] public RubiconCameraController3D Camera3D;
    
    [Export] public BeatSyncer BeatSyncer;

    private Dictionary<StringName, RubiconCharacterController> _characters = [];
    private Dictionary<StringName, RubiconCharacterGroup> _characterGroups = [];
    private Dictionary<StringName, RubiconStage2D> _stage2Ds = [];
    private Dictionary<StringName, RubiconStage3D> _stage3Ds = [];

    /// <summary>
    /// Initializes this space for use.
    /// </summary>
    /// <param name="meta">The song meta</param>
    /// <param name="autoGenerate">Whether to auto-generate stages and characters.</param>
    public void Initialize(SongMeta meta, bool autoGenerate = true)
    {
        Camera2D = new RubiconCameraController2D();
        Camera2D.Name = "Controller";
        Camera2D.ZoomMotionData.LerpWeight = 3.125f;
        
        Camera3D = new RubiconCameraController3D();
        Camera3D.Name = "Controller";
        Camera3D.ZoomMotionData.LerpWeight = 3.125f;
        
        BeatSyncer = new BeatSyncer();
        BeatSyncer.Name = "CameraBeatSyncer";
        BeatSyncer.Value = 1f;
        BeatSyncer.Bumped += Bounce;
        AddChild(BeatSyncer);
        
        if (!autoGenerate)
            return;
        
        // TODO: ADD STAGE FALLBACK KMS
        RubiconStage2D first2DStage = null;
        RubiconStage3D first3DStage = null;
        for (int s = 0; s < meta.Stages.Length; s++)
        {
            string curStage = meta.Stages[s];
            bool hasStage = ResourceLoader.Exists(curStage);
            if (!hasStage)
                continue;

            PackedScene stageScene = ResourceLoader.Load<PackedScene>(curStage);
            Node stageNode = stageScene.Instantiate();
            AddChild(stageNode);

            if (stageNode is RubiconStage2D stage2D)
            {
                _stage2Ds[stage2D.Name] = stage2D;   
                if (first2DStage == null)
                    first2DStage = stage2D;
            }
            else if (stageNode is RubiconStage3D stage3D)
            {
                _stage3Ds[stage3D.Name] = stage3D;
                if (first3DStage == null)
                    first3DStage = stage3D;
            }
        }

        
        if (first2DStage != null)
        {
            if (first2DStage.CameraParent != null)
                Camera2D.Reparent(first2DStage.CameraParent);

            Camera2D camera = new Camera2D();
            camera.Name = "Camera2D";
            AddChild(camera);
            RegisterCamera2D(camera);

            if (first2DStage.InitialFocus != null)
                Camera2D.SetInitialPoint(first2DStage.InitialFocus.GetPoint());
        }
        
        if (first3DStage != null)
        {
            if (first3DStage.CameraParent != null)
                Camera3D.Reparent(first3DStage.CameraParent);
            
            Camera3D camera = new Camera3D();
            camera.Name = "Camera3D";
            AddChild(camera);
            RegisterCamera3D(camera);
            
            if (first3DStage.InitialFocus != null)
                Camera3D.SetInitialPoint(first3DStage.InitialFocus.GetPoint());
        }
        
        for (int c = 0; c < meta.Characters.Length; c++)
        {
            CharacterMeta curCharacter = meta.Characters[c];
            bool hasCharacter = ResourceLoader.Exists(curCharacter.Character);
            if (!hasCharacter)
                continue;
            
            PackedScene characterScene = ResourceLoader.Load<PackedScene>(curCharacter.Character);
            Node characterNode = characterScene.Instantiate();
            if (characterNode is not IRubiconCharacter character)
                continue;
            
            character.Initialize();
            characterNode.Name = curCharacter.Nickname;
            RegisterCharacter(character.GetGenericController(), curCharacter.BarLine);

            if (character is RubiconCharacter2D character2D)
            {
                RubiconStage2D validStage2D = _stage2Ds.Values.FirstOrDefault(x => x.HasSpawnPointForNickname(curCharacter.Nickname));
                if (validStage2D == null)
                    continue;
                
                validStage2D.GetSpawnPointForNickname(curCharacter.Nickname).AddCharacter(character2D);
            }
            else if (character is RubiconCharacter3D character3D)
            {
                RubiconStage3D validStage3D = _stage3Ds.Values.FirstOrDefault(x => x.HasSpawnPointForNickname(curCharacter.Nickname));
                if (validStage3D == null)
                    continue;
                
                validStage3D.GetSpawnPointForNickname(curCharacter.Nickname).AddCharacter(character3D);
            }
        }
    }

    /// <summary>
    /// Gets a character of the specified nickname.
    /// </summary>
    /// <param name="nickName">The nickname</param>
    /// <returns>A character if found, null if not.</returns>
    public RubiconCharacterController GetCharacter(StringName nickName)
    {
        return CollectionExtensions.GetValueOrDefault(_characters, nickName);
    }

    /// <summary>
    /// Gets a character group by group name.
    /// </summary>
    /// <param name="nickName">The group name</param>
    /// <returns>A character group if found, null if not.</returns>
    public RubiconCharacterGroup GetCharacterGroup(StringName nickName)
    {
        return CollectionExtensions.GetValueOrDefault(_characterGroups, nickName);
    }

    /// <summary>
    /// Gets a 2D stage by the specified stage name.
    /// </summary>
    /// <param name="stageName">The stage name</param>
    /// <returns>A stage if found, null if not.</returns>
    public RubiconStage2D GetStage2D(StringName stageName)
    {
        return CollectionExtensions.GetValueOrDefault(_stage2Ds, stageName);
    }
    
    /// <summary>
    /// Gets a 3D stage by the specified stage name.
    /// </summary>
    /// <param name="stageName">The stage name</param>
    /// <returns>A stage if found, null if not.</returns>
    public RubiconStage3D GetStage3D(StringName stageName)
    {
        return CollectionExtensions.GetValueOrDefault(_stage3Ds, stageName);
    }

    /// <summary>
    /// Registers the provided camera for this space to use.
    /// </summary>
    /// <param name="camera">The provided camera</param>
    public void RegisterCamera2D(Camera2D camera)
    {
        Camera2D.Camera = camera;
        
        if (Camera2D.GetParent() == null)
            camera.AddChild(Camera2D);
        else
            Camera3D.Reparent(camera);
    }

    /// <summary>
    /// Registers the provided camera for this space to use.
    /// </summary>
    /// <param name="camera">The provided camera</param>
    public void RegisterCamera3D(Camera3D camera)
    {
        Camera3D.Camera = camera;
        
        if (Camera3D.GetParent() == null)
            camera.AddChild(Camera3D);
        else
            Camera3D.Reparent(camera);
    }

    /// <summary>
    /// Makes the provided character be able to interact with the space. This does not add the character as a child!
    /// </summary>
    /// <param name="controller">The provided character's controller</param>
    /// <param name="groupName">The group to register the character under.</param>
    public void RegisterCharacter(RubiconCharacterController controller, StringName groupName)
    {
        _characters[controller.GetGenericCharacter().Name] = controller;
        
        if (!_characterGroups.ContainsKey(groupName))
            _characterGroups[groupName] = new RubiconCharacterGroup();
        
        _characterGroups[groupName].RegisterCharacter(controller);
    }

    public void FocusOnCharacter(StringName nickName)
    {
        RubiconCharacterController chara = _characters[nickName];
        
        if (chara is RubiconCharacterController2D chara2D)
            Camera2D?.FocusOnPoint(chara2D.Character.CameraPoint);
        
        if (chara is RubiconCharacterController3D chara3D)
            Camera3D?.FocusOnPoint(chara3D.Character.CameraPoint);
    }

    public void FocusOnCharacterGroup(StringName groupName)
    {
        RubiconCharacterGroup group = _characterGroups[groupName];
        
        Camera2D?.FocusOnPoint(group.GetCameraPoint2D());
        Camera3D?.FocusOnPoint(group.GetCameraPoint3D());
    }

    public void FocusOnStagePoint(StringName stagePointName)
    {
        RubiconStage2D validStage2D = _stage2Ds.Values.FirstOrDefault(x => x.HasCameraPoint(stagePointName));
        RubiconStage3D validStage3D = _stage3Ds.Values.FirstOrDefault(x => x.HasCameraPoint(stagePointName));
        
        if (validStage2D != null)
            Camera2D.FocusOnPoint(validStage2D.CameraPoints[stagePointName].GetPoint());
        
        if (validStage3D != null)
            Camera3D.FocusOnPoint(validStage3D.CameraPoints[stagePointName].GetPoint());
    }

    public void Bounce()
    {
        Camera2D?.Bump();
        Camera3D?.Bump();
    }
}