using System;
using System.Runtime.CompilerServices;

namespace ModiBuff.Core
{
	public enum Targeting
	{
		/// <summary>
		///		Target gets attacked by Source
		/// </summary>
		TargetSource,

		/// <summary>
		///		Source gets attacked by Target
		/// </summary>
		SourceTarget,

		/// <summary>
		///		Target gets attacked by Target
		/// </summary>
		TargetTarget,

		/// <summary>
		///		Source gets attacked by Source
		/// </summary>
		SourceSource,
	}

	public static class TargetingExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void UpdateTargetSource(this Targeting targeting, ref IUnit target, ref IUnit source)
		{
			IUnit finalTarget, finalSource;

			switch (targeting)
			{
				case Targeting.TargetSource:
					finalTarget = target;
					finalSource = source;
					break;
				case Targeting.SourceTarget:
					finalTarget = source;
					finalSource = target;
					break;
				case Targeting.TargetTarget:
					finalTarget = target;
					finalSource = target;
					break;
				case Targeting.SourceSource:
					finalTarget = source;
					finalSource = source;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			target = finalTarget;
			source = finalSource;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void UpdateTarget(this Targeting targeting, ref IUnit target, IUnit source)
		{
			IUnit finalTarget;

			switch (targeting)
			{
				case Targeting.TargetSource:
				case Targeting.TargetTarget:
					finalTarget = target;
					break;
				case Targeting.SourceTarget:
				case Targeting.SourceSource:
					finalTarget = source;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			target = finalTarget;
		}
	}
}