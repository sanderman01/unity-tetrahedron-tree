// Copyright(C) 2017, Alexander Verbeek

using UnityEngine;
using System.Collections;

/// <summary>
/// Defines angle limits as the maximum deviation away from the rotation of this object.
/// (in other words: if the yawlimit is 45, then you can only move up to 45 degrees away from this rotation in both directions. 
/// This means the total angle available would be an angle of 90 degrees)
/// An angle of 180 allows complete freedom of movement on that axis.
/// </summary>
public class FocusPoint : MonoBehaviour
{
	[SerializeField]
	private float yawLimit = 45f;

	[SerializeField]
	private float pitchLimit = 45;

	public float YawLimit { get { return yawLimit; } }
	public float PitchLimit { get { return pitchLimit; } }
}