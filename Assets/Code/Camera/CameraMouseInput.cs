// Copyright(C) 2017, Alexander Verbeek

using UnityEngine;
using System.Collections;

/// <summary>
/// Directs mouse input to OrbitCamera
/// </summary>
public class CameraMouseInput : MonoBehaviour
{
	[SerializeField]
	private float sensitivity = 1.0f;

	[SerializeField]
	private OrbitCamera cam;

	private Vector3 prevMousePos;

	void Update()
	{
		const int LeftButton = 0;
		if (Input.GetMouseButton(LeftButton))
		{
			// mouse movement in pixels this frame
			Vector3 mouseDelta = Input.mousePosition - prevMousePos;

			// adjust to screen size
			Vector3 moveDelta = sensitivity * mouseDelta * (360f / Screen.height);

			cam.Move(moveDelta.x, -moveDelta.y);
		}

        float scrollDelta = Input.mouseScrollDelta.y;
        if(scrollDelta > 0) {
            cam.ZoomIn();
        } else if(scrollDelta < 0) {
            cam.ZoomOut();
        }
        


		prevMousePos = Input.mousePosition;
	}
}