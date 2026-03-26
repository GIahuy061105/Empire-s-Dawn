using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRPGGame
{
	public static class EquipmentGenerator
	{
		private static Random _rand = new Random();

		// Tỉ lệ ra đồ: Common 50%, Rare 30%, Epic 15%, Legendary 5%
		public static Rarity GenerateRandomRarity()
		{
			int roll = _rand.Next(1, 101); // 1-100
			if (roll <= 50) return Rarity.Common;
			if (roll <= 80) return Rarity.Rare;
			if (roll <= 95) return Rarity.Epic;
			return Rarity.Legendary;
		}

		public static Equipment GenerateItem(EquipmentSlot slot)
		{
			// 1. Random độ hiếm trước
			Rarity rarity = GenerateRandomRarity();

			var sets = Enum.GetValues(typeof(EquipmentSet));

			EquipmentSet set = (EquipmentSet)sets.GetValue(_rand.Next(sets.Length));

			// 2. Lấy Main Stat CỐ ĐỊNH theo độ hiếm
			EquipmentStats mainStat = GetFixedMainStat(slot, rarity);

			// 3. Số lượng dòng phụ cũng nên phụ thuộc độ hiếm (Option tùy chọn)
			// Common: 1 dòng, Rare: 2, Epic: 3, Legendary: 4
			int subStatCount = (int)rarity + 1;
			if (subStatCount > 4) subStatCount = 4;
			List<EquipmentStats> subStats = GenerateSubStats(subStatCount, mainStat.StatType);

			// 4. Tạo tên
			string name = $"{GetRarityPrefix(rarity)} {GetBaseName(slot)}";

			return new Equipment(name, rarity, slot, set, mainStat, subStats.ToArray());
		}

		// --- BẢNG TRA CỨU CHỈ SỐ CỐ ĐỊNH (FIXED STATS TABLE) ---
		private static EquipmentStats GetFixedMainStat(EquipmentSlot slot, Rarity rarity)
		{
			float val = 0;
			StatType selectedStat;
			switch (slot)
			{
				case EquipmentSlot.Weapon: // Main: ATK
					// Cố định: Common=100, Rare=250, Epic=500, Legendary=800
					val = rarity switch
					{
						Rarity.Common => 200,
						Rarity.Rare => 250,
						Rarity.Epic => 350,
						Rarity.Legendary => 400,
						_ => 100
					};
					return new EquipmentStats(StatType.Atk, val);

				case EquipmentSlot.Armor: // Main: DEF
					val = rarity switch
					{
						Rarity.Common => 50,
						Rarity.Rare => 100,
						Rarity.Epic => 150,
						Rarity.Legendary => 200,
						_ => 50
					};
					return new EquipmentStats(StatType.Def, val);

				case EquipmentSlot.Helmet: // Main: HP
					val = rarity switch
					{
						Rarity.Common => 500,
						Rarity.Rare => 1000,
						Rarity.Epic => 1500,
						Rarity.Legendary => 2000,
						_ => 500
					};
					return new EquipmentStats(StatType.Hp, val);

				case EquipmentSlot.Boots: // Main: SPEED
					val = rarity switch
					{
						Rarity.Common => 15,
						Rarity.Rare => 25,
						Rarity.Epic => 30,
						Rarity.Legendary => 45, // Giày cam chạy siêu nhanh
						_ => 15
					};
					return new EquipmentStats(StatType.Speed, val);

				case EquipmentSlot.Ring:
					// 1. Random loại chỉ số cho Nhẫn
					StatType[] ringOptions = { StatType.AtkPercent, StatType.HpPercent, StatType.DefPercent, StatType.Effectiveness, StatType.Resistance };
					selectedStat = ringOptions[_rand.Next(ringOptions.Length)];

					// 2. Tính giá trị dựa trên Độ hiếm (Các chỉ số này scale giống nhau)
					val = rarity switch
					{
						Rarity.Common => 0.10f,    // 10%
						Rarity.Rare => 0.25f,      // 25%
						Rarity.Epic => 0.40f,      // 40%
						Rarity.Legendary => 0.60f, // 60%
						_ => 0.1f
					};
					return new EquipmentStats(selectedStat, val);

				// --- DÂY CHUYỀN: Random giữa (CritRate, CritDamage, ATK%, HP%, DEF%) ---
				case EquipmentSlot.Amulet:
					// 1. Random loại chỉ số cho Dây chuyền
					StatType[] amuletOptions = { StatType.CritRate, StatType.CritDamage, StatType.AtkPercent, StatType.HpPercent, StatType.DefPercent };
					selectedStat = amuletOptions[_rand.Next(amuletOptions.Length)];

					// 2. Tính giá trị 
					switch (selectedStat)
					{
						case StatType.CritRate:
							val = rarity switch
							{
								Rarity.Common => 0.05f,    // 5%
								Rarity.Rare => 0.15f,      // 15%
								Rarity.Epic => 0.25f,      // 25%
								Rarity.Legendary => 0.35f, // 35%
								_ => 0.05f
							};
							break;

						case StatType.CritDamage:
							val = rarity switch
							{
								Rarity.Common => 0.15f,    // 15%
								Rarity.Rare => 0.30f,      // 30%
								Rarity.Epic => 0.50f,      // 50%
								Rarity.Legendary => 0.70f, // 70%
								_ => 0.15f
							};
							break;

						default: // ATK, HP, DEF scale chuẩn (Max 60%)
							val = rarity switch
							{
								Rarity.Common => 0.30f,
								Rarity.Rare => 0.35f,
								Rarity.Epic => 0.45f,
								Rarity.Legendary => 0.60f,
								_ => 0.3f
							};
							break;
					}
					return new EquipmentStats(selectedStat, val);

				default:
					return new EquipmentStats(StatType.Hp, 500);
			}
		}


		private static List<EquipmentStats> GenerateSubStats(int count, StatType excludeStat)
		{
			List<EquipmentStats> subs = new List<EquipmentStats>();
			var allStats = Enum.GetValues(typeof(StatType)).Cast<StatType>().ToList();

			for (int i = 0; i < count; i++)
			{
				if (allStats.Count == 0) break;
				int index = _rand.Next(allStats.Count);
				StatType selectedType = allStats[index];

				// Demo giá trị random nhỏ cho substat
				float val = 0;
				if (selectedType == StatType.CritRate || selectedType == StatType.CritDamage || selectedType == StatType.Effectiveness
					|| selectedType == StatType.Resistance)
				{
					val = (float)(_rand.NextDouble() * 0.08f + 0.02f); // 0% - 10%
				}
				else if (selectedType == StatType.AtkPercent || selectedType == StatType.HpPercent || selectedType == StatType.DefPercent)
				{
					val = (float)(_rand.NextDouble() * 0.10f + 0.10f); // 10% - 20%
				}
				else
					val = _rand.Next(15, 50); // Chỉ số thường 15-50

				subs.Add(new EquipmentStats(selectedType, val));
				allStats.RemoveAt(index);
			}
			return subs;
		}

		public static List<Equipment> GenerateFullSetForCharacter(string charName)
		{
			List<Equipment> set = new List<Equipment>();
			foreach (EquipmentSlot type in Enum.GetValues(typeof(EquipmentSlot)))
			{
				set.Add(GenerateItem(type));
			}
			return set;
		}

		private static string GetBaseName(EquipmentSlot type)
		{
			return type.ToString(); // Đơn giản hóa tên
		}

		private static string GetRarityPrefix(Rarity rarity)
		{
			return rarity switch
			{
				Rarity.Common => "[Thường]",
				Rarity.Rare => "[Hiếm]",
				Rarity.Epic => "[Sử Thi]",
				Rarity.Legendary => "[HUYỀN THOẠI]",
				_ => ""
			};
		}
	}
}
