namespace Rubicon.Environment;

public interface IRubiconCharacter
{
    /// <summary>
    /// Initializes the character's controller and camera point.
    /// </summary>
    public void Initialize();

    /// <summary>
    /// Gets the character controller in its most generic form.
    /// </summary>
    /// <returns>The character controller</returns>
    public RubiconCharacterController GetGenericController();
}