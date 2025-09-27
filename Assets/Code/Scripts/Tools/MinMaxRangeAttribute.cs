// https://github.com/matheusamazonas/min_max_range_attribute
using System;
using UnityEngine;

namespace Code.Scripts.Tools
{
	/// <summary>
	/// An attribute that simplifies defining bounded ranges (ranges with minimum and maximum limits) on the inspector.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class MinMaxRangeAttribute : PropertyAttribute
	{
		public readonly float minLimit;
		public readonly float maxLimit;
		public readonly uint decimals;

		/// <summary>
		/// A bounded range for integers.
		/// </summary>
		/// <param name="minLimit">The minimum acceptable value.</param>
		/// <param name="maxLimit">The maximum acceptable value.</param>
		public MinMaxRangeAttribute(int minLimit, int maxLimit)
		{
			this.minLimit = minLimit;
			this.maxLimit = maxLimit;
		}

		/// <summary>
		/// A bounded range for floats.
		/// </summary>
		/// <param name="minLimit">The minimum acceptable value.</param>
		/// <param name="maxLimit">The maximum acceptable value.</param>
		/// <param name="decimals">How many decimals the inspector labels should display. Values must be in the [0,3]
		/// range. Default is 1.</param>
		public MinMaxRangeAttribute(float minLimit, float maxLimit, uint decimals = 1)
		{
			this.minLimit = minLimit;
			this.maxLimit = maxLimit;
			this.decimals = decimals;
		}
	}
}