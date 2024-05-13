using Godot;
using System;

namespace GodotBeginnerSeries2024;

public partial class SaveManager : Node {
    private const string PATH = "user://save.cfg";
    private const string SECTION = "Save";
    private const string KEY = "HighScore";

    private ConfigFile _config;

    public override void _Ready() {
        _config = new ConfigFile();
        if (_config.Load(PATH) != Error.Ok) {
            SaveHighScore(0);
            _config.Save(PATH);
        }
    }

    public int LoadHighScore() {
        return _config.GetValue(SECTION, KEY).AsInt32();
    }

    public void SaveHighScore(int highScore) {
        _config.SetValue(SECTION, KEY, highScore);
        _config.Save(PATH);
    }
}
