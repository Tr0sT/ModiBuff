using ModiBuff.Core;
using NUnit.Framework;

namespace ModiBuff.Tests
{
	public sealed class ConditionTests : BaseModifierTests
	{
		[Test]
		public void HealthCondition_OnApply_InitDamage()
		{
			Unit.TakeDamage(UnitHealth - 6, Unit); //6hp left

			Unit.AddApplierModifier(Recipes.GetRecipe("InitDamage_ApplyCondition_HealthAbove100"), ApplierType.Cast);
			Unit.Cast(Unit);
			Assert.AreEqual(UnitHealth - UnitHealth + 6, Unit.Health);
		}

		[Test]
		public void HealthCondition_OnEffect_InitDamage()
		{
			Unit.TryAddModifierSelf("InitDamage_EffectCondition_HealthAbove100");
			Assert.AreEqual(UnitHealth - 5, Unit.Health); //995

			Unit.TakeDamage(UnitHealth - 6, Unit); //1000-6=994 => 1 hp left
			Unit.TryAddModifierSelf("InitDamage_EffectCondition_HealthAbove100");
			Assert.AreEqual(1, Unit.Health); //Still 1hp left
		}

		[Test]
		public void HealthIsFullCondition_OnEffect_InitDamage()
		{
			Unit.TryAddModifierSelf("InitDamage_EffectCondition_HealthFull");
			Assert.AreEqual(UnitHealth - 5, Unit.Health);

			Unit.TryAddModifierSelf("InitDamage_EffectCondition_HealthFull");
			Assert.AreEqual(UnitHealth - 5, Unit.Health);
		}
		//TODO HealthIsBelow 50%-10%
		//TODO Has StatusEffect
		//TODO Has LegalAction
		//TODO Has Modifier
	}
}