using Godot;
using System;
using System.Threading.Tasks;

namespace GodotBeginnerSeries2024;

public partial class TubeTest : Node {
    [Export] private Tube _tube;

    public async override void _Ready() {
        var rand = new Random();

        while (true) {
            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            _tube.Traverse();
            _tube.Spin(rand.NextDouble() > 0.5);
        }
    }
}
