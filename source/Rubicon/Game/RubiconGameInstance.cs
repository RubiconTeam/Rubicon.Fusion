using System.Collections.Generic;
using PukiTools.GodotSharp;
using PukiTools.GodotSharp.Audio;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;
using Rubicon.Core.Data;
using Rubicon.Core.Rulesets;
using Rubicon.Environment;

namespace Rubicon.Game;

/// <summary>
/// The main node that brings characters, stages, and ruleset gameplay together. Serves as "PlayState" in other Funkin' engines.
/// </summary>
[GlobalClass, Autoload("RubiconGame")]
public partial class RubiconGameInstance : CanvasLayer
{
	/// <summary>
	/// Disables <see cref="_Process"/> and <see cref="_Input"/> when false.
	/// </summary>
	[Export] public bool Active = false;
	
	/// <summary>
	/// The load context of the current song.
	/// Helps loading songs, charts, metadata and others.
	/// </summary>
	[Export] public RubiconGameLoadContext Context;
	
	/// <summary>
	/// The instrumental of the current song and difficulty.
	/// Obligatory for the song to work.
	/// </summary>
	[Export] public AudioStreamPlayer Instrumental
	{
		get => PlayField.Music;
		set => PlayField.Music = value;
	}

	/// <summary>
	/// The vocals of the current song.
	/// Not required for the song to start.
	/// </summary>
	[Export] public AudioStreamPlayer Vocals;
	
	[ExportGroup("Status"), Export] public bool Paused = false;

	/// <summary>
	/// The metadata of the current song.
	/// Obligatory for the song to work.
	/// </summary>
	[Export] public SongMeta Metadata;

	/// <summary>
	/// The chart of the current song.
	/// Obligatory for the song to work.
	/// </summary>
	[Export] public RubiChart Chart;

	/// <summary>
	/// The events of the current song.
	/// Not required for the song to start.
	/// </summary>
	[Export] public EventMeta Events;
	
	/// <summary>
	/// The RuleSet of the current song.
	/// The default RuleSet is Mania, found in Project Settings (rubicon/rulesets/default_ruleset).
	/// </summary>
	[ExportGroup("References"), Export] public RuleSet RuleSet;
	
	/// <summary>
	/// The PlayField of the current song.
	/// </summary>
	[Export] public PlayField PlayField;

	/// <summary>
	/// Executes <see cref="Bounce"/> depending on the bpm,
	/// <see cref="TimeValue"/> and step.
	/// </summary>
	[Export] public BeatSyncer BounceBeatSyncer;

	/// <summary>
	/// The Root/Parent node of <see cref="RubiconGameInstance"/>, usually <see cref="RubiconGameScreen"/>
	/// </summary>
	[Export] public Node RootNode;

	/// <summary>
	/// The place the characters and stages take place.
	/// </summary>
	[Export] public RubiconEnvironmentSpace Space;

	/// <summary>
	/// The class responsible for loading and executing events.
	/// </summary>
	[Export] public SongEventController EventController;

	private string[] _actionNames;

	/// <summary>
	/// Sets up <see cref="RubiconGameInstance"/> for use.
	/// Loads RuleSets, Charts and Metadata, PlayField 
	/// </summary>
	/// <param name="rootNode"></param>
	/// <exception cref="Exception"></exception>
	public void Setup(Node rootNode)
	{
		if (!Context.IsValid())
			return;

		Active = true;
		Layer = 16;
		
		Metadata = RubiconEngine.Songs.Data[Context.Name];
		Chart = Metadata.GetDifficultyByName(Context.Difficulty, Context.RuleSet).Chart;
		Events = Metadata.Events;
		
		RootNode = rootNode ?? this;
		
		// Set up rule set, and check paths too
		string ruleSetName = Context.RuleSet;
		RuleSet = RubiconEngine.RuleSets.Data[ruleSetName];
		if (Metadata.Vocals is not null)
			Vocals = AudioManager.GetGroup("Music").Play(Metadata.Vocals, false);

		LoadSpace(Metadata);
		
		// Set up play field
		PlayField = LoadPlayField(RuleSet);
		PlayField.Setup(RuleSet, Metadata, Chart, Context.TargetIndex);
		PlayField.NoteHit += NoteHit;
		AddChild(PlayField);
		PrintUtility.Print("RubiconGame", "PlayField loaded successfully.", true);
		
		if (Events != null)
		{
			for (int i = 0; i < Events.Events.Length; i++)
				Events.Events[i].ConvertData(Metadata.TimeChanges);
            
			EventController = new SongEventController();
			EventController.Name = "Event Controller";
			EventController.Setup(Events, PlayField);
			AddChild(EventController);
		}
		
		BarLine targetBarLine = PlayField.BarLines[PlayField.TargetIndex];
		_actionNames = new string[targetBarLine.Controllers.Length];
		for (int i = 0; i < _actionNames.Length; i++)
			_actionNames[i] = targetBarLine.Controllers[i].Action;
		
		BounceBeatSyncer = new BeatSyncer();
		BounceBeatSyncer.Name = "UI Bumper";
		BounceBeatSyncer.Value = 1f;
		BounceBeatSyncer.Bumped += Bounce;
		AddChild(BounceBeatSyncer);
		
		LoadGameScripts();

		/*
		float SPEED = 1f;
		PlayField.Music.PitchScale = SPEED;
		Vocals?.SetPitchScale(SPEED);
		Conductor.Speed = SPEED;*/

		PackedScene pauseMenu = PlayField.UiStyle.PauseMenu;
		if (pauseMenu != null && pauseMenu.CanInstantiate())
			AddChild(pauseMenu.Instantiate());
		
		// TODO: Countdown
		PlayField.Start();
		Vocals?.Play();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!Active)
			return;
		
