using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ModiBuff.Core.Units
{
	public sealed class PoisonDamageEffect : IStackEffect, IEffect, IStateEffect,
		IMetaEffectOwner<PoisonDamageEffect, float, int, float>, IPostEffectOwner<PoisonDamageEffect, float, int>
	{
		private const float PoisonDamage = 5f;

		private readonly StackEffectType _stackEffect;
		private readonly float _stackValue;
		private readonly Targeting _targeting;
		private IMetaEffect<float, int, float>[] _metaEffects;
		private IPostEffect<float, int>[] _postEffects;

		private float _extraDamage;
		private readonly Dictionary<IUnit, int> _poisonStacksPerUnit;

		public PoisonDamageEffect(StackEffectType stackEffect = StackEffectType.Effect, float stackValue = -1,
			Targeting targeting = Targeting.TargetSource) : this(stackEffect, stackValue, targeting, null, null)
		{
		}

		/// <summary>
		///		Manual modifier generation constructor
		/// </summary>
		public static PoisonDamageEffect Create(StackEffectType stackEffect = StackEffectType.Effect,
			float stackValue = -1, Targeting targeting = Targeting.TargetSource,
			IMetaEffect<float, int, float>[] metaEffects = null, IPostEffect<float, int>[] postEffects = null) =>
			new PoisonDamageEffect(stackEffect, stackValue, targeting, metaEffects, postEffects);

		private PoisonDamageEffect(StackEffectType stackEffect, float stackValue, Targeting targeting,
			IMetaEffect<float, int, float>[] metaEffects, IPostEffect<float, int>[] postEffects)
		{
			_stackEffect = stackEffect;
			_stackValue = stackValue;
			_targeting = targeting;
			_metaEffects = metaEffects;
			_postEffects = postEffects;

			_poisonStacksPerUnit = new Dictionary<IUnit, int>();
		}

		public PoisonDamageEffect SetMetaEffects(params IMetaEffect<float, int, float>[] metaEffects)
		{
			_metaEffects = metaEffects;
			return this;
		}

		public PoisonDamageEffect SetPostEffects(params IPostEffect<float, int>[] postEffects)
		{
			_postEffects = postEffects;
			return this;
		}

		public void Effect(IUnit target, IUnit source)
		{
			foreach (var kvp in _poisonStacksPerUnit)
			{
				//Check if the source is still alive, if not, handle it

				var stackSource = kvp.Key;
				int stacks = kvp.Value;
				float damage = stacks * PoisonDamage;

				if (_metaEffects != null)
					foreach (var metaEffect in _metaEffects)
						damage = metaEffect.Effect(damage, stacks, target, stackSource);

				damage += _extraDamage;

				float returnDamageInfo = Effect(damage, stacks, target, stackSource);

				if (_postEffects != null)
					foreach (var postEffect in _postEffects)
						postEffect.Effect(returnDamageInfo, stacks, target, stackSource);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private float Effect(float damage, int stacks, IUnit target, IUnit source)
		{
			_targeting.UpdateTargetSource(ref target, ref source);
			return ((IPoisonable)target).TakeDamagePoison(damage, stacks, source);
		}

		public void StackEffect(int stacks, IUnit target, IUnit source)
		{
			if (_poisonStacksPerUnit.ContainsKey(source))
				_poisonStacksPerUnit[source]++;
			else
				_poisonStacksPerUnit.Add(source, 1);

			if ((_stackEffect & StackEffectType.Add) != 0)
				_extraDamage += _stackValue;

			if ((_stackEffect & StackEffectType.AddStacksBased) != 0)
				_extraDamage += _stackValue * stacks;

			if ((_stackEffect & StackEffectType.Effect) != 0)
				Effect(target, source);
		}

		public void ResetState()
		{
			_extraDamage = 0;
			_poisonStacksPerUnit.Clear();
		}

		public IEffect ShallowClone() =>
			new PoisonDamageEffect(_stackEffect, _stackValue, _targeting, _metaEffects, _postEffects);

		object IShallowClone.ShallowClone() => ShallowClone();
	}
}