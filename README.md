#ML assignment

It's largely derivative of my own earlier work. This is why it is written in unity of all things.

Features visualization of various kinds. Turning on gizmos shows a graph of how the weights look.
It is possible to draw your own digits and have the network classify them.

# Accuracy
It has gotten as high as 94 but the real ceiling is probably like 90% due to low-res sampling, the accuracy sample window consists of 100 samples. 

#Controls (yes, controls):
Escape: Train the network (some labels saying "new text" will need time to refresh)
Space: Cease training (is needed to interact with the editor)
Return: Send your drawn digit for classification to the network
S: Save the trained weights (path is determined automatically)
L: Load the trained weights (I think I purged my own weight files, sorry)

# Ok but where is the ML code?
It's in ANN/Assets/ANN.cs with most helper functions in ANN/Assets/Extensions.cs

# Building
You need a unity editor version of 2021.3.13f1 LTS or later. If you choose a later version and unity prompts you on whether to migrate to the newer version, accept all the warning prompts.

Run build.sh before opening the project.


forgot to upload this until the day after the exercise (saturday) I hope it's fine. Had it out during the exercise!
