using UnityEngine;


namespace Barliesque.InspectorTools
{

	public class SingleSelectionAttribute : PropertyAttribute
	{
		public readonly bool AllowNone = false;

		/// <summary>
		/// Forces a property of an enum type marked with the Flags attribute to use a single-selection dropdown instead of the default multi-selection.
		/// </summary>
		/// <param name="allowNone">Allow none to be selected?</param>
		public SingleSelectionAttribute(bool allowNone = false)
		{
			AllowNone = allowNone;
		}
	}

}
