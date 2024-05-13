using Godot;
using System;

namespace GodotBeginnerSeries2024;

public partial class Player : RigidBody3D {
    private const string SCALE_Y = "scale:y";

    [Export] private float _strength = 8;

    [Export] private float _bounceScale = 0.5f;
    [Export] private float _bounceTime = 0.5f;

    [ExportGroup("Components")]
    [Export] private MeshInstance3D _mesh;

    private Vector3 _impulse;

    public event Action Bounced = delegate { };

    public override void _Ready() {
        _impulse = Vector3.Up * _strength;
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node _) {
        LinearVelocity = Vector3.Zero;
        ApplyCentralImpulse(_impulse);

        _mesh.Scale = new Vector3(_mesh.Scale.X, _mesh.Scale.Y * _bounceScale, _mesh.Scale.Z);
        CreateTween()
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out)
            .TweenProperty(_mesh, SCALE_Y, 1, _bounceTime);

        Bounced();
    }
}
