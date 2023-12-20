using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Godot;
using Godot.Collections;
using ProceduralPlanet.scripts.effects;
using static Godot.Extensions.PropertyHelper;

namespace ProceduralPlanet.scripts.planet.monitors;

[Tool]
public partial class AtmosphereEntryExitMonitor : Node3D
{
	private readonly List<Area3D> _previousIntersectingAreas = new();

	private Area3D? _atmosphereInner;
	private Area3D? _atmosphereOuter;

	private CelestialBodyGenerator? _planet;
	private NodePath? _planetPath;

	private CelestialBodyGenerator? Planet
	{
		get
		{
			if (_planet == default  && _planetPath is {} planetPath)
			{
				_planet ??= GetNode<CelestialBodyGenerator>(planetPath);
			}

			return _planet;
		}
	}

	[Export(PropertyHint.NodePathValidTypes, "CelestialBodyGenerator")]
	public NodePath? PlanetPath
	{
		get => _planetPath;
		set
		{
			if (!SetIfChanged(ref _planetPath, value))
			{
				return;
			}

			(var planet, _planet) = (_planet, default);
			planet?.Dispose();
		}
	}

	[Export] public float ShakeStrength { get; set; } = 20;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (Planet is not { Body.Shading.AtmosphereSettings.AtmosphereScale: var atmosphereScale } planet)
		{
			return;
		}

		var attachToInner = _atmosphereInner == default;
		_atmosphereInner ??= FindChild("AtmosphereInner") as Area3D;
		if (_atmosphereInner is not {} atmosphereInner)
		{
			return;
		}

		if (attachToInner)
		{
			atmosphereInner.TreeExiting += () =>
			{
				_atmosphereInner = default;
			};
		}

		var attachToOuter = _atmosphereOuter == default;
		_atmosphereOuter ??= FindChild("AtmosphereOuter") as Area3D;
		if (_atmosphereOuter is not { } atmosphereOuter)
		{
			return;
		}

		if (attachToOuter)
		{
			atmosphereOuter.TreeExiting += () =>
			{
				_atmosphereInner = default;
			};
		}

		var atmosphereRadius = (1f + atmosphereScale) * planet.BodyScale;

		var innerCollider = atmosphereInner.GetChild<CollisionShape3D>(0);
		if (innerCollider.Shape is SphereShape3D innerColliderShape)
		{
			innerColliderShape.Radius = planet.BodyScale + 0.3f * (atmosphereRadius - planet.BodyScale);
		}
		else
		{
			Debug.WriteLine("Invalid inner atmosphere collider shape");
			return;
		}

		var outerCollider = atmosphereOuter.GetChild<CollisionShape3D>(0);
		if (outerCollider.Shape is SphereShape3D outerColliderShape)
		{
			outerColliderShape.Radius = planet.BodyScale + 1.3f * (atmosphereRadius - planet.BodyScale);
		}
		else
		{
			Debug.WriteLine("Invalid outer atmosphere collider shape");
			return;
		}

		var overlappingAreasInner = atmosphereInner.GetOverlappingAreas();
		var overlappingAreasOuter = atmosphereOuter.GetOverlappingAreas();

		if (overlappingAreasInner.Count < 1 && overlappingAreasOuter.Count < 1 && _previousIntersectingAreas.Count < 1)
		{
			return;
		}

		List<Area3D> intersectingAreas = new();

		foreach (var overlappingAreaOuter in overlappingAreasOuter)
		{
			if (overlappingAreasInner.Contains(overlappingAreaOuter))
			{
				continue;
			}

			intersectingAreas.Add(overlappingAreaOuter);

			if (TryGetShakeEffect(overlappingAreaOuter, out var shakeEffect))
			{
				shakeEffect.IsShaking = true;
			}

			if (TryGetAudioStreamPlayer(overlappingAreaOuter, out var audioStreamPlayer3D) && !audioStreamPlayer3D.Playing)
			{
				audioStreamPlayer3D.Playing = true;
			}
		}

		foreach (var previousIntersectingArea in _previousIntersectingAreas)
		{
			if (!intersectingAreas.Contains(previousIntersectingArea))
			{
				if (TryGetShakeEffect(previousIntersectingArea, out var shakeEffect))
				{
					shakeEffect.IsShaking = false;
				}

				if (TryGetAudioStreamPlayer(previousIntersectingArea, out var audioStreamPlayer3D) && audioStreamPlayer3D.Playing)
				{
					audioStreamPlayer3D.Playing = false;
				}
			}
			else
			{
				var distanceFromIntersectingArea = previousIntersectingArea.GlobalPosition.DistanceTo(GlobalPosition);

				var innerRadius = innerColliderShape.Radius;
				var innerDistance = Math.Abs(distanceFromIntersectingArea - innerRadius);

				var outerRadius = outerColliderShape.Radius;
				var outerDistance = Math.Abs(distanceFromIntersectingArea - outerRadius);

				var atmosphereThickness = outerRadius - innerRadius;
				var atmosphereRatio = Math.Abs(outerDistance - innerDistance) / atmosphereThickness;

				if (TryGetShakeEffect(previousIntersectingArea, out var shakeEffect))
				{
					shakeEffect.Speed = ShakeStrength * (1f - atmosphereRatio);
				}

				if (TryGetAudioStreamPlayer(previousIntersectingArea, out var audioStreamPlayer3D))
				{
					audioStreamPlayer3D.MaxDb = -24 + 24 * Math.Min(1, MathF.Exp(-atmosphereRatio));
					audioStreamPlayer3D.VolumeDb = -80 + 80 * Math.Min(1, MathF.Exp(-atmosphereRatio));
				}
			}
		}

		_previousIntersectingAreas.Clear();
		_previousIntersectingAreas.AddRange(intersectingAreas);
	}

	private bool TryGetAudioStreamPlayer(Node node, [NotNullWhen(true)] out AudioStreamPlayer3D? audioStreamPlayer3D)
	{
		foreach (var child in node.GetParent().GetChildren())
		{
			if (!child.IsInGroup("AtmosphereEffect"))
			{
				continue;
			}

			audioStreamPlayer3D = child as AudioStreamPlayer3D;
			if (audioStreamPlayer3D != default)
			{
				return true;
			}
		}

		audioStreamPlayer3D = default;
		return false;
	}

	private bool TryGetShakeEffect(Node node, [NotNullWhen(true)] out ShakeEffect? shakeEffect)
	{
		foreach (var child in node.GetParent().GetChildren())
		{
			shakeEffect = child as ShakeEffect;
			if (shakeEffect != default)
			{
				return true;
			}
		}

		shakeEffect = default;
		return false;
	}
}
