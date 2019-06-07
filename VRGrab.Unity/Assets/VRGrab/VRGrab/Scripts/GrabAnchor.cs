using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable //(WIP)

namespace Barliesque.VRGrab
{
	/// <summary>
	/// A component to refine grabbing of objects.  Multiple anchors can be used to specify a variety of anchor points and orientations.
	/// </summary>
	public class GrabAnchor : MonoBehaviour
	{
		[SerializeField] Hand _hand = Hand.Either;
		public enum Hand
		{
			Left = 1,
			Right = 2,
			Either = 3
		}

		[SerializeField] bool _reorientate;


		//TODO  Implement GrabAnchors
		


		private void OnDrawGizmosSelected()
		{
			//TODO  Draw a wireframe of a hand mesh!
		}

	}
}