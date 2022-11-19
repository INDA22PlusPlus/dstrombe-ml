using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class Node {
	public Extensions.activation _activation;
	public float[] weights;
	public float bias;
	public float sum;
	public float activatedSum;
	public float unactivated;
	

	public Node(float[] _weights,float _bias,Extensions.activation act)
	 {
		weights = _weights;
		bias = _bias;
		_activation = act;
	}
	public void Compute(Node[] inputs)
	{
		sum = 0;
		for(int i = 0; i < weights.Length; i++)
		{
			sum += (inputs[i].activatedSum * weights[i]);
			
		}
		unactivated = sum + bias;
		activatedSum = Extensions.CalculateActivation(sum + bias, _activation);
	}
	public float derivative(float[] inputs)
	{
		return inputs[0];
	}
	public string debugData()
	{
		string form = "_____\n";
		form += "weights: \n";
		foreach (float f in weights)
		{
			form += f.ToString() + "\n";
		}
		form += "\n";
		form += "bias: " + bias.ToString();
		form += "\nUnactivated: " + unactivated.ToString();
		form += "\nActivated: " + activatedSum.ToString();
		form += "\n";
		return form;
	}
	public static Node operator+(Node arg1, Node arg2)
	{
		float[] outWeights = new float[arg1.weights.Length];
		float outBias  = arg1.bias + arg2.bias;
	
	
		for(int i = 0; i < arg1.weights.Length; i++)
		{
			outWeights[i] = arg1.weights[i] + arg2.weights[i];
			
		}
		Node ret = new Node(outWeights,outBias,arg1._activation);
	
		return ret;
	}
	public static Node operator-(Node arg1, Node arg2)
	{
		float[] outWeights = new float[arg1.weights.Length];
		float outBias  = arg1.bias - arg2.bias;
	
	
		for(int i = 0; i < arg1.weights.Length; i++)
		{
			outWeights[i] = arg1.weights[i] - arg2.weights[i];
			
		}
		Node ret = new Node(outWeights,outBias,arg1._activation);
	
		return ret;
	}
	public static Node operator/(Node arg1, float arg2)
	{
		float[] outWeights = new float[arg1.weights.Length];
		float outBias  = arg1.bias /arg2;
	
		for(int i = 0; i < arg1.weights.Length; i++)
		{
			outWeights[i] = arg1.weights[i] / arg2;
			
		}
		Node ret = new Node(outWeights,outBias,arg1._activation);
	
	
		return ret;
	}
}
