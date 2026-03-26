using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoRPGGame
{
	public enum StatType
	{
		Atk,AtkPercent,
		Def,DefPercent,
		Hp,HpPercent,
		Speed,
		CritRate,
		CritDamage,
		Resistance,
		Effectiveness
	}
	public enum Rarity
	{
		Common,
		Uncommon,
		Rare,
		Epic,
		Legendary
	}
	public enum EquipmentSlot 
	{
		Weapon, //Atk 
		Armor, // def
		Helmet, // Hp 
		Boots, // Speed 
		Ring, //  Atk , CritRate , CritDamage , Resistance , Effectiveness , Speed
		Amulet // Atk , CritRate , CritDamage , Resistance , Effectiveness , Speed
	}
	public enum EquipmentSet
	{
		Attack_Set, //Set 4
		Speed_Set, // Set 4
		Defense_Set, // Set 2
		Health_Set, // Set 2
		Effectiveness_Set, //Set 2
		Resistance_Set, //Set 2
		Critical_Set, // Set 2
		CritDamage_Set  // Set 4
	}

	public class EquipmentStats
	{
		public StatType StatType { get; set; }
		public float Value { get; set; } 
		public EquipmentStats(StatType statType, float value)
		{
			StatType = statType;
			Value = value;
		}
	}
	public class Equipment
	{
		public string Name { get; set; }
		public EquipmentSlot Slot { get; set; }
		public EquipmentSet Set { get; set; }
		public Rarity Rarity { get; set; } = Rarity.Common;
		public EquipmentStats MainStats { get; set; }
		public List<EquipmentStats> SubStats { get; set; } = new List<EquipmentStats>();
		public Equipment(string name, Rarity rarity , EquipmentSlot slot,EquipmentSet set , EquipmentStats mainstats , params EquipmentStats[] substats)
		{
			Name = name;
			Rarity = rarity;
			Set = set;
			Slot = slot;
			MainStats = mainstats;
			SubStats = substats.ToList();
		}
	}
}
