using System.Collections.Generic;

namespace ModiBuff.Core
{
	public struct InitComponent : IStateReset
	{
		private readonly IEffect[] _effects;
		private readonly bool _oneTime;
		private readonly ModifierCheck _modifierCheck;

		private bool _isInitialized;

		public InitComponent(bool oneTimeInit, IEffect[] effects, ModifierCheck check)
		{
			_oneTime = oneTimeInit;
			_effects = effects;
			_modifierCheck = check;

			_isInitialized = false;
		}

		public void Init(IUnit target, IUnit owner)
		{
			if (_oneTime && _isInitialized)
				return;

			if (_modifierCheck != null && !_modifierCheck.Check(owner))
				return;

			for (int i = 0; i < _effects.Length; i++)
				_effects[i].Effect(target, owner);

			_isInitialized = true;
		}

		public void Init(IList<IUnit> targets, IUnit owner)
		{
			if (_oneTime && _isInitialized)
				return;

			if (_modifierCheck != null && !_modifierCheck.Check(owner))
				return;

			for (int i = 0; i < _effects.Length; i++)
				_effects[i].Effect(targets, owner);

			_isInitialized = true;
		}

		public void InitLoad(IUnit target, IUnit owner)
		{
			for (int i = 0; i < _effects.Length; i++)
			{
				var effect = _effects[i];
				if (effect is IRegisterEffect)
					effect.Effect(target, owner);
			}
		}

		public void InitLoad(IList<IUnit> targets, IUnit owner)
		{
			for (int i = 0; i < _effects.Length; i++)
			{
				var effect = _effects[i];
				if (effect is IRegisterEffect)
					effect.Effect(targets, owner);
			}
		}

		public void ResetState() => _isInitialized = false;

		public SaveData SaveState() => new SaveData(_isInitialized);
		public void LoadState(SaveData data) => _isInitialized = data.IsInitialized;

		public readonly struct SaveData
		{
			public readonly bool IsInitialized;

#if JSON_SERIALIZATION && (NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER || NET462_OR_GREATER || NETCOREAPP2_1_OR_GREATER)
			[System.Text.Json.Serialization.JsonConstructor]
#endif
			public SaveData(bool isInitialized) => IsInitialized = isInitialized;
		}
	}
}