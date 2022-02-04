using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace NewStockIndicator.Common.Configs;

[Label("Mod Options")]
public class ModOptions : ModConfig {
	public static ModOptions Instance { get; private set; }

	public override ConfigScope Mode => ConfigScope.ClientSide;

	[Label("Highlight New Items")]
	[DefaultValue(true)]
	public bool HighlightNewItems;

	[Label("Stock Poll Rate")]
	[Tooltip("How often (in seconds) the game will check to see if an NPC has new items in stock")]
	[DefaultValue(5f)]
	[Range(1f, 30f)]
	[Increment(1f)]
	public float StockPollRate;

	public override void OnLoaded() {
		Instance = this;
	}
}
