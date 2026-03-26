using Godot;
using System.Collections.Generic;
using System.Linq; 

namespace DemoRPGGame
{
	public enum MonsterRank { Normal, Elite, Boss }

	public class Monster : Character
	{
		public MonsterRank Rank { get; set; }
		private bool _isEnraged = false;
		public Dictionary<MaterialType, int> DropTable { get; set; } = new Dictionary<MaterialType, int>();
		public int GoldDrop { get; set; } = 0;

		public Monster(string name, MonsterRank rank, CharacterStats stats) : base(name, stats)
		{
			Rank = rank;
		}

		public void CheckEnrage()
		{
			if (!_isEnraged && CurrentHP > 0 && CurrentHP < this.SheetHP * 0.5f)
			{
				_isEnraged = true;
				PerformEnrage();
			}
		}

		public virtual void PerformEnrage() { }

		public virtual void ExecuteTurn(List<Character> allFighters)
		{
			var heroes = allFighters.FindAll(c => c.CurrentHP > 0 && !(c is Monster));
			if (heroes.Count == 0) return;
			Character target = heroes[System.Random.Shared.Next(heroes.Count)];
			Attack(target);
		}

		public Dictionary<MaterialType, int> GetDropMaterials()
		{
			var drops = new Dictionary<MaterialType, int>();
			if (GoldDrop > 0) drops.Add(MaterialType.Gold, GoldDrop);

			foreach (var item in DropTable)
			{
				float dropChance = Rank switch
				{
					MonsterRank.Normal => 0.5f,
					MonsterRank.Elite => 0.75f,
					MonsterRank.Boss => 1.0f,
					_ => 0f
				};

				dropChance *= item.Value / 100f;
				if (System.Random.Shared.NextDouble() <= dropChance)
				{
					drops.Add(item.Key, item.Value);
				}
			}
			return drops;
		}
	}
}
