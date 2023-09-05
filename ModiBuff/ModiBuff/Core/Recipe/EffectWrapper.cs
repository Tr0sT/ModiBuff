namespace ModiBuff.Core
{
	public sealed class EffectWrapper
	{
		private readonly IEffect _effect;
		public readonly EffectOn EffectOn;

		private readonly bool _effectIsCloneable;
		private readonly IShallowClone<IEffect> _effectShallowClone;

		private IEffect _effectClone;

		public EffectWrapper(IEffect effect, EffectOn effectOn)
		{
			_effect = effect;
			EffectOn = effectOn;

			if (_effect is IShallowClone<IEffect> shallowClone)
			{
				_effectIsCloneable = true;
				_effectShallowClone = shallowClone;
			}
		}

		public IEffect GetEffect()
		{
			if (_effectIsCloneable)
			{
				if (_effectClone == null)
					_effectClone = _effectShallowClone.ShallowClone();
				return _effectClone;
			}

			return _effect;
		}

		/// <summary>
		///		Only called in remove effect wrapper
		/// </summary>
		public void UpdateGenId(int genId)
		{
			((IRemoveEffect)GetEffect()).SetGenId(genId);
		}

		public void Reset()
		{
			_effectClone = null;
		}
	}
}