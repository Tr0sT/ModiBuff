using BenchmarkDotNet.Attributes;
using ModiBuff.Core;

namespace ModiBuff.Tests
{
	[MemoryDiagnoser]
	public class BenchNewModifier : ModifierBenches
	{
		private IModifierGenerator _initDamageRecipe;
		private IModifierGenerator _initDoTSeparateDamageRemoveRecipe;
		private IModifierGenerator _intervalDamageStackAddDamageRecipe;

		public override void GlobalSetup()
		{
			base.GlobalSetup();

			_initDamageRecipe = Recipes.GetGenerator("InitDamage");
			_initDoTSeparateDamageRemoveRecipe = Recipes.GetGenerator("InitDoTSeparateDamageRemove");
			_intervalDamageStackAddDamageRecipe = Recipes.GetGenerator("IntervalDamage_StackAddDamage");

			Pool.Clear();
			Pool.SetMaxPoolSize(1_000_000);
		}

		[Benchmark]
		public void BenchNewBasicModifierFromRecipe()
		{
			var modifier = _initDamageRecipe.Create();
		}

		[Benchmark]
		public void BenchNewMediumModifierFromRecipe()
		{
			var modifier = _initDoTSeparateDamageRemoveRecipe.Create();
		}

		[Benchmark]
		public void BenchPooledMediumModifierFromRecipeReturn()
		{
			var modifier = Pool.Rent(_initDoTSeparateDamageRemoveRecipe.Id);
			Pool.Return(modifier);
		}

		//[Benchmark]
		public void BenchPooledFullStateModifierFromRecipeReturn()
		{
			var modifier = Pool.Rent(_intervalDamageStackAddDamageRecipe.Id);
			Pool.Return(modifier);
		}
	}
}