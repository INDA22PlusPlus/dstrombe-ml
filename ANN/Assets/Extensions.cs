using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class Extensions  {
	public enum activation {relu,sigmoid,leaky,linear};
	
	public static float CalculateActivation(float input, activation func)
	{
		string think = "";
		switch((int)func)
		{
			case 0:
				return ((input > 0) ? input : 0);
			break;
			case 1:
				return (1 / (1 + Mathf.Exp(-input))); 
				break;
			case 2:
				return ((input > 0) ? input : (input * 0.01f));
				break;
			case 3:
				return input;
				
				break;
			default:
			return -1f;
			break;

		}
		
	}
	public static float FunctionDerivative(float input, activation func)
	{
		switch((int)func)
		{
			case 0:
				
				return ((input > 0) ? 1 : 0);
			break;
			case 1:
				return ((1 / (1 + Mathf.Exp(-input))) *(1 - (1 / (1 + Mathf.Exp(-input))))); //implement!
				break;
			case 2:
				return ((input > 0) ? 1 : (0.01f));
				break;
			case 3:
				return input;
				break;
			default:
			return -1f;
			break;

		}
	}
	public static T DeepClone<T>(T obj)
	{
 		using (var ms = new MemoryStream())
 		{
 		  var formatter = new BinaryFormatter();
 		  formatter.Serialize(ms, obj);
 		  ms.Position = 0;

 		  return (T) formatter.Deserialize(ms);
 	}
}
	public static float[] XavierInit(int previousLength)
	{
		float[] xav = new float[previousLength];
	    float dispersion = (2f / previousLength);
		for(int i = 0; i < previousLength; i++)
		{
			xav[i] = Random.Range(-dispersion, dispersion);
		}
		return xav;
	}
	public static float[] PropInit(int previousLength)
	{
		float[] xav = new float[previousLength];
	    float dispersion = (2f / previousLength);
		for(int i = 0; i < previousLength; i++)
		{
			xav[i] = Random.Range(-dispersion, dispersion);
		}
		return xav;
	}
	public static string ReadString(string fileName)
    {
        string path = "Assets/Resources/" + fileName;

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path,System.Text.Encoding.Default); 
        string tmp = reader.ReadToEnd();
        reader.Close();
		return tmp;
    }
	public static float[] GetImageData(int number)
	{
		string path = "Assets/Resources/trainingSet/" + number.ToString();
		return new float[0];
	}
	public static float[] GetMnist(int number)
	{
		Texture2D[] all = Resources.LoadAll<Texture2D>("trainingSet/" + number.ToString());
		Texture2D test = all[Mathf.RoundToInt(Random.Range(0f,all.Length - 2))];
		Color[] c = test.GetPixels();
		float[] ret = new float[784];
		for(int i = 0; i < 784; i++)
		{
			ret[i] = c[i].r;
		}
		return ret;
	}
	
	public static float[] GetMnistValidation(int number, out Texture2D tx2D)
	{
		Texture2D[] all = Resources.LoadAll<Texture2D>("testSample/" + number.ToString());
		Texture2D test = all[Mathf.RoundToInt(Random.Range(0f,all.Length - 2))];
		Color[] c = test.GetPixels();
		float[] ret = new float[784];
		for(int i = 0; i < 784; i++)
		{
			ret[i] = c[i].r;
		}
		tx2D = test;
		return ret;
	}
	public static Texture2D GetMnistValImage(int number)
	{
		Texture2D[] all = Resources.LoadAll<Texture2D>("testSample/" + number.ToString());
		Texture2D test = all[Mathf.RoundToInt(Random.Range(0f,all.Length - 2))];
	
		return test;
	}
}
