using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader.IO;

namespace NewStockIndicator.Common.Players;

public class StockRecordList {
	private readonly Dictionary<int, StockRecord> _records;

	public StockRecordList() {
		_records = new Dictionary<int, StockRecord>();
	}

	public bool TryGet(NPC npc, out StockRecord record) {
		if (!_records.ContainsKey(npc.type)) {
			if (Helpers.IsNPC(npc)) {
				_records[npc.type] = new StockRecord();
			}
		}

		if (_records.ContainsKey(npc.type)) {
			record = _records[npc.type];
			return true;
		}

		record = null;
		return false;
	}

	internal void Clear() {
		_records.Clear();
	}

	public TagCompound Serialize() {
		var list = new List<TagCompound>();

		foreach (var entry in _records) {
			var seenItems = entry.Value.SeenItems;

			if (seenItems.Count > 0) {
				// Combine NPC type and ItemsSeen list into a single tag
				var tag = Helpers.SerializeNPCType(entry.Key);
				tag["ItemsSeen"] = Helpers.SerializeItemTypeList(seenItems);

				list.Add(tag);
			}
		}

		return new TagCompound() {
			{"Records", list}
		};
	}

	public void Deserialize(TagCompound tag) {
		_records.Clear();

		if (!tag.ContainsKey("Records"))
			return;

		foreach (var recordData in tag.GetList<TagCompound>("Records")) {
			var npcType = Helpers.DeserializeNPCType(recordData);

			if (npcType.HasValue) {
				_records[npcType.Value] = new StockRecord() {
					SeenItems = Helpers.DeserializeItemTypeList(recordData.GetList<TagCompound>("ItemsSeen"))
				};
			}
		}
	}
}
