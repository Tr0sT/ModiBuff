using ModiBuff.Core;
using ModiBuff.Core.Units;
using NUnit.Framework;

namespace ModiBuff.Tests
{
	public class ReactCallbackTests : ModifierTests
	{
		[Test]
		public void AddDamageAbove5RemoveDamageBelow5React_Manual()
		{
			AddGenerator("AddDamageAbove5RemoveDamageBelow5React", (id, genId, name) =>
			{
				var effect = new AddDamageEffect(5, true, true);
				var @event = new DamageChangedEvent((unit, newDamage, deltaDamage) =>
				{
					if (newDamage > 9)
						effect.Effect(unit, unit);
					else
						effect.RevertEffect(unit, unit);
				});

				var registerReactEffect = new ReactCallbackRegisterEffect<ReactType>(
					new ReactCallback<ReactType>(ReactType.DamageChanged, @event));
				var initComponent = new InitComponent(false, new IEffect[] { registerReactEffect }, null);
				return new Modifier(id, genId, name, initComponent, null, default(StackComponent), null,
					new SingleTargetComponent(), null);
			}, new ModifierAddData(true, false, false, false));
			Setup();

			Unit.AddModifierSelf("AddDamageAbove5RemoveDamageBelow5React"); //Starts with 10 baseDmg, adds 5 from effect
			Assert.AreEqual(UnitDamage + 5, Unit.Damage);
			Unit.Update(0);

			//Remove 6 damage, should remove the effect, making it 15 - 6 - 5 = 4
			Unit.AddDamage(-6); //Revert
			Assert.AreEqual(UnitDamage - 6, Unit.Damage);

			Unit.Update(0);
			Unit.AddDamage(6);
			Assert.AreEqual(UnitDamage + 5, Unit.Damage);
		}

		//TODO Finish recipe react callbacks
		/*[Test]
		public void AddDamageAbove5RemoveDamageBelow5React()
		{
			AddRecipe("AddDamageAbove5RemoveDamageBelow5React")
				.Effect(new AddDamageEffect(5, true, true), EffectOn.ReactCallback)
				.ReactCallback(ReactType.DamageChanged, (IRevertEffect effectReference) => new DamageChangedEvent((unit, newDamage, deltaDamage) =>
				{
					if (newDamage > 9)
						effectReference.Effect(unit, unit);
					else
						effectReference.RevertEffect(unit, unit);
				}));
			Setup();

			Unit.AddModifierSelf("AddDamageAbove5RemoveDamageBelow5React"); //Starts with 10 baseDmg, adds 5 from effect
			Assert.AreEqual(UnitDamage + 5, Unit.Damage);
			Unit.Update(0);

			//Remove 6 damage, should remove the effect, making it 15 - 6 - 5 = 4
			Unit.AddDamage(-6); //Revert
			Assert.AreEqual(UnitDamage - 6, Unit.Damage);

			Unit.Update(0);
			Unit.AddDamage(6);
			Assert.AreEqual(UnitDamage + 5, Unit.Damage);
		}*/

		[Test]
		public void InitStatusEffectSleep_RemoveOnTenDamageTaken_Manual()
		{
			AddGenerator("InitStatusEffectSleep_RemoveOnTenDamageTaken", (id, genId, name) =>
			{
				var effect = new StatusEffectEffect(StatusEffectType.Sleep, 5f, true);
				effect.SetModifierId(id);
				effect.SetGenId(genId);
				var removeEffect = new RemoveEffect(id, genId);
				float totalDamageTaken = 0f;
				var @event = new HealthChangedEvent((target, source, health, deltaHealth) =>
				{
					totalDamageTaken += deltaHealth;
					if (totalDamageTaken >= 10)
						removeEffect.Effect(target, source);
				});
				var registerReactEffect = new ReactCallbackRegisterEffect<ReactType>(
					new ReactCallback<ReactType>(ReactType.CurrentHealthChanged, @event));
				//Order of reverts matters here, if we revert the captured variable after
				//it will trigger a recursive effect, because the captured variable will never be reset
				removeEffect.SetRevertibleEffects(new IRevertEffect[]
					{ effect, new RevertActionEffect(() => { totalDamageTaken = 0f; }), registerReactEffect });

				var initComponent = new InitComponent(false, new IEffect[] { effect, registerReactEffect }, null);
				return new Modifier(id, genId, name, initComponent, null, default(StackComponent), null,
					new SingleTargetComponent(), null);
			}, new ModifierAddData(true, false, false, false));
			Setup();

			//Starts with 10 baseDmg, adds 5 from effect
			Unit.AddModifierSelf("InitStatusEffectSleep_RemoveOnTenDamageTaken");
			Assert.True(Unit.StatusEffectController.HasStatusEffect(StatusEffectType.Sleep));
			Unit.Update(4); //Still has sleep

			Unit.TakeDamage(9, Unit); //Still has sleep
			Assert.True(Unit.StatusEffectController.HasStatusEffect(StatusEffectType.Sleep));

			Unit.TakeDamage(2, Unit); //Removes and reverts sleep
			Assert.False(Unit.StatusEffectController.HasStatusEffect(StatusEffectType.Sleep));
		}

