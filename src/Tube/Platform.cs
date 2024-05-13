using Godot;
using System;

namespace GodotBeginnerSeries2024;

public partial class Platform : StaticBody3D {
    public const int LENGTH = 2;

    private const int CLIPS_COUNT = 14;

    private static readonly Random RANDOM = new();

    [ExportGroup("Components")]
    [Export] private Area3D _area;
    [Export] private AudioStreamPlayer _player;

    public override void _Ready() {
        SetStream();

        _area.BodyEntered += _ => _player.Play();
        _player.Finished += SetStream;
    }

    private void SetStream() {
        var index = RANDOM.Next(CLIPS_COUNT) + 1;
        var path = $"res://src/Tube/Resources/key{index.ToString().PadLeft(2, '0')}.mp3";
        var stream = GD.Load<AudioStream>(path);
        _player.Stream = stream;
    }
}
