using Godot;
using NiaLando.Application;
using NiaLando.Domain;

namespace NiaLando.Game.UI;

public partial class GameUI : Control
{
    private GameSession? _session;
    private Label? _turnLabel;
    private Button? _endTurnButton;
    private Label? _messageLabel;
    private Label? _provinceLabel;

    public override void _Ready()
    {
        _turnLabel = GetNode<Label>("TopBar/TurnLabel");
        _endTurnButton = GetNode<Button>("TopBar/EndTurnButton");
        _messageLabel = GetNode<Label>("MessagePanel/MessageLabel");
        _provinceLabel = GetNode<Label>("ProvincePanel/ProvinceLabel");
        
        _session = new GameSession();
        _session.NewGame();
        
        _endTurnButton!.Pressed += OnEndTurnPressed;
        UpdateUI();
    }

    private void OnEndTurnPressed()
    {
        if (_session == null) return;
        var result = _session.EndTurn();
        UpdateUI();
        if (_messageLabel != null && result.NewMessages.Count > 0)
        {
            _messageLabel.Text = $"[{result.NewMessages[0].Category}] {result.NewMessages[0].Key}";
        }
    }

    private void UpdateUI()
    {
        if (_session == null || _turnLabel == null) return;
        _turnLabel.Text = $"Turno: {_session.State.CurrentTurn} | Ano: {_session.State.CurrentYear}";
    }

    public void ShowProvince(Province? province)
    {
        if (_provinceLabel == null) return;
        if (province == null)
            _provinceLabel.Text = "Província: -";
        else
            _provinceLabel.Text = $"Província: {province.Name}\nTerreno: {province.Terrain}\nDesenvolvimento: {province.Development}\nDono: {(province.OwnerId?.Value.ToString() ?? "-")}";
    }
}
