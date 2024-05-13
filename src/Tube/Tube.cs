using Godot;
using System;

namespace GodotBeginnerSeries2024;

public partial class Tube : Node3D {
    private const string Z_POS = "position:z";

    [Export] private float _radius = 3;
    [Export] private int _sides = 8;
    [Export] private int _capacity = 100;

    [Export] private float _spinTime = 0.1f;
    [Export] private float _traverseTime = 0.2f;

    [ExportGroup("Resources")]
    [Export] private PackedScene _platformScene;

    private readonly Random _random = new();

    private Platform[] _platforms;
    private int _platformIdx;

    private Transform3D[] _transforms;
    private int _transformIdx;

    private bool _isSpinning;
    private float _angle;

    public float Radius => _radius;

    public override void _Ready() {
        SetupTransforms();
        SetupPlatforms();

        void SetupTransforms() {
            _transforms = new Transform3D[_sides];
            _angle = Mathf.Tau / _sides;

            var theta = 0f;
            for (int i = 0; i < _sides; i++, theta += _angle) {
                var origin = new Vector3(_radius * Mathf.Sin(theta), -_radius * Mathf.Cos(theta), 0);
                var basis = Basis.Identity.Rotated(Vector3.Forward, -theta);
                var transform = new Transform3D(basis, origin);
                _transforms[i] = transform;
            }
        }

        void SetupPlatforms() {
            _platforms = new Platform[_capacity];

            var platform = _platformScene.Instantiate<Platform>();
            _platforms[0] = platform;
            platform.Position = new Vector3(0, -_radius, 0);
            AddChild(platform);

            for (int i = 1; i < _capacity; i++) {
                platform = _platformScene.Instantiate<Platform>();
                _platforms[i] = platform;

                UpdateTransformIndex();
                var transform = _transforms[_transformIdx];
                platform.Basis = transform.Basis;
                platform.Position = new Vector3(transform.Origin.X, transform.Origin.Y, -(i * Platform.LENGTH));

                AddChild(platform);
            }
        }
    }

    public void Reset() {
        var firstPlatform = _platforms[_platformIdx];

        var zDiff = firstPlatform.GlobalPosition.Z - GlobalPosition.Z;
        GlobalPosition = Vector3.Zero;
        foreach (var child in GetChildren()) {
            if (child is Platform platform) {
                platform.GlobalPosition = new Vector3(platform.GlobalPosition.X, platform.GlobalPosition.Y, platform.GlobalPosition.Z - zDiff);
            }
        }

        var rotDiff = firstPlatform.GlobalRotation.Z;
        GlobalRotation = new Vector3(0, 0, GlobalRotation.Z - rotDiff);
    }

    public void Spin(bool right) {
        if (_isSpinning) {
            return;
        }

        _isSpinning = true;
        var tween = CreateDefaultTween();
        tween.TweenProperty(this,
                            Node3D.PropertyName.Basis.ToString(),
                            Basis.Rotated(Vector3.Forward, right ? -_angle : _angle),
                            _spinTime);
        tween.TweenCallback(Callable.From(() => _isSpinning = false));
    }

    public void Traverse() {
        var tween = CreateDefaultTween();
        tween.TweenProperty(this,
                            Z_POS,
                            Position.Z + Platform.LENGTH,
                            _traverseTime);
        tween.TweenCallback(Callable.From(Wrap));

        void Wrap() {
            var lastIdx = _platformIdx > 0 ? _platformIdx - 1 : _capacity - 1;
            var last = _platforms[lastIdx];
            var first = _platforms[_platformIdx];

            UpdateTransformIndex();
            var transform = _transforms[_transformIdx];
            first.Basis = transform.Basis;
            first.Position = new Vector3(transform.Origin.X, transform.Origin.Y, last.Position.Z - Platform.LENGTH);

            if (++_platformIdx == _capacity) {
                _platformIdx = 0;
            }
        }
    }

    private void UpdateTransformIndex() {
        _transformIdx += _random.NextDouble() > 0.5 ? 1 : -1;
        if (_transformIdx < 0) {
            _transformIdx += _sides;
        } else if (_transformIdx >= _sides) {
            _transformIdx -= _sides;
        }
    }

    private Tween CreateDefaultTween() {
        return CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.InOut);
    }
}
