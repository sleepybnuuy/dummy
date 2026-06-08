using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.Numerics;

using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dummy.Windows;

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

namespace Dummy;

public sealed class Dummy : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
	[PluginService] internal static IClientState ClientState { get; private set; } = null!;

    private const string CommandName = "/dummy";

	private readonly string _modelPath = "bgcommon/world/lvd/017/bgparts/w_lvd_017_01a.mdl"; // Dummy Mdl
	private readonly uint _targetMap = 370; // Kugane Map
	private readonly uint _targetTerritory = 628; // Kugane Territory
	private readonly Vector3 _dummyPos = new (43.276f, 4.6f, -48.855f); // Aetheryte Position
	private unsafe BgObject* _dummyPtr;

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("Dummy");
    private ConfigWindow ConfigWindow { get; init; }

    public Dummy() {
        this.Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        this.ConfigWindow = new ConfigWindow(this);

        this.WindowSystem.AddWindow(this.ConfigWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand) {
            HelpMessage = "opens Dummy settings"
        });

        // Tell the UI system that we want our windows to be drawn through the window system
        PluginInterface.UiBuilder.Draw += this.WindowSystem.Draw;

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += this.ToggleConfigUi;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += this.ToggleConfigUi;

		ClientState.TerritoryChanged += this.MapChanged;
		if (ClientState.MapId == this._targetMap)
			this.BuildDummy();
	}

    public void Dispose() {
        // Unregister all actions to not leak anything during disposal of plugin
        PluginInterface.UiBuilder.Draw -= this.WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= this.ToggleConfigUi;
		PluginInterface.UiBuilder.OpenMainUi -= this.ToggleConfigUi;
		ClientState.TerritoryChanged -= this.MapChanged;

        this.WindowSystem.RemoveAllWindows();

        this.ConfigWindow.Dispose();
		this.DisposeDummy();

        CommandManager.RemoveHandler(CommandName);
    }

	private void MapChanged(uint territoryId) {
		if (territoryId == this._targetTerritory)
			this.BuildDummy();
		else
			this.DisposeDummy();
	}

	private unsafe void BuildDummy() {
		if (this._dummyPtr is not null)
			return;

		this._dummyPtr = BgObject.Create(this._modelPath, "bgcommon", null);
		this._dummyPtr->Position = this._dummyPos;
		Framework.RunOnTick(() => {
			this._dummyPtr->UpdateCulling();
			this._dummyPtr->UpdateRender();
		}, delayTicks:1);
	}

	private unsafe void DisposeDummy() {
		if (this._dummyPtr is null)
			return;

		this._dummyPtr->CleanupRender();
		this._dummyPtr->Dtor(1);
		this._dummyPtr = null;
	}

	private void OnCommand(string command, string args) => this.ToggleConfigUi();
    private void ToggleConfigUi() => this.ConfigWindow.Toggle();
}
