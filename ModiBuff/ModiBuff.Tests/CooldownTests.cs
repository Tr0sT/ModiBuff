using ModiBuff.Core;
using NUnit.Framework;

namespace ModiBuff.Tests
{
	public sealed class CooldownTests : ModifierTests
	{
		[Test]
		public void InitDamage_Cooldown()
		{
			Unit.AddApplierModifier(Recipes.GetGenerator("InitDamage_Cooldown"), ApplierType.Attack);

			Unit.Attack(Enemy);

			Assert.AreEqual(EnemyHealth - UnitDamage - 5, Enemy.Health);

			// 1 second cooldown
			Unit.Attack(Enemy);
			Assert.AreEqual(EnemyHealth - UnitDamage * 2 - 5, Enemy.Health);

			Unit.Update(1); //Cooldown gone
			Unit.Attack(Enemy);
			Assert.AreEqual(EnemyHealth - UnitDamage * 3 - 5 * 2, Enemy.Health);
		}

		[Test]
		public void InitDamage_Cooldown_Effect()
		{
			Unit.AddModifierSelf("InitDamage_Cooldown_Effect"); // 1 second cooldown
			Assert.AreEqual(UnitHealth - 5, Unit.Health);

			Unit.AddModifierSelf("InitDamage_Cooldown_Effect"); // On Cooldown
			Assert.AreEqual(UnitHealth - 5, Unit.Health);

			Unit.Update(1); //Cooldown gone
			Unit.AddModifierSelf("InitDamage_Cooldown_Effect");
			Assert.AreEqual(UnitHealth - 5 * 2, Unit.Health);
		}

		[Test]
		public void InitDamage_Cooldown_Pool()
		{
			int id = IdManager.GetId("InitDamage_Cooldown_Pool");
			Pool.Clear();
			Pool.Allocate(id, 1);

			Unit.AddModifierSelf("InitDamage_Cooldown_Pool"); // 1 second cooldown
			Assert.AreEqual(UnitHealth - 5, Unit.Health);
			Unit.ModifierController.Remove(new ModifierReference(id, -1)); //State reset, back in pool, no cooldown

			Unit.AddModifierSelf("InitDamage_Cooldown_Pool"); // No cooldown
			Assert.AreEqual(UnitHealth - 5 - 5, Unit.Health);
		}
	}
}