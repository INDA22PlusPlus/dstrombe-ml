using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour {

	public ANN network;
	public GameObject square;
	public List<PixelPaint> pixels = new List<PixelPaint>();
	public List<float> vals = new List<float>();
	public float ui_scale;
	public float brush_size;
	void Start () {
		network = (ANN)FindObjectOfType<ANN>();
		for(int i = 0; i < (28 * 28); i++)
		{
			GameObject g = GameObject.Instantiate(square, transform.position + (new Vector3(i % 28, i / 28, transform.position.z) * ui_scale), transform.rotation);
			g.transform.localScale = Vector3.one * ui_scale;
			g.transform.parent = transform;
			pixels.Add(g.GetComponent<PixelPaint>());
			vals.Add(0);
		}
	}
	
	// Update is called once per frame
	public void UpdatePaint () {
		for(int i = 0; i < pixels.Count; i++)
		{
			vals[i] = pixels[i].active;
		}
		network.userMnist = vals.ToArray();
	}
}
