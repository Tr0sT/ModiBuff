using BenchmarkDotNet.Attributes;
using ModiBuff.Core;

namespace ModiBuff.Tests
{
	[MemoryDiagnoser]
	public class BenchInitialization
	{
		[Benchmark]
		public void BenchSetupRecipes()
		{
			var idManager = new ModifierIdManager();
			var recipes = new TestModifierRecipes(idManager);
			var pool = new ModifierPool(recipes.GetRecipes());
			idManager.Reset();
		}
	}
}