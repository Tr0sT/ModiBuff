using ModiBuff.Core;
using NUnit.Framework;

namespace ModiBuff.Tests
{
	public sealed class PoolTests : ModifierTests
	{
		[Test]
		public void TimeStateReset()
		{
			Pool.Clear();
			Pool.Allocate(IdManager.GetId("DurationRemove"), 1);

			Unit.AddModifierSelf("DurationRemove");

			Unit.Update(1);
			Assert.True(Unit.ContainsModifier("DurationRemove"));

			Unit.Update(4);
			Assert.False(Unit.ContainsModifier("DurationRemove")); //Return to pool

			Enemy.AddModifierSelf("DurationRemove"); //State should be reset
			Assert.True(Enemy.ContainsModifier("DurationRemove"));
			Enemy.Update(1);
			Assert.True(Enemy.ContainsModifier("DurationRemove"));

			Enemy.Update(4);
			Assert.False(Enemy.ContainsModifier("DurationRemove"));
		}

		[Test]
		public void StackStateReset()
		{
			Pool.Clear();
			Pool.Allocate(IdManager.GetId("StackBasedDamage"), 1);

			Unit.AddModifierSelf("StackBasedDamage");
			Assert.AreEqual(UnitHealth - 5 - 2, Unit.Health); //1 stack = +2 damage == 2

			Unit.AddModifierSelf("StackBasedDamage");
			Assert.AreEqual(UnitHealth - 10 - 6, Unit.Health); //2 stacks = +4 damage == 4

			Unit.ModifierController.Remove(new ModifierReference(IdManager.GetId("StackBasedDamage"), 0)); //Return to pool

			Enemy.AddModifierSelf("StackBasedDamage"); //State should be reset
			Assert.AreEqual(EnemyHealth - 5 - 2, Enemy.Health);
		}

		[Test]
		public void AllocateModifiers_RentAll()
		{
			const int count = 5000;

			var modifiers = new Modifier[count];

			var recipe = Recipes.GetRecipe("InitDamage");
			Pool.Allocate(recipe.Id, count);

			for (int i = 0; i < count; i++)
				modifiers[i] = Pool.Rent(recipe.Id);

			for (int i = 0; i < count; i++)
				Pool.Return(modifiers[i]);
		}

		//[Test]
		public void FullLibraryInit()
		{
			Config.PoolSize = 512;
			Pool.Dispose();
			IdManager.Reset();

			var idManager = new ModifierIdManager();
			var recipes = new TestModifierRecipes(idManager);
			var pool = new ModifierPool(recipes.GetRecipes());

			Config.Reset();
		}

		//TODO Pool AddedDamage revertible state reset
	}
}