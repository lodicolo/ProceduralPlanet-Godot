using Godot;
using System;
using System.Diagnostics;

public partial class Main : Node2D
{
	private FreeLookCamera3D? _freeLookCamera3D;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ChildEnteredTree += OnChildEnteredTree;
		ChildExitingTree += OnChildExitingTree;

		var node = FindChild("Camera3D");
		if (node is FreeLookCamera3D freeLookCamera3D)
		{
			_freeLookCamera3D = freeLookCamera3D;
			_freeLookCamera3D.TreeExiting += () =>
			{
				if (_freeLookCamera3D == freeLookCamera3D)
				{
					_freeLookCamera3D = default;
				}
			};
		}
	}

	private void OnChildExitingTree(Node node)
	{
		Console.WriteLine($"Old {node.GetType().Name} \"{node.Name}\" exiting tree...");
	}

	private void OnChildEnteredTree(Node node)
	{
		Console.WriteLine($"New {node.GetType().Name} \"{node.Name}\" entered tree...");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _Input(InputEvent inputEvent)
	{
		// Console.WriteLine($"{Name} [{GetType().Name}] {nameof(_Input)}: {inputEvent.GetType().Name}");
		_freeLookCamera3D?._Input(inputEvent);
	}
}