		// TODO: Optimize this later
		Vector2 playFieldScale = PlayField.Scale;
		PlayField.Scale = playFieldScale.Lerp(Vector2.One, 3.125f * (float)delta);

		PlayField.PivotOffset = PlayField.Size / 2f;
	}

	/// <summary>
	/// Handle camera bouncing set by <see cref="BounceBeatSyncer"/>.
	/// </summary>
	public void Bounce()
	{
		PlayField.Scale += Vector2.One * 0.015f;
		Space.Bounce();
	}

	/// <summary>
	/// Gets called every time a note has been hit.
	/// Handles singing and missing animations for each environment.
	/// </summary>
	/// <param name="name">The name for the note type of the hit note.</param>
	/// <param name="result">The result of the hit note.</param>
	protected virtual void NoteHit(StringName name, NoteResult result)
	{
		if (result.Rating == Judgment.None)
			return;
		
		bool missed = result.Rating == Judgment.Miss;
		if (!result.Flags.HasFlag(NoteResultFlags.Animation))
		{
			RubiconCharacterGroup group = Space.GetCharacterGroup(name);	
			group.Sing(result.Direction, !missed && result.Hit == Hit.Hold, missed);
			bool charactersMissed = false;
			for (int i = 0; i < group.Controllers.Length; i++)
				if (group.Controllers[i].Missed)
					charactersMissed = true;

			missed = charactersMissed;
		}

		if (!result.Flags.HasFlag(NoteResultFlags.Vocals) && Vocals is not null)
			Vocals.VolumeLinear = missed ? 0f : 1f;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!Active || @event.IsEcho())
			return;

		// Freeze singing
		bool isAction = false;
		bool isHolding = false;
		for (int i = 0; i < _actionNames.Length; i++)
		{
			if (@event.IsAction(_actionNames[i]))
			{
				if (@event.IsPressed())
					isHolding = true;
				
				isAction = true;	
			}
			
			if (Input.IsActionPressed(_actionNames[i]))
				isHolding = true;
		}

		if (!isAction)
			return;
		
		Space.GetCharacterGroup(PlayField.TargetBarLine).SetFreezeSinging(isHolding);
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public void Pause()
	{
		Paused = true;
		
		PlayField.Pause();
		Vocals?.Stop();
		GetTree().Paused = true;
	}

	/// <summary>
	/// Resumes the game.
	/// </summary>
	public void Resume()
	{
		Paused = false;
		
		PlayField.Resume();
		Vocals?.Play(Conductor.AudioTime);
		GetTree().Paused = false;
	}

	/// <summary>
	/// Resets the song as well as all of its components.
	/// </summary>
	public void Reset()
	{
		Active = false;
			
		AudioPlayerGroup musicGroup = AudioManager.GetGroup("Music");
		musicGroup.Stop();
		musicGroup.DestroyPlayer(Instrumental);
		musicGroup.DestroyPlayer(Vocals);
		
		PlayField.QueueFree();
		BounceBeatSyncer.QueueFree();
		
		RuleSet = null;
		RootNode = null;
		
		Space.QueueFree();

		Metadata = null;
		Chart = null;
		Events = null;

		EventController.Reset();
	}

	private PlayField LoadPlayField(RuleSet ruleSet)
	{
		GodotObject ruleSetObject = ruleSet.PlayFieldScript.New().AsGodotObject();
		if (ruleSetObject is not PlayField playField)
		{
			PrintUtility.PrintError("RubiconGame", $"Ruleset at path \"{ruleSet.ResourcePath}\" does not contain a valid PlayField script.");
			return null;
		}

		return playField;
	}

	/// <summary>
	/// Loads each <see cref="GameEnvironment"/> node and initializes it.
	/// </summary>
	/// <param name="songMeta">The metadata of the song.</param>
	private void LoadSpace(SongMeta songMeta)
	{
		Space = new RubiconEnvironmentSpace();
		RootNode.AddChild(Space);
		Space.Initialize(songMeta, Context.AutoGenerate);
	}

	/// <summary>
	/// Loads each song script, as well as global/common scripts.
	/// </summary>
	private void LoadGameScripts()
	{
		ModulePathData[] globalModules = RubiconEngine.GlobalModules.Modules;
		for (int g = 0; g < globalModules.Length; g++)
		{
			ModulePathData module = globalModules[g];
			PackedScene moduleScene = ResourceLoader.Load<PackedScene>(module.Path);
			
			if (!moduleScene.CanInstantiate())
				continue;
			
			AddChild(moduleScene.Instantiate());
		}

		ModulePathData[] songModules = Metadata.Modules;
		for (int m = 0; m < songModules.Length; m++)
		{
			ModulePathData module = songModules[m];
			PackedScene moduleScene = ResourceLoader.Load<PackedScene>(module.Path);
			
			if (!moduleScene.CanInstantiate())
				continue;
			
			AddChild(moduleScene.Instantiate());
		}
	}
}
