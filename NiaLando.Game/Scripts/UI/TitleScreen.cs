using Godot;
using NiaLando.Application;

namespace NiaLando.Game.UI;

public partial class TitleScreen : Control
{
    private Label? _subtitleLabel;
    private GameSession? _session;

    public override void _Ready()
    {
        _subtitleLabel = GetNode<Label>("VBoxContainer/SubtitleLabel");
        _session = new GameSession();
        _session.NewGame();
        
        if (_subtitleLabel != null)
        {
            _subtitleLabel.Text = _session.Title;
        }
    }
}
