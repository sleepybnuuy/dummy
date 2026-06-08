using Dalamud.Configuration;
using System;

namespace Dummy;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    // The below exists just to make saving less cumbersome
    public void Save() {
        Dummy.PluginInterface.SavePluginConfig(this);
    }
}
