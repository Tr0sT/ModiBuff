namespace ModiBuff.Core
{
	public sealed class RemoveEffect : IRemoveEffect, IModifierIdOwner, IShallowClone<RemoveEffect>
	{
		private IRevertEffect[] _revertibleEffects;
		private int _id;

		public RemoveEffect()
		{
		}

		internal RemoveEffect(int id) => _id = id;

		public void SetModifierId(int id) => _id = id;

		public void SetRevertibleEffects(IRevertEffect[] revertibleEffects)
		{
			_revertibleEffects = revertibleEffects;
		}

		public void Effect(IUnit target, IUnit source)
		{
			//Debug.Log("RemoveEffect Effect, modifier id: " + _modifier.Id);
			for (int i = 0; i < _revertibleEffects.Length; i++)
				_revertibleEffects[i].RevertEffect(target, source);

			//Still not fully ideal, but fixed the state issue 
			((IModifierOwner)target).ModifierController.PrepareRemove(_id); //TODO From which collection? Applier support?
		}

		public RemoveEffect ShallowClone() => new RemoveEffect(_id);
		object IShallowClone.ShallowClone() => ShallowClone();
	}
}