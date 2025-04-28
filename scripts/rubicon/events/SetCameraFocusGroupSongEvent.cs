using Godot.Collections;
using Rubicon.API;
using Rubicon.Game;

namespace Rubicon.Extras.Events;

// This is a template for events you place in the chart editor.
// This can also act as a Node! So yes, you will have access to such things like _Ready() and _Process(delta).
[GlobalClass] public partial class SetCameraFocusGroupSongEvent : CsSongEvent
{
	// Called when the event controller reaches this event.
	public override void CallEvent(float time, Dictionary<StringName, Variant> args)
	{
		StringName focusKey = new StringName("Focus");
		if (!args.ContainsKey(focusKey))
			return;
		
		StringName focusOn = args[focusKey].AsStringName();
		RubiconGame.Space.FocusOnCharacterGroup(focusOn);
	}
}

