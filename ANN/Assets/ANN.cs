using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class ANN : MonoBehaviour {

	// tons of global variables. ???
	public Text errorText;
	public Text accText;
	public Text statText;
	public Text merMedQuote;
	public Text thonk;
	public RawImage mnistUI;
	public float trainingLimit;
	int wrongs;
	int rights;
	public float simDelay;
	public float simDelay2;
	public int[] layerConfig; 
	
	int hiddenLayers;
	int hlNeuronCount;
	int inputSize;
	int outputSize;
	public float biasGamma;
	public float learningRate;
	public float hiddenOverride = 1;
	public float sigmaOverride = 1;
	public Node[][] layers;
	public bool dontRender = false;
	int largest;
	public GameObject neuronUI;
	int  merMeid = 0;
	int step = 0;
	public string[] mercy;
	public string[] medic;
	Node[][] template;
	Queue cent;
	GameObject[][] ui;
	public bool renderweights;
	bool hasWritten;
	char[] allowed;
	public float[] userMnist = new float[784];

	// this function is largely irrelevant, and sets up some unity/ui things mainly
	void Start () {
		NetworkConfig();

		cent = new Queue();
		string tmp = Extensions.ReadString("Mercy.txt");
		mercy = tmp.Split(
   		new[] { System.Environment.NewLine },
  		System.StringSplitOptions.None);
		  	string tmp2 = Extensions.ReadString("Medic.txt");
		medic = tmp2.Split(
   		new[] { System.Environment.NewLine },
  		System.StringSplitOptions.None);
		allowed = "abcdefghijklmnopqrstuvwxyzäö ,.!?".ToCharArray();
		
		//return;
		largest = (inputSize > outputSize) ? inputSize : (outputSize > hlNeuronCount) ? outputSize : hlNeuronCount;

		
		InitializeNetwork(false);
	}
	
	void NetworkConfig() {
		hiddenLayers = layerConfig.Length - 2;
		hlNeuronCount = layerConfig[1];
		inputSize = layerConfig[0];
		outputSize = layerConfig[layerConfig.Length - 1];
	}

	// UI keyboard shortcuts
	void Update () {
					
		// Train the model
		if(Input.GetKeyDown(KeyCode.Escape))
			StartCoroutine(MainLoop());
		
		// Save the model
		if(Input.GetKeyDown(KeyCode.S))
		{
			SaveModel();
		}

		// Load the model
		if(Input.GetKeyDown(KeyCode.L))
		{
			LoadModel();
		}

		// Send the drawn input to the model
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Node[][] test = FeedForward(Mathf.RoundToInt(Random.Range(0f,9f)),true, userMnist,false);
		}
	}

	// Initialize the network based on the config
	void InitializeNetwork(bool temp)
	{
		int l = hiddenLayers + 2;
		layers = new Node[l][];
		
		ui = new GameObject[l][];
		for(int i = 0; i < (l); i++)
		{
			if(i > 0 && i < (l-1))
				layers[i] = new Node[layerConfig[i]];
			else
			{
				if(i > 0)
					layers[i] = new Node[outputSize];
				else
					layers[i] = new Node[inputSize];
			}
			if(!dontRender && !temp)
			ui[i] = new GameObject[layers[i].Length];
		}
		template =  layers;
		for(int j = 0; j < layers.Length; j++)
		{
			
			for(int i = 0; i < layers[j].Length; i++)
			{
				layers[j][i] = new Node((j > 0) ? (j > 1 ? Extensions.XavierInit(layers[j-1].Length) : Extensions.XavierInit(layers[j-1].Length)) : new float[0], 0, Extensions.activation.relu);
			//	template[j][i] = new Node((j > 0) ? new float[layers[j-1].Length]: new float[0], 0, Extensions.activation.relu);
				if(j < 1)
				{
					layers[j][i]._activation = Extensions.activation.linear;
						layers[j][i].activatedSum = 0;
				}
				else if ((layers.Length - 1) == j)
				
				layers[j][i]._activation = Extensions.activation.sigmoid;
			//	template[j][i]._activation = layers[j][i]._activation;
				if(!dontRender && !temp && j > 0)
				ui[j][i] = GameObject.Instantiate(neuronUI,new Vector2(j * 3,(i * 2) - (layers[j].Length + j % 2)), transform.rotation);
				
			}
			
			
		}
	//	template = layers;
		drawWeights();

	//	StartCoroutine("MainLoop");
	}
	
	// This is not used. Was in use when the network was a string classifier 
	public void HumanTest(string content)
	{
		if(content.Length < 1)
		{
			Debug.Log("invalid input");
			return;
		}
		int _m = content[0];
		Debug.Log(_m);
		string parsed = content.Substring(2, content.Length-2);
		FeedForward(_m, true, null,content[1] == 'y');
	}

	// The training loop
	IEnumerator MainLoop()
	{
		
		
		Node[][][] gradients = new Node[70][][];
		for(int i = 0; i < gradients.Length; i++)
		{
			merMeid = i % 9;
			//Node[][] _lay = layers;
			gradients[i] = FeedForward(merMeid,false, null, true);
		//	layers = _lay;
		}
		
		Node[][] sigma = CloneNetwork();
		
		for(int i = 0; i < gradients.Length; i++)
		{
			for(int j = 0; j < gradients[i].Length; j++)
			{
				for(int k = 0; k < gradients[i][j].Length; k++)
				{
					sigma[j][k] = sigma[j][k] + (gradients[i][j][k] / 4);
				}
			}		
		}
		
	
		
	//	drawWeights();
		layers = sigma;
		for(int i = 0; i < 16; i++)
		{
			Random.seed += i;
			Node[][] test = FeedForward(Mathf.RoundToInt(Random.Range(0f,9f)),true, null,false);
		}
		if(!hasWritten)
		{
			//SaveModel();
			hasWritten = true;
		}
		//Debug.Log(JsonUtility.ToJson(test[1][0]));
		yield return new WaitForSeconds(simDelay);
		if(!Input.GetKey(KeyCode.Space))
		StartCoroutine("MainLoop");
		
	}
	Node[][] FeedForward(int number, bool refreshUI = false, float[] humanOverride = null,bool backProp = false)
	{
		
		float[] randMnist = new float[784];
		Texture2D img = null;
		randMnist = refreshUI ? Extensions.GetMnistValidation(number, out img) : Extensions.GetMnist(number);
		if(humanOverride != null)
			randMnist = humanOverride;
		if(refreshUI && humanOverride == null)
		{
			merMedQuote.text =("Label: " + number);
			mnistUI.texture = img;
		}
		//	layers[0][0].activatedSum = (float)Mathf.RoundToInt(Random.Range(0f,1f));
		//	layers[0][1].activatedSum = (float)Mathf.RoundToInt(Random.Range(0f,1f));
		
		for(int m = 0; m < layers[0].Length; m++)
		{
			layers[0][m].activatedSum = 0;
		}
		
		for(int k = 0; k < randMnist.Length; k++)
		{
			
			layers[0][(k)].activatedSum = randMnist[k];	
		}
		if(!dontRender && refreshUI && false)
		for(int k = 0; k < layers[0].Length; k++)
			{
				
				ui[0][k].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = layers[0][k].activatedSum.ToString("F3");
				ui[0][k].transform.GetChild(0).GetChild(1).GetComponent<Text>().text = layers[0][k].bias.ToString("F3");
			}
		for(int j = 0; j < (layers.Length - 1); j++)
		{
			for(int i = 0; i < layers[j + 1].Length; i++)
			{
				// The actual forward prop call
				layers[j + 1][i].Compute(layers[j]);
				if(dontRender || !refreshUI)
					continue;

				ui[j +1][i].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = layers[j + 1][i].activatedSum.ToString((j != (layers.Length-2) ?"F3" : "F9"));
				ui[j +1][i].transform.GetChild(0).GetChild(1).GetComponent<Text>().text = layers[j + 1][i].bias.ToString("F3");
				drawWeights();
			}	
		}
		//	bool one = layers[0][0].activatedSum > 0.5f;
		//	bool two = layers[0][0].activatedSum > 0.5f;
		float highest = 0;
		int highestIndex = 0;
		for(int i = 0; i < layers[layers.Length - 1].Length; i++)
		{
			//if(refreshUI) {
			//	Debug.Log("i: " + layers[layers.Length -1][i].activatedSum.ToString());
			//}
			if(layers[layers.Length -1][i].activatedSum > highest)
			{
				highest = layers[layers.Length -1][i].activatedSum;
				highestIndex = i;

			}
		}

		// UI printing logic, refreshUI is to be set when a validation batch is run
		if(refreshUI)
		{
			if(highestIndex == number)
			{
				rights++;
				cent.Enqueue(1);
			}
			else
			{
				wrongs++;
				cent.Enqueue(0);
			}
			if(cent.Count > 100)
				cent.Dequeue();
			
			if(cent.Count >= 100)
			{
				int cc = 0;
				for(int p = 0; p < cent.Count; p++)
					cc += cent.ToArray().Cast<int>().ToArray()[p];
				accText.text = (cc).ToString() + "% accuracy";
				accText.color = (cc <= 50) ? Color.red : Color.green;
				accText.color = (cc == 50) ? new Color(1,1,0) : accText.color;
			}

			thonk.text = "Hypothesis: " +(highestIndex) + " | confidence: " + (highest).ToString("F5");
			
			statText.text = "Wrongs: " + wrongs.ToString() + " Rights: "+ rights.ToString() + " R/W: " + ((float)rights / (float)(rights + wrongs)).ToString("F3");
			float sm = 0;
			for(int i = 0; i < layers[layers.Length -1].Length; i++)
			{
				if(i != highest)
				{
					sm += layers[layers.Length -1][i].activatedSum;
				}
			}	
			sm /= 9;
			float _err = ((1 - sm));
			errorText.text = "error: " + _err.ToString("F6");
			errorText.color = new Color(_err, 1 - _err,0);
			
		}
		// Chain with backprop if told to
		return backProp ? BackPropagate(number,refreshUI) : CloneNetwork();
	}
	public Node[][] BackPropagate(int correct,bool refreshUI = false)
	{
		Node[][] clone = CloneNetwork();
		Node[] lastLayer = layers[clone.Length - 1];
		
		
		float[] votedDeltas = new float[largest];
		float costS = 0;
		for(int j = 0; j < lastLayer.Length; j++)
		{

			float costGradient = -((lastLayer[j].activatedSum - (j == correct ? 1 : 0))) * learningRate;
			float diff = (lastLayer[j].activatedSum - (j == correct ? 1 : 0));
			costS += (diff * diff);
			clone[clone.Length - 1][j].bias = -biasGamma * Extensions.FunctionDerivative((lastLayer[j].unactivated),Extensions.activation.sigmoid)* (lastLayer[j].activatedSum - (j == correct ? 1 : 0) * 2);
			Node[] penultimate = clone[clone.Length - 2];
			for(int k = 0; k < penultimate.Length; k++)
			{
				clone[clone.Length - 1][j].weights[k] = -learningRate * penultimate[k].activatedSum * Extensions.FunctionDerivative((lastLayer[j].unactivated),Extensions.activation.sigmoid)* (lastLayer[j].activatedSum - (j == correct ? 1 : 0) * 2) ;//* ((float)penultimate.Length / lastLayer.Length);
				votedDeltas[k] += learningRate * layers[clone.Length - 1][j].weights[k] * Extensions.FunctionDerivative((lastLayer[j].unactivated),Extensions.activation.sigmoid)* (lastLayer[j].activatedSum - (j == correct ? 1 : 0) * 2);
			}
			for(int k = 0; k < penultimate.Length; k++)
			{
			//	votedDeltas[k] /= (float)penultimate.Length;
			}
			//Debug.Log(costGradient);
		}
		
		int currIt = clone.Length - 2;
		
		while(currIt > 0)
		{
			float[] thisVote = votedDeltas;
		
			Node[] currLay = clone[currIt];
			for(int j = 0; j < currLay.Length; j++)
			{	
				float errAbs = Mathf.Abs(thisVote[j]);
				float errPrime = thisVote[j] * learningRate;
				float costGradient = thisVote[j];
		//		thisVote[j] = thisVote[j] /
				clone[currIt][j].bias = -biasGamma * learningRate * Extensions.FunctionDerivative(layers[currIt][j].unactivated, Extensions.activation.relu) * 2 * thisVote[j];
				Node[] penultimate = clone[currIt - 1];
				for(int k = 0; k < penultimate.Length; k++)
				{
					clone[currIt][j].weights[k] = -penultimate[k].activatedSum * learningRate * Extensions.FunctionDerivative(layers[currIt][j].unactivated,Extensions.activation.relu) * 2 * thisVote[j]; //* ((float)penultimate.Length / clone[currIt].Length);
					votedDeltas[k] += clone[currIt][j].weights[k] * learningRate * Extensions.FunctionDerivative(layers[currIt][j].unactivated,Extensions.activation.relu) * 2 * thisVote[j];
					// (clone[clone.Length - 1].Length / clone[clone.Length - 1][j].weights.Length)
					//yield return new WaitForSeconds(simDelay);
					
				}
				for(int k = 0; k < penultimate.Length; k++)
				{
				//	votedDeltas[k] /= (float)penultimate.Length;
				}
				//Debug.Log(costGradient);
			}
			currIt--;
		}
		
		return clone;
		
	}
	void drawWeights()
	{
		if(dontRender || !renderweights)
		return;
		for(int j = 1; j < layers.Length; j++)
		{
			for(int i = 0; i < layers[j].Length; i++)
			{
				
				for(int k = 0; k < layers[j-1].Length; k++)
				{
					
					float weight = ((true) ? layers[j][i].weights[k] : layers[j][i].weights[k]);
					float w2 =  Mathf.Abs(layers[j][i].weights[k]) * 0.5f;
					
					int col = (weight < 0) ? -1 : 1;
					if(j > 1)
					{
						Vector3 dir = ui[j][i].transform.position - ui[j-1][k].transform.position;
						Debug.DrawLine(ui[j][i].transform.position,ui[j-1][k].transform.position,new Color((weight < 0f) ? w2 : 0,(weight > 0f) ? w2 : 0,w2 * 0.025f),simDelay);
					}
					else
					{
				//		ui[j - 1][i].GetComponent<SpriteRenderer>().color = new Color((weight < 0f) ? w2 : 0,(weight > 0f) ? w2 : 0,w2 * 0.025f);
					}
				}
			}
		}
	}
	Node[][] CloneNetwork()
	{
		int p = layers.Length;
		Node[][] clone = new Node[p][];
		for(int i = 0; i < clone.Length; i++)
		{
			int t = layers[i].Length;
			clone[i] = new Node[t];
			for(int j = 0; j < t; j++)
			{
			
				clone[i][j] = Extensions.DeepClone<Node>(layers[i][j]);
			}
		}
		return clone;
	}
	public void SaveModel()
	{
		List<string> nodes = new List<string>();
		for(int i = 0; i < layers.Length; i++)
		{
			nodes.Clear();
			
			foreach(Node n in layers[i])
			{
				nodes.Add(JsonUtility.ToJson(n));
			}
						
			System.IO.File.WriteAllLines(Application.persistentDataPath + i + ".txt",nodes.ToArray());
			
		}
		
		Debug.Log("Saved.");
		
	}
	public void LoadModel()
	{
		
		Node[][] ret = CloneNetwork();
		for(int i = 0; i < ret.Length; i++)
		{
			
			string[] read = System.IO.File.ReadAllLines(Application.persistentDataPath + i + ".txt");
			for(int j = 0; j < read.Length; j++)
			{
				layers[i][j] = JsonUtility.FromJson<Node>(read[j]);
			}
						
			
			
		}
		Debug.Log("Loaded.");
		
		
	}
}
