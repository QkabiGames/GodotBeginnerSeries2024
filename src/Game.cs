using Godot;
using System;

namespace GodotBeginnerSeries2024;

public partial class Game : Node {
    [Export] private float _initGravity = 2f;
    [Export] private float _gravityStep = 0.1f;

    [ExportGroup("Components")]
    [Export] private Tube _tube;
    [Export] private Player _player;
    [Export] private Area3D _gameOverArea;

    [Export] private UI _ui;
    [Export] private SaveManager _saveManager;

    private int _score;
    private int _highScore;

    public event Action<int> ScoreUpdated = delegate { };
    public event Action GameOver = delegate { };

    public override void _Ready() {
        _ui.RestartRequested += Restart;

        _player.Bounced += () => {
            _ui.SetScore(++_score);
            _tube.Traverse();
            _player.GravityScale += _gravityStep;
        };

        _highScore = _saveManager.LoadHighScore();

        _gameOverArea.Position = new Vector3(0, -2 * _tube.Radius, 0);
        _gameOverArea.BodyEntered += _ => {
            _player.Freeze = true;

            _highScore = Mathf.Max(_highScore, _score);
            _saveManager.SaveHighScore(_highScore);
            _ui.GameOver(_highScore);

            SetProcessUnhandledInput(false);
        };

        Restart();
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (@event is not InputEventKey key || !key.IsPressed()) {
            return;
        }

        if (key.Keycode == Key.Right) {
            _tube.Spin(false);
        } else if (key.Keycode == Key.Left) {
            _tube.Spin(true);
        }
    }

    private void Restart() {
        _player.Position = Vector3.Zero;
        _player.GravityScale = _initGravity;
        _player.Freeze = false;

        _score = 0;
        _ui.SetScore(0);

        _tube.Reset();
        SetProcessUnhandledInput(true);
    }
}
