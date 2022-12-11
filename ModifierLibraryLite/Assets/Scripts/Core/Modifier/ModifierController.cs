using System.Collections.Generic;
using UnityEngine;

namespace ModifierLibraryLite.Core
{
	public sealed class ModifierController
	{
		//TODO Array mapping?
		private readonly IDictionary<string, Modifier> _modifiers;
		private readonly HashSet<ModifierRecipe> _modifierRecipeAppliers;

		private readonly List<string> _modifiersToRemove;

		public ModifierController()
		{
			_modifiers = new Dictionary<string, Modifier>();
			_modifierRecipeAppliers = new HashSet<ModifierRecipe>(5);

			_modifiersToRemove = new List<string>(5);
		}

		public void Update(in float delta)
		{
			//int length = _modifiers.Count;
			//TODO Array for loop mapping
			foreach (var modifier in _modifiers.Values)
			{
				modifier.Update(delta);

				if (modifier.ToRemove)
					_modifiersToRemove.Add(modifier.Id);
			}

			for (int i = 0; i < _modifiersToRemove.Count; i++)
				_modifiers.Remove(_modifiersToRemove[i]);
		}

		public IReadOnlyCollection<ModifierRecipe> GetApplierModifiers()
		{
			return _modifierRecipeAppliers;
		}

		//TODO do appliers make sense? Should we just store the id, what kind of state do appliers have?
		public (bool Success, Modifier Modifier) TryAdd(ModifierRecipe recipe, IUnit owner, IUnit target, IUnit sender = null)
		{
			//TODO We should call the original modifier's check component here or before
			return (true, Add(recipe, owner, target, sender));
		}

		public bool TryAddApplier(ModifierRecipe recipe)
		{
			if (_modifierRecipeAppliers.Contains(recipe))
				return false;

			_modifierRecipeAppliers.Add(recipe);
			return true;
		}

		public bool TryAddAppliers(ModifierRecipe[] recipes)
		{
			bool success = true;
			foreach (var recipe in recipes)
			{
				if (!TryAddApplier(recipe))
					success = false;
			}

			return success;
		}

		private Modifier Add(ModifierRecipe recipe, IUnit owner, IUnit target, IUnit sender = null)
		{
			//TODO We should call the original modifier's check component before

			if (_modifiers.TryGetValue(recipe.Id, out var existingModifier))
			{
				//TODO should we update the modifier targets when init/refreshing/stacking?
				existingModifier.Init();
				existingModifier.Refresh();
				existingModifier.Stack();
				return existingModifier;
			}

			var modifier = recipe.Create();

			//TODO Do we want to save the sender of the original modifier? Ex. for thorns. Because owner is always the owner of the modifier instance
			modifier.SetTargets(target, owner, sender);

			_modifiers.Add(modifier.Id, modifier);
			modifier.Init();
			modifier.Refresh();
			modifier.Stack();
			return modifier;
		}

		public bool Contains(ModifierRecipe recipe) => Contains(recipe.Id);
		public bool Contains(Modifier modifier) => Contains(modifier.Id);
		public bool Contains(string id) => _modifiers.ContainsKey(id);
	}
}