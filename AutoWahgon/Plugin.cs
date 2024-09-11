using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using AutoWahgon.Windows;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui;
using System.Timers;

namespace AutoWahgon;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICondition Condition { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IGameGui GameGui { get; private set; } = null!;

    private Timer gameStateTimer;
    public readonly WindowSystem WindowSystem = new("AutoWahgon");
    private MainWindow MainWindow { get; init; }

    public Plugin()
    {
        MainWindow = new MainWindow(this, Condition, ClientState, GameGui);
        WindowSystem.AddWindow(MainWindow);

        PluginInterface.UiBuilder.Draw += DrawUI;
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
        PluginInterface.UiBuilder.OpenConfigUi += ToggleMainUI;  // Register Config UI

        MainWindow.CheckGameState();

        gameStateTimer = new Timer(2000);
        gameStateTimer.Elapsed += OnTimedEvent;
        gameStateTimer.AutoReset = true;
        gameStateTimer.Enabled = true;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        MainWindow.Dispose();

        gameStateTimer.Stop();
        gameStateTimer.Dispose();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleMainUI() => MainWindow.IsOpen = !MainWindow.IsOpen;

    private void OnTimedEvent(object sender, ElapsedEventArgs e)
    {
        MainWindow.CheckGameState();
    }
}
