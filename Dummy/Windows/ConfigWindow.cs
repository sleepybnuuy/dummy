using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;

namespace Dummy.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    public ConfigWindow(Dummy plugin) : base("Dummy Config")
    {
        this.Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        this.Size = new Vector2(232, 90);
        this.SizeCondition = ImGuiCond.Always;

        this.configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
		ImGui.TextWrapped("Nothing to configure yet! Visit the Kugane Aetheryte to see the dummy!");
    }
}
