using UnityEngine;

namespace HandsOnVR
{

	/// <summary>
	/// An interface allowing custom joints to be abstracted.
	/// </summary>
	public interface IJoint
	{
		Transform ConnectedTo { get; set; }

	}
}