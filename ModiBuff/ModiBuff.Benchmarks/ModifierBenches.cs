using BenchmarkDotNet.Attributes;
using ModiBuff.Core;
using ModiBuff.Core.Units;

namespace ModiBuff.Tests
{
	public abstract class ModifierBenches
	{
		protected ModifierIdManager IdManager { get; private set; }
		protected EffectIdManager EffectIdManager { get; private set; }
		protected ModifierRecipes Recipes { get; private set; }
		protected ModifierPool Pool { get; private set; }
		protected ModifierLessEffects Effects { get; private set; }


		[GlobalSetup]
		public virtual void GlobalSetup()
		{
			Config.PoolSize = 1024;

			IdManager = new ModifierIdManager();
			EffectIdManager = new EffectIdManager();
			Recipes = new BenchmarkModifierRecipes(IdManager);
			Pool = new ModifierPool(Recipes.GetGenerators());
			Effects = new ModifierLessEffects(EffectIdManager);
			Effects.Add("InitDamage", new DamageEffect(5));
			Effects.Finish();
		}

		[GlobalCleanup]
		public virtual void OneTimeTearDown()
		{
			Pool.Reset();
			IdManager.Reset();
			Effects.Reset();

			IdManager = null;
			Recipes = null;
			Pool = null;
		}
	}
}