using NewStockIndicator.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace NewStockIndicator.Common;

public class GlobalNPCListeners : GlobalNPC {
	public override void Load() {
		// Instantly update CurrentItems when an NPC spawns
		On.Terraria.NPC.NewNPC += (orig, x, y, type, start, ai0, ai1, ai2, ai3, target) => {
			var index = orig(x, y, type, start, ai0, ai1, ai2, ai3, target);
			var npc = Main.npc[index];

			if (Helpers.IsNPC(npc))
				Main.LocalPlayer.GetModPlayer<PlayerRecordManager>().UpdateRecord(npc);

			return index;
		};

		// For vanilla shops
		On.Terraria.Main.OpenShop += (orig, self, index) => {
			orig(self, index);

			if (index > 0)
				PlayerRecordManager.OnShopOpened(Main.npc[Main.LocalPlayer.talkNPC]);
		};
	}

	public override void OnChatButtonClicked(NPC npc, bool firstButton) {
		// For modded shops
		if (Main.npcShop > 0)
			PlayerRecordManager.OnShopOpened(npc);
	}
}
