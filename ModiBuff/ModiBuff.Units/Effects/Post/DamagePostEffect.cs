namespace ModiBuff.Core.Units
{
	public sealed class DamagePostEffect : IPostEffect<float>
	{
		private readonly Targeting _targeting;

		public DamagePostEffect(Targeting targeting = Targeting.TargetSource)
		{
			_targeting = targeting;
		}

		public void Effect(float value, IUnit target, IUnit source)
		{
			_targeting.UpdateTargetSource(ref target, ref source);
			if (!(target is IAttackable<float, float> attackableTarget))
				return;

			attackableTarget.TakeDamage(value, source);
		}
	}
}