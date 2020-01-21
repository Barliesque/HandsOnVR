using UnityEngine;

namespace HandsOnVR
{

	/// <summary>
	/// An interface allowing joints to be paired with the PropAttacher component.
	/// </summary>
	public interface IJoint
	{
		Transform ConnectedTo { get; set; }

	}
}