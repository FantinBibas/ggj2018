using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ImageEffect : MonoBehaviour
{

	private Camera _camera;
	public Material RenderMaterial;
	
	private void Start ()
	{
		_camera = GetComponent<Camera>();
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, RenderMaterial);
	}
	
}
