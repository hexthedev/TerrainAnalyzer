# Unity Terrain Analyzer
Unity based Terrain Analyzer that filters maps by strategic regions based on custom attributes 

### General Idea
VIDEO TO COME...

The Terrain Analyzer explained below is a tool created to automatically analyze maps based on characteristics defined by the user. It is designed to return the following objects the user:
* A traversability graph representing a grid based map of all traversable points and their connections (For use in pathfinding)
  * SMALL MODIFICATION NEEDED TO IMPLEMENT THIS FEATURE
* An analysis graph, representing strategic regions on the map (For use in AI reasoning, and strategy)
  * Analysis graph includes system for users to write custom attributes by which each node will be analyzed and given a value (For more precise AI reasoning, and different AI strategies)
 
 
### Understanding the code
![UML Diagram of Design](/images/logo.png)
Format: ![Alt Text](IMAGE COMING SOON)

Above is a UML diagram representing the current code. Major scripts' role is listed below
* Terrain Analysis: Master script that controls the analysis procedure. Responsible for input and output
* Analyzer: Analyzes map returning a traversability map and Veronoi graph (Used to find choke points)
* VoronoiFinalizer: Culls nodes in Veronoi Graph until only open regions and choke points are remaining

The Algorithm works using the following steps:
1. Create a 2D grid representing height values and their positions
2. Make a traversability map representing all movable positions and their connections
3. Make a boarder map representing the outline of the movable map
4. Using border map, create a Voronoi graph 
5. Prune all edges with non-traversable nodes
6. Compute a radius for all remaining Voronoi graph nodes, representing the nodes distance from an untraversable obstacle. 
7. Use various culling methods to remove all unneeded or unwanted nodes
8. Return a simple analysis graph


