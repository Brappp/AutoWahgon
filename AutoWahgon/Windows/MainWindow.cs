using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui;
using System.Threading.Tasks;
using System.Text;

namespace AutoWahgon.Windows
{
    public class MainWindow : Window, IDisposable
    {
        private readonly Plugin Plugin;
        private readonly ICondition Condition;
        private readonly IClientState ClientState;
        private readonly IGameGui GameGui;
        private bool isAutomating = false;
        private bool hasAutomated = false;
        private bool ffxivAttached = false;
        private StringBuilder keyPressLog;
        private bool hasLogged6 = false;
        private bool hasLogged0First = false;
        private bool hasLogged0Second = false;
        private bool hasLogged0Third = false;
        private bool isUserClosed = false;

        public MainWindow(Plugin plugin, ICondition condition, IClientState clientState, IGameGui gameGui)
            : base("Auto Wahgon", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
        {
            SizeConstraints = new WindowSizeConstraints
            {
                MinimumSize = new Vector2(400, 400),
                MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
            };

            Plugin = plugin;
            Condition = condition;
            ClientState = clientState;
            GameGui = gameGui;

            keyPressLog = new StringBuilder();
        }

        public void Dispose() { }

        public void CheckGameState()
        {
            if (!isUserClosed && !IsOpen)
            {
                IsOpen = true;
            }

            if (!hasAutomated)
            {
                _ = MonitorTitleScreenAsync();
            }
        }

        public override void Draw()
        {
            if (ffxivAttached)
            {
                ImGui.TextColored(new Vector4(0, 1, 0, 1), "FFXIV is attached.");
            }
            else
            {
                ImGui.TextColored(new Vector4(1, 0, 0, 1), "FFXIV is not attached.");
            }

            ImGui.Separator();

            ImGui.Text("Client State Detection:");
            if (ClientState.IsLoggedIn)
            {
                ImGui.Text("Player is logged in.");
            }
            else
            {
                ImGui.Text("Player is not logged in.");
            }

            ImGui.Separator();
            ImGui.Text("UI Screen Detection:");

            IntPtr titleAddon = GameGui.GetAddonByName("Title", 1);
            if (titleAddon == IntPtr.Zero)
            {
                ImGui.Text("Title screen is not visible.");
            }
            else
            {
                ImGui.Text("Title screen is visible (Press Start).");

                if (isAutomating)
                {
                    ImGui.Text("Automating key presses...");
                }
            }

            ImGui.Separator();
            ImGui.Text("Keys Pressed:");
            ImGui.Text(keyPressLog.ToString());

            ImGui.Separator();

            if (ImGui.Button("Close Window"))
            {
                isUserClosed = true;
                IsOpen = false;
            }
        }

        private async Task MonitorTitleScreenAsync()
        {
            while (!hasAutomated)
            {
                ffxivAttached = true;

                if (ffxivAttached && GameGui.GetAddonByName("Title", 1) != IntPtr.Zero)
                {
                    isAutomating = true;
                    try
                    {
                        await Task.Delay(2000);

                        if (!hasLogged6)
                        {
                            keyPressLog.AppendLine("Numpad6");
                            hasLogged6 = true;
                        }
                        await Task.Run(() => User32Helper.SendNumpad6ToFfxiv());
                        await Task.Delay(3000);

                        if (!hasLogged0First)
                        {
                            keyPressLog.AppendLine("Numpad0");
                            hasLogged0First = true;
                        }
                        await Task.Run(() => User32Helper.SendNumpad0ToFfxiv());

                        while (GameGui.GetAddonByName("Title", 1) != IntPtr.Zero)
                        {
                            await Task.Delay(1000);
                        }

                        if (!hasLogged0Second)
                        {
                            keyPressLog.AppendLine("Numpad0");
                            hasLogged0Second = true;
                        }
                        await Task.Run(() => User32Helper.SendNumpad0ToFfxiv());
                        await Task.Delay(3000);

                        if (!hasLogged0Third)
                        {
                            keyPressLog.AppendLine("Numpad0");
                            hasLogged0Third = true;
                        }
                        await Task.Run(() => User32Helper.SendNumpad0ToFfxiv());

                        hasAutomated = true;
                    }
                    finally
                    {
                        isAutomating = false;
                    }
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }
    }
}
