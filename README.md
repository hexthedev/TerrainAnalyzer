# Unity Terrain Analyzer
Unity based Terrain Analyzer that filters maps by strategic regions based on custom attributes 

### General Idea
VIDEO TO COME...

The Terrain Analyzer explained below is a tool created to automatically analyze maps based on characteristics defined by the user. It returns to following:
* A traversability graph representing a grid based map of all traversable points and their connections (For use in pathfinding)
  * SMALL MODIFICATION NEEDED TO IMPLEMENT THIS FEATURE
* An analysis graph, representing strategic regions on the map (For use in AI reasoning, and strategy)
  * Analysis graph includes system for users to write custom attributes by which each node will be analyzed and given a value (For more precise AI reasoning, and different AI strategies)
 
 
 
### Understanding the code
![UML Diagram of Design](/img/01Analyze.png)


*Fig 1: Above is a UML diagram representing the current code. Major scripts' role is listed below*
* Terrain Analysis: Master script that controls the analysis procedure. Responsible for input and output
* Analyzer: Analyzes map returning a traversability map and Veronoi graph (Used to find choke points)
* VoronoiFinalizer: Culls nodes in Veronoi Graph until only open regions and choke points are remaining


The Algorithm works using the following steps:
1. Create a 2D grid representing height values and their positions
2. Make a traversability map representing all movable positions and their connections
3. Make a boarder map representing the outline of the traversability map
4. Using border map, create a Voronoi graph 
5. Prune all edges with non-traversable nodes
6. Compute a radius for all remaining Voronoi graph nodes, representing the nodes distance from an untraversable obstacle. 
7. Use various culling methods to remove all unneeded or unwanted nodes
8. Return a simple analysis graph


