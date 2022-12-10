using JetBrains.Annotations;

namespace ModifierLibraryLite.Core
{
	/// <summary>
	///		Lower level recipe for a modifier.
	/// </summary>
	public class ModifierInternalRecipe
	{
		public string Id { get; }

		public TargetComponent TargetComponent { get; }
		[CanBeNull] public IInitComponent InitComponent { get; private set; }
		[CanBeNull] public ITimeComponent[] TimeComponents { get; private set; }
		[CanBeNull] public IRefreshComponent RefreshComponent { get; private set; }

		[CanBeNull] public IStackComponent StackComponent { get; private set; }

		//[CanBeNull] public IRemoveComponent RemoveComponent { get; private set; }
		public RemoveEffect RemoveEffect { get; private set; }

		public ModifierInternalRecipe(string id)
		{
			Id = id;
		}

		internal ModifierInternalRecipe(string id, TargetComponent targetComponent, IInitComponent initComponent,
			ITimeComponent[] timeComponents, IRefreshComponent refreshComponent, IStackComponent stackComponent,
			RemoveEffect removeEffect = null)
		{
			Id = id;
			TargetComponent = targetComponent;
			InitComponent = initComponent;
			TimeComponents = timeComponents;
			RefreshComponent = refreshComponent;
			StackComponent = stackComponent;

			RemoveEffect = removeEffect;
		}

		public void SetInitComponent(IInitComponent initComponent)
		{
			InitComponent = initComponent;
			//If init != null, log warning
		}

		public void SetTimeComponents(params ITimeComponent[] timeComponents)
		{
			TimeComponents = timeComponents;
		}

		public void SetStackComponent(IStackComponent stackComponent)
		{
			StackComponent = stackComponent;
		}

		public void SetRefreshable( /*IRefreshComponent refreshComponent*/)
		{
			//TODO
			//RefreshComponent = refreshComponent;
		}
	}
}