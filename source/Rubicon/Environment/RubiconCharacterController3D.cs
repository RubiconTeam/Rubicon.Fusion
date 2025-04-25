namespace Rubicon.Environment;

[GlobalClass] public partial class RubiconCharacterController3D : RubiconCharacterController
{
    /// <summary>
    /// The character this controller is acting on.
    /// </summary>
    [Export] public RubiconCharacter3D Character;
    
    public override Node GetGenericCharacter()
    {
        return Character;
    }
}