using NewStockIndicator.Common.Configs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace NewStockIndicator.Common.Players;

public class PlayerRecordManager : ModPlayer {
	private const int LatestDataVersion = 0;

	public StockRecordList StockRecords;
	private int _ticksUntilRefresh;

	public override void Load() {
		StockRecords = new StockRecordList();
	}

	public override void PostUpdate() {
		if (Main.myPlayer != Player.whoAmI)
			return;

		if (_ticksUntilRefresh > 0)
			_ticksUntilRefresh--;

		if (_ticksUntilRefresh == 0) {
			// Only refresh if we aren't in a shop
			if (Main.npcShop == 0)
				UpdateRecordsForAllActiveNPCs();

			_ticksUntilRefresh = (int) (ModOptions.Instance.StockPollRate * 60f);
		}
	}

	public override void SaveData(TagCompound tag) {
		tag.Set("Version", LatestDataVersion);
		tag.Set("StockRecords", StockRecords.Serialize());
	}

	public override void LoadData(TagCompound tag) {
		if (tag.ContainsKey("StockRecords"))
			StockRecords.Deserialize(tag.GetCompound("StockRecords"));
	}

	public void UpdateRecordsForAllActiveNPCs() {
		foreach (var npc in Main.npc)
			if (npc != null && npc.active && Helpers.IsNPC(npc))
				UpdateRecord(npc);
	}

	public void UpdateRecord(NPC npc) {
		if (StockRecords.TryGet(npc, out var record))
			record.UpdateCurrent(npc);
	}

	public static void OnShopOpened(NPC npc) {
		if (Main.LocalPlayer.GetModPlayer<PlayerRecordManager>().StockRecords.TryGet(npc, out var stock)) {
			if (ModOptions.Instance.HighlightNewItems) {
				foreach (var item in Main.instance.shop[Main.npcShop].item) {
					if (item != null && !item.IsAir && !stock.SeenItems.Contains(item.type)) {
						item.newAndShiny = true;
					}
				}
			}

			stock.UpdateCurrent(npc);
			stock.PushCurrentToSeen();
		}
	}
}
