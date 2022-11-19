using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelPaint : MonoBehaviour {
	public SpriteRenderer sr;
	public float active = 0;
	PaintManager parent;
	
	void Start()
	{
		sr = GetComponent<SpriteRenderer>();		
		parent = transform.parent.GetComponent<PaintManager>();
	}
	void OnMouseOver()
	{
		
		if(Input.GetMouseButton(0))
		{
			bool erase = false;
			active = 1;
			if(Input.GetKey(KeyCode.LeftShift))
				erase = true;
			foreach(Transform t in transform.parent) {
				PixelPaint p = t.gameObject.GetComponent<PixelPaint>();
				if(p == null) {
					continue;
				}
				float dist = Vector3.Distance(t.position, transform.position);
				float colorScale = Mathf.Max(0f, Mathf.Min(1f, (parent.brush_size * parent.ui_scale) - dist));
				
				if(dist < (parent.brush_size * parent.ui_scale))
				{
					if(erase)
						p.active = 0;
					else {
						p.active += active * colorScale;
					}
				}
					
				
				Debug.Log(colorScale);
				p.sr.color = new Color(1 - p.active, 1 - p.active, 1 - p.active, 1f);
			}
		}
		
		
		/*
		else if(Input.GetMouseButton(1))
		{
			active = 0;
		}
		*/
		parent.UpdatePaint();
	}
}
