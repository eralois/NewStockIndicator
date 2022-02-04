using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace NewStockIndicator.Common.Players;

public class StockRecord {
	public HashSet<int> CurrentItems = new();
	public HashSet<int> SeenItems = new();

	public bool HasUnseenItems { get; private set; }

	/// <summary>
	/// Clears CurrentItems and refills it with the IDs of every item currently in the NPC's shop
	/// </summary>
	/// <param name="npc"></param>
	public void UpdateCurrent(NPC npc) {
		CurrentItems.Clear();

		var chest = new Chest();
		Helpers.FillWithShopItems(chest, npc);

		for (int i = 0; i < chest.item.Length; i++) {
			var item = chest.item[i];

			if (item != null && !item.IsAir)
				CurrentItems.Add(item.type);
		}

		UpdateUnseenItems();
	}

	public void PushCurrentToSeen() {
		SeenItems.UnionWith(CurrentItems);
		UpdateUnseenItems();
	}

	private void UpdateUnseenItems() {
		HasUnseenItems = CurrentItems.Count > 0 && CurrentItems.Any(i => !SeenItems.Contains(i));
	}
}
