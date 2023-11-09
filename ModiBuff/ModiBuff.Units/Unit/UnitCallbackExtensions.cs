namespace ModiBuff.Core.Units
{
	public static class UnitCallbackExtensions
	{
		public static bool CheckCallback<TCallbackType, TCallback>(this Callback<TCallbackType> callbackObject,
			out TCallback callbackOut)
		{
			return callbackObject.Action.CheckCallback(out callbackOut);
		}

		public static bool CheckCallback<TCallback>(this object callbackObject, out TCallback callbackOut)
		{
			if (!(callbackObject is TCallback callback))
			{
				Logger.LogError($"objectDelegate is not of type {nameof(TCallback)}, use named delegates instead.");
				callbackOut = default;
				return false;
			}

			callbackOut = callback;
			return true;
		}
	}
}