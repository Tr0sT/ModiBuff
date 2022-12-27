using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModiBuff.Core
{
	public abstract class ModifierRecipes
	{
		public static int RecipesCount { get; private set; }

		private readonly IDictionary<string, IModifierRecipe> _recipes;

		public ModifierRecipes()
		{
			_recipes = new Dictionary<string, IModifierRecipe>();

			SetupRecipes();
			foreach (var modifier in _recipes.Values)
				modifier.Finish();

			RecipesCount = _recipes.Count;
			Debug.Log($"[ModiBuff] Loaded {RecipesCount} recipes.");
		}

		protected abstract void SetupRecipes();

		public IModifierRecipe GetRecipe(string id) => _recipes[id];
		internal IModifierRecipe GetRecipe(int id) => _recipes.Values.ElementAt(id);

		internal IModifierRecipe[] GetRecipes() => _recipes.Values.ToArray();

		protected ModifierRecipe Add(string name)
		{
			var recipe = new ModifierRecipe(name);
			if (_recipes.ContainsKey(name))
			{
				Debug.LogError($"Modifier with id {name} already exists");
				return (ModifierRecipe)_recipes[name];
			}

			_recipes.Add(name, recipe);
			return recipe;
		}

		protected ModifierEventRecipe AddEvent(string name, EffectOnEvent effectOnEvent)
		{
			var recipe = new ModifierEventRecipe(name, effectOnEvent);
			if (_recipes.ContainsKey(name))
			{
				Debug.LogError($"Modifier with id {name} already exists");
				return (ModifierEventRecipe)_recipes[name];
			}

			_recipes.Add(name, recipe);
			return recipe;
		}
	}
}