using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GodotBeginnerSeries2024;

public partial class TubeAnimation : Node {
    private const string SCALE = "scale";
    private const string TRANSFORM = "transform";
    private const int CAPACITY = 10;

    [Export] private float _radius = 3;
    [Export] private int _sides = 8;

    [Export] private float _animTime = 0.5f;

    [ExportGroup("Resources")]
    [Export] private PackedScene _platformScene;

    private readonly Vector3 _initScale = Vector3.One * 0.001f;
    private Random _random = new();

    private int _transformIdx;
    private Transform3D[] _transforms;
    private Platform[] _platforms = new Platform[CAPACITY];

    public override async void _Ready() {
        _transforms = new Transform3D[_sides];
        var angle = Mathf.Tau / _sides;
        var theta = 0f;
        for (int i = 0; i < _sides; i++, theta += angle) {
            var origin = new Vector3(_radius * Mathf.Sin(theta), -_radius * Mathf.Cos(theta), 0);
            var basis = Basis.Identity.Rotated(Vector3.Forward, -theta);
            var transform = new Transform3D(basis, origin);
            _transforms[i] = transform;
        }

        await Task.Delay(TimeSpan.FromSeconds(1));
        await InstantiateFirstPlatformAsync();
        await Task.Delay(TimeSpan.FromSeconds(1));
        await InstantiateOtherPlatformsAsync();
        await Task.Delay(TimeSpan.FromSeconds(1));
        await InstantiateCircumferenceAsync();
        await Task.Delay(TimeSpan.FromSeconds(1));
        await MovePlatformsAsync();
    }

    private async Task InstantiateFirstPlatformAsync() {
        using var tween = CreateDefaultTween();
        var platform = _platformScene.Instantiate<Platform>();
        _platforms[0] = platform;
        platform.Scale = _initScale;
        platform.Position = new Vector3(0, -_radius, 0);
        AddChild(platform);
        tween.TweenProperty(platform, SCALE, Vector3.One, _animTime);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    private async Task InstantiateOtherPlatformsAsync() {
        using var tween = CreateDefaultTween();
        for (int i = 1; i < CAPACITY; i++) {
            var platform = _platformScene.Instantiate<Platform>();
            _platforms[i] = platform;
            platform.Scale = _initScale;
            platform.Position = new Vector3(0, -_radius, -i * Platform.LENGTH * 1.2f);
            AddChild(platform);
            tween.TweenProperty(platform, SCALE, Vector3.One, _animTime);
        }
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    private async Task InstantiateCircumferenceAsync() {
        var platforms = new List<Platform>(_sides - 1);

        using var tween1 = CreateDefaultTween();
        for (int i = 1; i < _sides; i++) {
            var platform = _platformScene.Instantiate<Platform>();
            platforms.Add(platform);

            var transform = _transforms[i];
            platform.Basis = transform.Basis;
            platform.Position = new Vector3(transform.Origin.X, transform.Origin.Y, _platforms[^1].Position.Z);
            platform.Scale = _initScale;
            AddChild(platform);
            tween1.TweenProperty(platform, SCALE, Vector3.One, _animTime);
        }
        await ToSignal(tween1, Tween.SignalName.Finished);
        await Task.Delay(TimeSpan.FromSeconds(1));

        using var tween2 = CreateDefaultTween();
        foreach (var platform in platforms) {
            tween2.TweenProperty(platform, SCALE, _initScale, _animTime);
        }
        await ToSignal(tween2, Tween.SignalName.Finished);
        foreach (var platform in platforms) {
            platform.QueueFree();
        }
    }

    private async Task MovePlatformsAsync() {
        using var tween = CreateDefaultTween();
        for (int i = 1; i < CAPACITY; i++) {
            UpdateTransformIndex();
            var transform = _transforms[_transformIdx];
            var platform = _platforms[i];
            transform.Origin = new Vector3(transform.Origin.X, transform.Origin.Y, platform.Position.Z);
            tween.TweenProperty(platform, TRANSFORM, transform, _animTime);
        }
        await ToSignal(tween, Tween.SignalName.Finished);

        void UpdateTransformIndex() {
            _transformIdx += _random.NextDouble() > 0.5 ? 1 : -1;
            if (_transformIdx < 0) {
                _transformIdx += _sides;
            } else if (_transformIdx >= _sides) {
                _transformIdx -= _sides;
            }
        }
    }

    private Tween CreateDefaultTween() {
        return CreateTween()
            .SetParallel()
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);
    }
}
