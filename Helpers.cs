using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace NewStockIndicator;

public static class Helpers {
	// Pull up the private shopToNPC array from tMod's NPCLoader
	private static readonly int[] ShopToNPC = (int[]) typeof(NPCLoader).GetField("shopToNPC", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

	public static int GetVanillaShopIndex(NPC npc) {
		for (int i = 0; i < ShopToNPC.Length; i++)
			if (ShopToNPC[i] == npc.type)
				return i;

		return 0;
	}

	public static bool IsNPC(NPC npc) {
		return npc.townNPC || npc.isLikeATownNPC;
	}

	public static void FillWithShopItems(Chest chest, NPC npc) {
		Array.Fill(chest.item, new Item());

		var vanillaShopIndex = GetVanillaShopIndex(npc);

		if (vanillaShopIndex > 0) {
			chest.SetupShop(vanillaShopIndex);
		} else if (npc.type >= NPCID.Count) {
			chest.SetupShop(npc.type);
		}
	}

	#region Serialization/Deserialization related
	public static TagCompound SerializeNPCType(int id) {
		if (id >= NPCID.Count) {
			var modNpc = NPCLoader.GetNPC(id);
			return new TagCompound() {
				{"Mod", modNpc.Mod.Name},
				{"InternalName", modNpc.Name}
			};
		} else {
			return new TagCompound() {
				{"Type", id}
			};
		}
	}

	public static int? DeserializeNPCType(TagCompound tag) {
		var isModded = tag.ContainsKey("Mod");

		if (isModded) {
			var modName = tag.GetString("Mod");
			var itemName = tag.GetString("InternalName");

			if (ModContent.TryFind<ModNPC>(modName, itemName, out var modNpc))
				return modNpc.Type;

			return null;
		} else {
			return tag.GetInt("Type");
		}
	}

	public static TagCompound SerializeItemType(int id) {
		if (id >= ItemID.Count) {
			var modItem = ItemLoader.GetItem(id);
			return new TagCompound() {
				{"Mod", modItem.Mod.Name},
				{"InternalName", modItem.Name}
			};
		} else {
			return new TagCompound() {
				{"Type", id}
			};
		}
	}

	public static int? DeserializeItemType(TagCompound tag) {
		var isModded = tag.ContainsKey("Mod");

		if (isModded) {
			var modName = tag.GetString("Mod");
			var itemName = tag.GetString("InternalName");

			if (ModContent.TryFind<ModItem>(modName, itemName, out var modItem))
				return modItem.Type;

			return null;
		} else {
			return tag.GetInt("Type");
		}
	}

	public static List<TagCompound> SerializeItemTypeList(HashSet<int> items) {
		return items.Select(i => SerializeItemType(i)).ToList();
	}

	public static HashSet<int> DeserializeItemTypeList(IList<TagCompound> tags) {
		return tags.Select(i => DeserializeItemType(i)).OfType<int>().ToHashSet();
	}
	#endregion
}