Using the Terrain Analyzer
1. Set up a Unity Terrain (DOES NOT WORK ON NONE UNITY TERRAIN MAPS, FEATURE COMING IN LATER VERSIONS)
2. Tweek preferences. Parameter explainations listed below:
  * World Distance
    * Terrain X (TX): The size in unity globalspace units of the terrain along X axis
    * Terrain Y (TY): The size in unity globalspace units of the terrain along Z axis
  * Percentage of Real Space
    * Grid X Percentage (GX): Each X axis grid line will be percent of real X length
    * Grid Y Percentage (GY): Each Y axis grid line will be percent of real X length
  * Map Preferences
    * Max Reachable Height (MRH): Defines a height above which units cannot reach
    * Max Traversable Slope (MTS): Defines max change in height still defined as movable between grid squares
  * Radius Computation
    * Iteration Distance (ID): Defines distance checked per iteration of radius calculation
    * Num of Directions (#D): Defines number of directions circle is broken into for checking radius calculations
  * Pruning Settings
    * Min Node Radius (MNR): Defines the minimum radius that any one node can have without being pruned
    * Min Region Radius(MRR): Defines the minimum region size that two nodes can occupy
    * Max Region Height Difference(MRHD): Defines the maximum height difference two nodes can have an still be considered in the same region
    * Corridor Percentage: When culling corridor nodes, controls possible variance in same corridor node sizes
    * Corridor Constant (CC): Gives a constant variance for corridor size, to stop small corridors retaining nodes. 



### Terrain Analysis Details


#### Step 1: Create a 2D grid representing height values and terrain points
Creates 2D float array of size 100/GX by 100/GY. Grid at position (i,j) represents world space position (i * (TX * GX), j * (TY * GY)). Populates grid using Terrain.sampleHeight(), per grid square. This leaves a grid of height values. 

Due to float number error, a GX and GY percentage must be chosen which always represents numbers of limited digits. Default percentages are 0.25, 0.5, 1 and 2. Smaller number represent denser grids, more accurate map outputs and more computation time. 


#### Step 2: Make a traversability map representing all movable positions and their connections
Cycle nodes creating nodes for each grid square within Max Reachable Height. Create edges between nodes that abide by Max Traversable Slope. (See Fig 3)

![Traversability Map](/img/02TraversabilityGrid.png)
*Fig 2: Complete traversability map of example map based on parameters*


#### Step 3: From traversability map, make a boarder map representing the outline of the movable map
Create a node map made up of nodes that count as border nodes outlining the moveable area. (See Fig 4)

![Border Map](/img/03BorderMap.PNG)
*Fig 3: Border map created on test terrain “Crater” with grid density = 0.5%*


#### Step 4: Using border map, create a Voronoi graph
Code used from https://code.google.com/archive/p/fortune-voronoi/ library created by codeproject user BenDi with Mozilla Public License, v. 2.0

Using the above library, a Voronoi graph is created over all nodes.

Border nodes do not make up a polygon. This method was chosen because Voronoi graphs only work over 2D surfaces, and height is an important aspect in these terrains. Instead the naive Voronoi is generated, then culled using various methods.Edges containing vertices outside of map or at unreachable heights are pruned (See Fig 5)

![Voronoi Graph](/img/04Veronoi.PNG)
*Fig 4: Voronoi graph after non-traversable edges are pruned*


#### Step 5: Compute a radius for all remaining Voronoi graph nodes, representing the nodes distance from an untraversable obstacle.
Iteratively increases radius until an object is hit (See Fig 6)

ID defines how much the radius expands iteration. An ID = 1 would check radius =1,2,3,4,....n until an obstacle is hit or the map bound is passed.

#D tells the radius how many angles to cut the circle into when iterating. #D = 4 would check every angle between 0 and 360 with increments of 90 degrees. 

Stopwatch tests have shown that this is the most costly part of the algorithm.

![Radius Graph](/img/05Radius.PNG)
*Fig 5: Shows radius of every node after computation*


#### Step 6: Use various culling methods to remove all unneeded or unwanted nodes
Systematically remove nodes from the graph using different properties. The right combination of culls is needed to efficiently remove of unimportant nodes. Culling methods are listed below. (See Fig 7)
  * Min Radius Cull: Removes all nodes with radius below certain number. 
  * Largest Nodes First: Starting at node with largest radius, removes all nodes within radius of node.
  * Region Merge: The MRR and MRHD can be tweaked. Region merge looks at each node n and checks if n.neighbours() contains a node within MRR distance. If a node is found, the smaller neighbour is merged into the larger neighbour. 
  * Leaf Prune: Any node with one neighbour is considered a leaf. It’s neighbour is considered the parent. If the parent has a larger radius than the leaf,  then the leaf is merged into the parent. Repeated until no nodes are pruned. 
  * Triangle Cull: For each node, checks both neighbours. If both neighbours are also neighbours, merges smallest node with largest node.
  * Corridor Prune: If a node n has degree = 2, checks the radius of both neighbours. If both neighbours are within n.Radius() * corrdiorPercentage + corridorConstant - n.Radius, then remove n and connect n’s neighbours.
  * Prune Zero Children: (Optional) Nodes can occur that have no children. Especially in regions that are more disconnected and so should only be represented by a single central node. When flying units that can bypass terrain are available this might not be desirable. Otherwise, culling all zero children nodes may be a better option

The order in which culling is performed in the presented algorithm is as follows:
1. Cull Below Radius
2. Largest Nodes First
3. Region Merge
4. Triangle Cull
5. Leaf Prune
6. Corridor Prune
7. Prune Zero Children

![Complete Graph](/img/06Analysis.PNG)
*Fig 6: Example of completely pruned Voronoi Graph*


#### Step 7: Return a simple analysis graph
The final output of the algorithm feeds AI the following code structure (See Fig 8)

IAttribute is an interface allowing anyone using the code to write their own attributes. Attributes calculation code is written in the calculate() function and can represent anything. It must output an number between 0 and 1. Sometimes for unity world calculations, the creation of a calculator class extending MonoBehaviour is necessary since MonoBehaviour is required to interact with unity game objects. 

![Analysis Graph](/img/07Hill.PNG)
*Fig 8: Example attribute graph. Takes same structure as analysis graph, but applies float values as attributes based on IAttribute interface. The above graph is using a custom "hill" attribute which gives each node a value based on it's height compared to it's lowest neighbour.*



