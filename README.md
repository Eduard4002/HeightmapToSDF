## Heightmap to SDF

This is a tool that can take in any grayscale heightmap and provide 
the **Signed Distance Field**, both inside of the heightmap and outside. The class was created in C# with Unity. However it can also be easy to change to also work outside of Unity.

### Instructions
You can download the file and call the ***GenerateSDF*** function, the class is static, meaning it does not need to be attached to any GameObject to function. Simply pass the input heightmap and a bool whether you want to generate the inside SDF or outside SDF. The function will automatically download the PNG inside a *GeneratedSDF* folder in your project's root directory. 

### Showcase
**Greyscale heightmap**
<img src="https://i.postimg.cc/4NcD7xvc/heightmap-2.png" alt="Alt Text" width="300" height="300">


|SDF Inside<img src="https://i.postimg.cc/65xHPLR7/SDFTrue-55-4082.png" alt="Alt Text" width="300" height="300">  |SDF Outside<img src="https://i.postimg.cc/Dz10xhK4/heightmap-2-outside.png" alt="Alt Text" width="300" height="300"> |
|--|--|

### Computation
Currently for a 1024 x 1024 heightmap, it takes my computer around 6 seconds to generate the SDF. This is because everything is run on the CPU, but I do have plans on converting the script into a compute shader that can run on the GPU. This will bring huge time savings and I will update this repository whenever I get around finishing that. 

### Final words
If anyone knows a way of improving the current algorithm(there is probably alot of things that can be done) you are more than welcome on opening a pull request or simply providing some feedback. Thank you for checking out the repository and good luck!
