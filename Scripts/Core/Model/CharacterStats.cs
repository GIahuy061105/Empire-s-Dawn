using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoRPGGame;

namespace DemoRPGGame

{
	public class CharacterStats
	{
		public int Attack { get; set; }
		public int MaxHP { get; set; }
		public int Defense { get; set; }
		public int Speed { get; set; }
		public float CritRate { get; set; }
		public float CritDamage { get; set; }
		public float Effectiveness { get; set; }
		public float Resistance { get; set; }


		public CharacterStats(int atk, int def, int hp, int spd, float critRate = 0.15f, float critDmg = 1.5f, float eff = 0.1f , float res = 0.0f)
		{
			Attack = atk;
			Defense = def;
			MaxHP = hp;
			Speed = spd;
			CritRate = critRate;
			CritDamage = critDmg;
			Effectiveness = eff;
			Resistance = res;
		}
	}
	public enum Element
	{
		Fire,
		Water,
		Earth,
		Light,
		Dark
	}
	public enum CharClass
	{
		Warrior,
		Knight,
		Thief,
		Mage,
		Archer,
		Priest,

		Monster
	}
}
