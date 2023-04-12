using System;
using UnityEngine;

public class UVAnimation : MonoBehaviour
{
	private void Start()
	{
		this.renderer = base.GetComponent<Renderer>();
	}

	private void Update()
	{
		this.index = (int)(Time.time * (float)this.fps);
		this.index %= this.uvTileY * this.uvTileX;
		this.size = new Vector2(1f / (float)this.uvTileX, 1f / (float)this.uvTileY);
		int num = this.index % this.uvTileX;
		int num2 = this.index / this.uvTileX;
		this.offset = new Vector2((float)num * this.size.x, 1f - this.size.y - (float)num2 * this.size.y);
		this.renderer.material.SetTextureOffset("_MainTex", this.offset);
		this.renderer.material.SetTextureScale("_MainTex", this.size);
	}

	public int uvTileY = 4;

	public int uvTileX = 4;

	public int fps = 30;

	private int index;

	private Vector2 size;

	private Vector2 offset;

	private Renderer renderer;
}
