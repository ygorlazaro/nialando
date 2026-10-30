using Godot;

namespace NiaLando.Game.Root;

public partial class GameRoot : Node
{
    public override void _Ready()
    {
        GD.Print("NiaLando.Game iniciado");
    }
}
