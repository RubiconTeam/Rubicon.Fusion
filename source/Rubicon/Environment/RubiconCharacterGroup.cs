using System.Collections.Generic;
using Godot.Collections;
using Rubicon.Data;

namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconCharacterGroup : RefCounted
{
    /// <summary>
    /// Every controller in the character group. Can include both 2D and 3D controllers.
    /// </summary>
    public RubiconCharacterController[] Controllers = [];
    
    /// <summary>
    /// All 2D characters in the group.
    /// </summary>
    public RubiconCharacter2D[] Character2Ds = [];
    
    /// <summary>
    /// All 3D characters in the group.
    /// </summary>
    public RubiconCharacter3D[] Character3Ds = [];

    private RubiconCameraPoint2D _point2D;
    private RubiconCameraPoint3D _point3D;
    
    public void SetGlobalPrefix(string prefix)
    {
        RubiconCharacterController[] controllers = Controllers;
        for (int i = 0; i < controllers.Length; i++)
            controllers[i].GlobalPrefix = prefix;
    }

    public void SetGlobalSuffix(string suffix)
    {
        RubiconCharacterController[] controllers = Controllers;
        for (int i = 0; i < controllers.Length; i++)
            controllers[i].GlobalSuffix = suffix;
    }

    public void SetFreezeSinging(bool freeze)
    {
        RubiconCharacterController[] controllers = Controllers;
        for (int i = 0; i < controllers.Length; i++)
            controllers[i].FreezeSinging = freeze;
    }

    public void SetCanDance(bool canDance)
    {
        RubiconCharacterController[] controllers = Controllers;
        for (int i = 0; i < controllers.Length; i++)
            controllers[i].FreezeDance = canDance;
    }
    
    public void Dance(string customPrefix = null, string customSuffix = null)
    {
        RubiconCharacterController[] controllers = Controllers;
        for (int i = 0; i < controllers.Length; i++)
            controllers[i].Dance(customPrefix, customSuffix);
    }

    public void Sing(string direction, bool holding = false, bool miss = false, string customPrefix = null, string customSuffix = null)
    {
        RubiconCharacterController[] controllers = Controllers;
        for (int i = 0; i < controllers.Length; i++)
            controllers[i].Sing(direction, holding, miss, customPrefix, customSuffix);
    }
    
    public void PlaySpecialAnimation(SpecialAnimation anim)
    {
        RubiconCharacterController[] controllers = Controllers;
        for (int i = 0; i < controllers.Length; i++)
            controllers[i].PlaySpecialAnimation(anim);
    }

    public void RegisterCharacter(RubiconCharacterController controller)
    {
        Controllers = [..Controllers, controller];
        
        if (controller is RubiconCharacterController2D chara2D)
            Character2Ds = [..Character2Ds, chara2D.Character];
        else if (controller is RubiconCharacterController3D chara3D)
            Character3Ds = [..Character3Ds, chara3D.Character];
    }

    public void DeregisterCharacter(RubiconCharacterController controller)
    {
        if (controller is RubiconCharacterController2D chara2D)
        {
            List<RubiconCharacter2D> character2Ds = [..Character2Ds];
            character2Ds.Remove(chara2D.Character);
            Character2Ds = character2Ds.ToArray();
        }
        else if (controller is RubiconCharacterController3D chara3D)
        {
            List<RubiconCharacter3D> character3Ds = [..Character3Ds];
            character3Ds.Remove(chara3D.Character);
            Character3Ds = character3Ds.ToArray();
        }
        
        List<RubiconCharacterController> controllers = [..Controllers];
        controllers.Remove(controller);
        Controllers = controllers.ToArray();
    }

    public RubiconCameraPoint2D GetCameraPoint2D()
    {
        if (_point2D == null)
            _point2D = new RubiconCameraPoint2D();

        if (Character2Ds.Length < 1)
            return _point2D;

        RubiconCameraPoint2D firstPoint = Character2Ds[0].CameraPoint;
        if (firstPoint == null)
            return _point2D;
        
        if (Character2Ds.Length == 1)
            return firstPoint;

        Transform2D transform = firstPoint.Transform;
        Vector2 min = _point2D.Transform.Origin;
        Vector2 max = min;
		
        for (int i = 1; i < Character2Ds.Length; i++)
        {
            RubiconCharacter2D character = Character2Ds[i];
            RubiconCameraPoint2D point = character.CameraPoint;
            if (point is null)
                continue;
            
            Vector2 camPos = point.Transform.Origin;
            min.X = Math.Min(camPos.X, min.X);
            min.Y = Math.Min(camPos.Y, min.Y);
            max.X = Math.Max(max.X, camPos.X);
            max.Y = Math.Max(max.Y, camPos.Y);
        }

        transform.Origin = new Vector2(min.X + (max.X - min.X) / 2f, min.Y + (max.Y - min.Y) / 2f);
        _point2D.Transform = transform;
        return _point2D;
    }
    
    public RubiconCameraPoint3D GetCameraPoint3D()
    {
        if (_point3D == null)
            _point3D = new RubiconCameraPoint3D();

        if (Character3Ds.Length < 1)
            return _point3D;

        RubiconCameraPoint3D firstPoint = Character3Ds[0].CameraPoint;
        if (firstPoint == null)
            return _point3D;
        
        if (Character3Ds.Length == 1)
            return firstPoint;

        Transform3D transform = firstPoint.Transform;
        Vector3 min = _point3D.Transform.Origin;
        Vector3 max = min;
		
        for (int i = 1; i < Character3Ds.Length; i++)
        {
            RubiconCharacter3D character = Character3Ds[i];
            RubiconCameraPoint3D point = character.CameraPoint;
            if (point is null)
                continue;
            
            Vector3 camPos = point.Transform.Origin;
			
            min.X = Math.Min(camPos.X, min.X);
            min.Y = Math.Min(camPos.Y, min.Y);
            min.Z = Math.Min(camPos.Z, min.Z);
            
            max.X = Math.Max(max.X, camPos.X);
            max.Y = Math.Max(max.Y, camPos.Y);
            max.Z = Math.Max(max.Z, camPos.Z);
        }

        transform.Origin = new Vector3(min.X + (max.X - min.X) / 2f, min.Y + (max.Y - min.Y) / 2f, min.Z + (max.Z - min.Z) / 2f);
        _point3D.Transform = transform;
        return _point3D;
    }
}