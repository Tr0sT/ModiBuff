using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModiBuff.Core
{
	public abstract class ModifierRecipes
	{
		public static int RecipesCount { get; private set; }

		private readonly IDictionary<string, IModifierRecipe> _recipes;
		private readonly IDictionary<string, ModifierEventRecipe> _eventRecipes; //TODO TEMP

		public ModifierRecipes()
		{
			_recipes = new Dictionary<string, IModifierRecipe>();
			_eventRecipes = new Dictionary<string, ModifierEventRecipe>();

			SetupRecipes();
			foreach (var modifier in _recipes.Values)
				modifier.Finish();
			foreach (var modifier in _eventRecipes.Values)
				((IModifierRecipe)modifier).Finish();

			RecipesCount = _recipes.Count + _eventRecipes.Count + 1;
			Debug.Log($"[ModiBuff] Loaded {RecipesCount} recipes.");
		}

		protected abstract void SetupRecipes();

		//TODO Will be invalid casts with EventRecipe
		public ModifierRecipe GetRecipe(string id) => (ModifierRecipe)_recipes[id];
		public ModifierEventRecipe GetEventRecipe(string id) => _eventRecipes[id];
		internal ModifierRecipe GetRecipe(int id) => (ModifierRecipe)_recipes.Values.ElementAt(id);

		internal ModifierRecipe[] GetRecipes() => _recipes.Values.Cast<ModifierRecipe>().ToArray();

		protected ModifierRecipe Add(string id)
		{
			var recipe = new ModifierRecipe(id);
			if (_recipes.ContainsKey(id))
			{
				Debug.LogError($"Modifier with id {id} already exists");
				return (ModifierRecipe)_recipes[id];
			}

			_recipes.Add(id, recipe);
			return recipe;
		}

		protected ModifierEventRecipe AddEvent(string id, EffectOnEvent effectOnEvent)
		{
			var recipe = new ModifierEventRecipe(id, effectOnEvent);
			if (_eventRecipes.ContainsKey(id))
			{
				Debug.LogError($"Modifier with id {id} already exists");
				return _eventRecipes[id];
			}

			_eventRecipes.Add(id, recipe);
			return recipe;
		}
	}
}