		/*[Test]
		public void InitStatusEffectSleep_RemoveOnTenDamageTaken_Recipe()
		{
			AddRecipe("InitStatusEffectSleep_RemoveOnTenDamageTaken")
				.Effect(new StatusEffectEffect(StatusEffectType.Sleep, 5f, true), EffectOn.Init)
				.Effect(new RemoveEffect(), EffectOn.React)
				.ReactCallback(ReactType.CurrentHealthChanged, (reactState, effectReference) =>
					new HealthChangedEvent((unit, health, deltaHealth) =>
					{
						reactState.Value += deltaHealth;
						if (reactState.Value >= 10)
							effectReference.Effect(unit, unit);
					}));

			//Starts with 10 baseDmg, adds 5 from effect
			Unit.AddModifierSelf("InitStatusEffectSleep_RemoveOnTenDamageTaken");
			Assert.True(Unit.StatusEffectController.HasStatusEffect(StatusEffectType.Sleep));
			Unit.Update(4); //Still has sleep

			Unit.TakeDamage(9, Unit); //Still has sleep
			Assert.True(Unit.StatusEffectController.HasStatusEffect(StatusEffectType.Sleep));

			Unit.TakeDamage(2, Unit); //Removes and reverts sleep
			Assert.False(Unit.StatusEffectController.HasStatusEffect(StatusEffectType.Sleep));
		}*/

		[Test]
		public void InitStatusEffectSleep_RemoveOnTenDamageTaken_Manual_StateReset()
		{
			AddGenerator("InitStatusEffectSleep_RemoveOnTenDamageTaken", (id, genId, name) =>
			{
				var effect = new StatusEffectEffect(StatusEffectType.Sleep, 5f, true);
				effect.SetModifierId(id);
				effect.SetGenId(genId);
				var removeEffect = new RemoveEffect(id, genId);
				float totalDamageTaken = 0f;
				var @event = new HealthChangedEvent((target, source, health, deltaHealth) =>
				{
					totalDamageTaken += deltaHealth;
					if (totalDamageTaken >= 10)
						removeEffect.Effect(target, source);
				});
				var registerReactEffect = new ReactCallbackRegisterEffect<ReactType>(
					new ReactCallback<ReactType>(ReactType.CurrentHealthChanged, @event));
				//Order of reverts matters here, if we revert the captured variable after
				//it will trigger a recursive effect, because the captured variable will never be reset
				removeEffect.SetRevertibleEffects(new IRevertEffect[]
					{ effect, new RevertActionEffect(() => { totalDamageTaken = 0f; }), registerReactEffect });

				var initComponent = new InitComponent(false, new IEffect[] { effect, registerReactEffect }, null);
				return new Modifier(id, genId, name, initComponent, null, default(StackComponent), null,
					new SingleTargetComponent(), null);
			}, new ModifierAddData(true, false, false, false));
			Setup();

			Pool.Clear();
			Pool.Allocate(IdManager.GetId("InitStatusEffectSleep_RemoveOnTenDamageTaken"), 1);

			Unit.AddModifierSelf("InitStatusEffectSleep_RemoveOnTenDamageTaken");
			Assert.True(Unit.StatusEffectController.HasStatusEffect(StatusEffectType.Sleep));
			Unit.Update(4);

			Unit.TakeDamage(12, Unit);
			Unit.Update(0);
			Assert.False(Unit.StatusEffectController.HasStatusEffect(StatusEffectType.Sleep));
			Assert.False(Unit.ContainsModifier("InitStatusEffectSleep_RemoveOnTenDamageTaken"));

			Unit.AddModifierSelf("InitStatusEffectSleep_RemoveOnTenDamageTaken");
			Unit.TakeDamage(9, Unit);
			Unit.Update(0);
			Assert.True(Unit.StatusEffectController.HasStatusEffect(StatusEffectType.Sleep));
		}
	}
}