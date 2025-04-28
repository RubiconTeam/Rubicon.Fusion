using System.Collections.Generic;
using System.Data;
using System.Text;
using PukiTools.GodotSharp.Screens;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Data;
using Rubicon.Core.Events;
using Rubicon.Core.Meta;
using Rubicon.Core.Rulesets;

namespace Rubicon.Game;

[GlobalClass] public partial class RubiconGameScreen : CsScreen
{
#if TOOLS
	[Export] public string SongName = ProjectSettings.GetSetting("rubicon/general/fallback/song").AsString();
	
	[Export] public string Difficulty = ProjectSettings.GetSetting("rubicon/general/fallback/difficulty").AsString();
	
	[Export] public string RuleSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
#endif
    
    public override void ReadyPreload()
	{
		base.ReadyPreload();
		
		RubiconGameLoadContext context = RubiconGame.Context;
		SongMeta meta = RubiconEngine.Songs.Data[context.Name];
		for (int s = 0; s < meta.Stages.Length; s++)
			ScreenManager.AddPath(meta.Stages[s]);
		
		for (int c = 0; c < meta.Characters.Length; c++)
			ScreenManager.AddPath(meta.Characters[c].Character);
		
		for (int m = 0; m < meta.Modules.Length; m++)
			ScreenManager.AddPath(meta.Modules[m].Path);
		
		for (int g = 0; g < RubiconEngine.GlobalModules.Modules.Length; g++)
			ScreenManager.AddPath(RubiconEngine.GlobalModules.Modules[g].Path);

		RuleSet ruleSet = RubiconEngine.RuleSets.Data[context.RuleSet];
		NoteSkinDatabase noteSkins = RubiconCore.NoteSkins;
		ScreenManager.AddPath(noteSkins.Skins[meta.NoteSkin].Rulesets[ruleSet.UniqueId].Path);

		RubiChart chart = meta.GetDifficultyByName(context.Difficulty, context.RuleSet).Chart;
		string[] noteTypeList = chart.GetAllNoteTypes();
		for (int n = 0; n < noteTypeList.Length; n++)
			ScreenManager.AddPath(RubiconCore.NoteTypes.Paths[noteTypeList[n]].Path);

		EventMeta eventMeta = meta.Events;
		List<string> eventsPassed = [];
		for (int i = 0; i < eventMeta.Events.Length; i++)
		{
			string eventName = eventMeta.Events[i].Name;
			if (eventsPassed.Contains(eventName))
				continue;
				
			eventsPassed.Add(eventName);
			ScreenManager.AddPath(RubiconEngine.Events.Paths[eventName].Path);
		}
	}

	public override void OnPreload(string path)
	{
		
	}

	public override void _Ready()
	{
		Name = "RubiconGameScreen";
		
		#if TOOLS
		if (RubiconGame.Context == null)
			RubiconGame.Context = new RubiconGameLoadContext() { Name = SongName, Difficulty = Difficulty, RuleSet = RuleSet };
		#endif

		base._Ready();
		
		if (!IsLoaded())
			return;
		
		RubiconGame.Setup(this);
	}

	public override string GetDebugInfo()
	{
		if (!RubiconGame.Active)
			return "RubiconGame is not active yet!";

		SongMeta songMeta = RubiconGame.Metadata;
		RubiChart chart = RubiconGame.Chart;
		RubiconGameLoadContext context = RubiconGame.Context;
		PlayField playField = RubiconGame.PlayField;
		ScoreManager scoreManager = playField.ScoreManager;
		
		StringBuilder debugInfo = new StringBuilder();
		
		// Conductor
		TimeChange curTimeChange = Conductor.GetCurrentTimeChange();
		debugInfo.AppendLine($"BPM: {curTimeChange.Bpm} [{curTimeChange.TimeSignatureNumerator}/{curTimeChange.TimeSignatureDenominator}] | Time: {Conductor.Time} | Audio Time: {Conductor.AudioTime}")
			.AppendLine($"Step: {Conductor.CurrentStep} | Beat: {Conductor.CurrentBeat} | Measure: {Conductor.CurrentMeasure}");

		debugInfo.AppendLine("BPM List:");
		foreach (TimeChange bpm in Conductor.TimeChanges)
			debugInfo.AppendLine($"[Time: {bpm.Time} | Exact Time (ms): {bpm.MsTime} | BPM: {bpm.Bpm} | Time Signature: {bpm.TimeSignatureNumerator}/{bpm.TimeSignatureDenominator}]");
		
		// Scores
		debugInfo.AppendLine();
		debugInfo.AppendLine($"Score: {scoreManager.Score} | Accuracy: {scoreManager.Accuracy}% | Rank: {scoreManager.Rank.ToString()} | Clear: {scoreManager.Clear.ToString()}");
		debugInfo.AppendLine($"Perfects: {scoreManager.PerfectHits} | Greats: {scoreManager.GreatHits} | Goods: {scoreManager.GoodHits} | Okays: {scoreManager.OkayHits} | Bads: {scoreManager.BadHits} | Misses: {scoreManager.Misses} [Streak: {scoreManager.MissStreak}]");
		debugInfo.AppendLine($"Combo: {scoreManager.Combo} | Highest Combo: {scoreManager.HighestCombo}");
		
		// Song Meta
		debugInfo.AppendLine();
		debugInfo.AppendLine($"Song Name: {songMeta.Name} by {songMeta.Artist} [{context.Name}] / Ruleset: {context.RuleSet} / Difficulty: {context.Difficulty}");
		debugInfo.AppendLine($"Note Skin: {songMeta.NoteSkin} / UI Style: {songMeta.UiStyle}");
		debugInfo.AppendLine($"Charter: {chart.Charter} / Difficulty: {chart.Difficulty} / Speed: {chart.ScrollSpeed}");
		
		// Bar Lines
		debugInfo.AppendLine();
		
		debugInfo.Append("Bar Lines: [");
		for (int i = 0; i < playField.BarLines.Length; i++)
			debugInfo.Append(playField.BarLines[i].Name + (i < playField.BarLines.Length - 1 ? ", " : ""));

		debugInfo.AppendLine("]");
		
		debugInfo.Append("Target Bar Line: ");
		debugInfo.AppendLine(playField.TargetBarLine);
		
		/*
		if (songMeta.Environment == GameEnvironment.None)
			return debugInfo.ToString();

		debugInfo.AppendLine();
		debugInfo.AppendLine("Characters:");
		switch (songMeta.Environment)
		{
			case GameEnvironment.CanvasItem:
				CanvasItemSpace canvasSpace = RubiconGame.CanvasItemSpace;
				for (int i = 0; i < playField.BarLines.Length; i++)
				{
					string barLine = playField.BarLines[i].Name;
					CharacterGroup2D group = canvasSpace.GetCharacterGroup(barLine);
					if (group == null)
						continue;
					
					debugInfo.Append($"[{barLine}] => [");
					for (int j = 0; j < group.Characters.Count; j++)
						debugInfo.Append(group.Characters[j].Name + (i < group.Characters.Count - 1 ? ", " : ""));
					debugInfo.AppendLine("]");
				}
				break;
			case GameEnvironment.Spatial:
				SpatialSpace spatialSpace = RubiconGame.SpatialSpace;
				for (int i = 0; i < playField.BarLines.Length; i++)
				{
					string barLine = playField.BarLines[i].Name;
					CharacterGroup3D group = spatialSpace.GetCharacterGroup(barLine);
					if (group == null)
						continue;
					
					debugInfo.Append($"[{barLine}] => [");
					for (int j = 0; j < group.Characters.Count; j++)
						debugInfo.Append(group.Characters[j].Name + (i < group.Characters.Count - 1 ? ", " : ""));
					debugInfo.AppendLine("]");
				}
				break;
		}

		debugInfo.Remove(debugInfo.Length - 1, 1);*/
		return debugInfo.ToString();
	}
}