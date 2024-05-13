using Godot;
using System;

namespace GodotBeginnerSeries2024;

public partial class UI : Control {
    private const string POS_X = "position:x";

    [Export] private float _tweenTime = 0.5f;

    [ExportGroup("Components")]
    [Export] private Label _scoreLabel;

    [Export] private Panel _gameOverPanel;
    [Export] private Label _highScoreLabel;
    [Export] private Button _restartButton;

    public event Action RestartRequested = delegate { };

    public override void _Ready() {
        _restartButton.Pressed += () => {
            CreateDefaultTween().TweenProperty(_gameOverPanel, POS_X, DisplayServer.WindowGetSize().X, _tweenTime);
            RestartRequested();
        };
    }

    public void SetScore(int score) {
        _scoreLabel.Text = score.ToString();
    }

    public void GameOver(int highScore) {
        _highScoreLabel.Text = $"Score: {_scoreLabel.Text}\nHigh Score: {highScore}";
        CreateDefaultTween().TweenProperty(_gameOverPanel, POS_X, 0, _tweenTime);
    }

    private Tween CreateDefaultTween() {
        return CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
    }
}
