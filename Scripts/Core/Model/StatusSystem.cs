using Godot;
namespace DemoRPGGame
{
	// Đưa Enum ra ngoài cho dễ gọi
	public enum EffectType { Buff, Debuff }
	public enum Buff { None, ATKUp, DEFUp, SPDUp, Immunity }
	public enum Debuff { None, ATKDown, DEFDown, SPDDown }
	public enum Dot { None, Poison, Burn, Bleed }
	public enum CCType { None, Stun, Sleep, Silence }

	public class StatusEffect
	{
		public int AtkScale { get; set; } = 0;
		public int HpScale { get; set; } = 0;
		public string Name { get; set; }
		public EffectType Type { get; set; }

		// Các ID định danh
		public Buff? BuffID { get; set; } = Buff.None;
		public Debuff? DebuffID { get; set; } = Debuff.None;
		public Dot? DotID { get; set; } = Dot.None;
		public CCType? CcID { get; set; } = CCType.None;

		public int Duration { get; set; }
		public float Magnitude { get; set; }

		// Constructor giữ nguyên như bạn viết (Rất tốt!)
		public StatusEffect(Buff buff, int duration, float magnitude)
		{
			Type = EffectType.Buff;
			BuffID = buff;
			Duration = duration;
			Magnitude = magnitude;
			Name = buff.ToString();
		}
		public StatusEffect(Debuff debuff, int duration, float magnitude)
		{
			Type = EffectType.Debuff;
			DebuffID = debuff;
			Duration = duration;
			Magnitude = magnitude;
			Name = debuff.ToString();
		}
		public StatusEffect(Dot dot, int duration, float magnitude)
		{
			Type = EffectType.Debuff;
			DotID = dot;
			Duration = duration;
			Magnitude = magnitude;
			Name = dot.ToString();
		}
		public StatusEffect(CCType cc, int duration )
		{
			Type = EffectType.Debuff;
			CcID = cc;
			Duration = duration;
			Magnitude = 0;
			Name = cc.ToString();
		}
	}
}
