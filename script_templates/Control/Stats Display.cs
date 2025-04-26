using Rubicon.Core.API;
using Rubicon.Core.Data;

// This is a template for certain UI elements that hook into the game (i.e. combos, judgment).
// This can also act as a Node! So yes, you will have access to such things like _Process(delta).
public partial class NewStatDisplay : CsStatDisplay
{
    // This is basically your _Ready() function.
    // Do note that you can access the PlayField just by getting "PlayField"!
    public override void Initialize()
    {
		base.Initialize();
    }

    // Called whenever the player hits or misses a note.
    public override void UpdateStats(long combo, Judgment hit, float distance)
    {
        
    }

    // Called when the options are updated in-game.
    public override void OptionsUpdated()
    {
        
    }
}
