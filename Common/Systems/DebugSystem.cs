using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NewStockIndicator.Common.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NewStockIndicator.Common.Systems;

public class DebugSystem : ModSystem {
	public override void PostUpdateEverything() {
		if (Main.hasFocus && Main.netMode == NetmodeID.SinglePlayer && Main.keyState.IsKeyDown(Keys.LeftShift) && Main.keyState.IsKeyDown(Keys.F3) && Main.oldKeyState.IsKeyUp(Keys.F3) && Main.LocalPlayer.name == "Testing Player") {
			var player = Main.LocalPlayer.GetModPlayer<PlayerRecordManager>();
			player.StockRecords.Clear();
			player.UpdateRecordsForAllActiveNPCs();

			Main.NewText("Reset player stock records", Color.Beige);
		}
	}
}
