using ModiBuff.Core;
using NUnit.Framework;

namespace ModiBuff.Tests
{
	public sealed class PostEffectTests : ModifierTests
	{
		[Test]
		public void LifeSteal_OnDamageEffectInit()
		{
			var generator = Recipes.GetGenerator("InitDamageLifeStealPost");
			Unit.AddApplierModifier(generator, ApplierType.Cast);

			Unit.TakeDamage(2.5f, Unit);

			Unit.TryCast(generator.Id, Enemy);

			Assert.AreEqual(UnitHealth, Unit.Health);
			Assert.AreEqual(EnemyHealth - 5, Enemy.Health);
		}

		[Test]
		public void AddDamage_OnKill_WithDamageEffectInit()
		{
			var generator = Recipes.GetGenerator("InitDamageAddDamageOnKillPost");
			Unit.AddApplierModifier(generator, ApplierType.Cast);

			Enemy.TakeDamage(EnemyHealth - 5, Unit);

			Unit.TryCast(generator.Id, Enemy);

			Assert.AreEqual(UnitDamage + 2, Unit.Damage);
			Assert.AreEqual(0, Enemy.Health);
		}

		[Test]
		public void HealTargetDamageSelf()
		{
			var generator = Recipes.GetGenerator("HealDamageSelfPost");
			Unit.AddApplierModifier(generator, ApplierType.Cast);

			Enemy.TakeDamage(5, Enemy);

			Unit.TryCast(generator.Id, Enemy);

			Assert.AreEqual(EnemyHealth, Enemy.Health);
			Assert.AreEqual(UnitHealth - 5, Unit.Health);
		}
	}
}