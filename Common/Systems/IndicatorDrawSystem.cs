using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NewStockIndicator.Common.Players;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace NewStockIndicator.Common.Systems;

public class IndicatorDrawSystem : ModSystem {
	private static Texture2D IndicatorTexture => TextureAssets.QuicksIcon.Value;

	private const float IconOffset = 8f;
	private const float IconBobSpeed = 4f;
	private const float IconBobRange = 1.5f;

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
		var emoteBubbleIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Emote Bubbles");

		if (emoteBubbleIndex != -1) {
			layers.Insert(emoteBubbleIndex + 1, new LegacyGameInterfaceLayer("NewStockIndicator: Indicator Icons", () => {
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Main.GameViewMatrix.ZoomMatrix);

				foreach (var npc in Main.npc)
					if (npc != null && npc.active)
						if (ShouldDrawIndicator(npc))
							DrawIndicator(npc, Main.spriteBatch);

				return true;
			}, InterfaceScaleType.None));
		}
	}

	private static void DrawIndicator(NPC npc, SpriteBatch spriteBatch) {
		var texture = IndicatorTexture;
		var bobOffset = (float) Math.Sin(Main.GlobalTimeWrappedHourly * IconBobSpeed) * IconBobRange;

		var position = new Vector2(
			npc.Center.X - (texture.Width / 2) - Main.screenPosition.X,
			npc.Bottom.Y - npc.frame.Height - IconOffset + bobOffset - Main.screenPosition.Y
		);

		spriteBatch.Draw(texture, position, texture.Bounds, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
	}

	private static bool ShouldDrawIndicator(NPC npc) {
		if (!Helpers.IsNPC(npc))
			return false;

		if (npc.type == NPCID.Angler && !Main.anglerWhoFinishedToday.Contains(Main.LocalPlayer.name))
			return true;

		var player = Main.LocalPlayer.GetModPlayer<PlayerRecordManager>();
		if (player.StockRecords.TryGet(npc, out var record))
			return record.HasUnseenItems;

		return false;
	}
}
