using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

/// <summary>
/// Encapsulates information for the Walker Tree Layout algorithm (and implements the algorithm).
/// http://www.cs.unc.edu/techreports/89-034.pdf
/// Walker J.Q. (1989) A Node-Positioning Algorithm for General Trees. University of North Carolina at Chapel Hill, USA.
/// </summary>
[DebuggerDisplay("Layout[{Node.ID}:{Node.Type}] Left={LeftSibling.Node.ID} Right={RightSibling.Node.ID}")]
public class FTLayoutInformation
{
    /*****************************************************************************************************/
    /* Enums/Constants
    /*****************************************************************************************************/
    #region Constants

    /// <summary>
    /// Distance between one node and its child (or its parent)
    /// </summary>
    public const int LEVEL_SEPARATION = 200;

    //----------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Width of a node
    /// </summary>
    public const int NODE_WIDTH = 200;

    //----------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Separation between sibling nodes
    /// </summary>
    public const int SIBLING_SEPARATION = 100;

    //----------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Separation between sibling sub-trees
    /// </summary>
    public const int SUBTREE_SEPARATION = 200;

    #endregion Constants

    /*****************************************************************************************************/
    /* Data
    /*****************************************************************************************************/
    #region Data
    #endregion Data

    /*****************************************************************************************************/
    /* Constructors
    /*****************************************************************************************************/
    #region Constructors

    /// <summary>
    /// Main Constructor
    /// </summary>
    /// <param name="node">The node to which this layout info belongs</param>
    public FTLayoutInformation(FTNode node)
    {
        Node = node;
    }

    #endregion Constructors

    /*****************************************************************************************************/
    /* Properties
    /*****************************************************************************************************/
    #region Properties

    public IList<FTNode> Children => Node.Children;
    public FTNode Node { get; private set; }
    public FTNode LeftNeighbour { get; set; }
    public FTNode LeftSibling { get; set; }
    public FTNode Parent => Node.Parent;
    public int PositionModifier { get; set; }
    public int PreliminaryPositionX { get; set; }
    public int PreliminaryPositionY { get; set; }
    public FTNode RightSibling { get; set; }

    #endregion Properties


    /*****************************************************************************************************/
    /* Static Functions
    /*****************************************************************************************************/
    #region Static Functions

    /// <summary>
    /// Gets the leftmost node in this subtree
    /// </summary>
    /// <param name="node"></param>
    /// <param name="level"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    private static FTNode GetLeftmost(FTNode node, int level, int depth)
    {
        if (level >= depth)
        {
            return node;
        }
        else
        {
            if (node.Children.Count == 0)
            {
                return null;
            }

            var rightmost = node.Children.First();
            var leftmost = GetLeftmost(rightmost, level + 1, depth);
            while (leftmost == null && rightmost.LayoutInformation.RightSibling != null)
            {
                rightmost = rightmost.LayoutInformation.RightSibling;
                leftmost = GetLeftmost(rightmost, level + 1, depth);
            }
            return leftmost;
        }
    }

    #endregion Static Functions


    /*****************************************************************************************************/
    /* Functions
    /*****************************************************************************************************/
    #region Functions

    /// <summary>
    /// Generate the layout for this node
    /// </summary>
    public void GenerateLayout()
    {
        Node.ResetLayoutInformation();
        RebuildFamilyTree();

        // First walk
        var previousNodeAtDepth = new List<FTNode>();
        GenerateLayoutFirstWalk(previousNodeAtDepth);

        // Second walk
        GenerateLayoutSecondWalk();

        // Apply offsets
        var leftmostPosition = GetLeftmostPosition();
        if (leftmostPosition < 0)
        {
            Node.ApplyOffsetX(-leftmostPosition);
        }
        Node.ApplyOffset(100, 100);
    }

    //----------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Apportion the positions as part of the first walk
    /// </summary>
    /// <param name="depth"></param>
    private void GenerateLayoutApportion(int depth)
    {
        var leftmost = Children.First();
        var neighbour = leftmost.LayoutInformation.LeftNeighbour;
        int compareDepth = 1;

        while (leftmost != null &&
               neighbour != null)
        {
            int leftModSum        = 0;
            int rightModSum       = 0;
            var ancestorLeftmost  = leftmost?.LayoutInformation;
            var ancestorNeighbour = neighbour?.LayoutInformation;

            for (int i = 0; i < compareDepth; i++)
            {
                ancestorLeftmost  = ancestorLeftmost.Parent?.LayoutInformation;
                ancestorNeighbour = ancestorNeighbour.Parent?.LayoutInformation;
                rightModSum      += ancestorLeftmost?.PositionModifier ?? 0;
                leftModSum       += ancestorNeighbour?.PositionModifier ?? 0;
            }

            int moveDistance = (neighbour?.LayoutInformation.PreliminaryPositionX ?? 0)
                + leftModSum
                + SUBTREE_SEPARATION
                + NODE_WIDTH
                - ((leftmost?.LayoutInformation.PreliminaryPositionX ?? 0) + rightModSum);

            if (moveDistance > 0)
            {
                var tempNode = this.Node;
                int leftSiblings = 0;
                while (tempNode != null && tempNode != ancestorNeighbour.Node)
                {
                    leftSiblings += 1;
                    tempNode = tempNode.LayoutInformation.LeftSibling;
                }

                if (tempNode != null && leftSiblings > 0)
                {
                    int portion = moveDistance / leftSiblings;
                    tempNode = this.Node;
                    while (tempNode != ancestorNeighbour.Node) // Error in original algorithm here: original had it as ==
                    {
                        tempNode.LayoutInformation.PreliminaryPositionX += moveDistance;
                        tempNode.LayoutInformation.PositionModifier     += moveDistance;
                        moveDistance                                    -= portion;
                        tempNode                                         = tempNode.LayoutInformation.LeftSibling;
                    }
                }
            }

            compareDepth += 1;
            if (leftmost != null && leftmost.Children.Count > 0)
            {
                leftmost = leftmost.Children.First();
            }
            else
            {
                leftmost = GetLeftmost(Node, 0, compareDepth);
            }

            if (leftmost != null)
            {
                neighbour = leftmost.LayoutInformation.LeftNeighbour;
            }
        }
    }

    //----------------------------------------------------------------------------------------------------//

    /// <summary>
    /// First pass of the layout algorithm
    /// </summary>
    /// <param name="previousNodeAtDepth"></param>
    /// <param name="depth"></param>
    private void GenerateLayoutFirstWalk(List<FTNode> previousNodeAtDepth, int depth = 0)
    {

        // Figure out left neighbour (could be in a different sub-tree)
        if (previousNodeAtDepth.Count <= depth)
        {
            previousNodeAtDepth.Add(Node);
        }
        else
        {
            LeftNeighbour = previousNodeAtDepth[depth];
            previousNodeAtDepth[depth] = Node;
        }

        // If this is a leaf node...
        if (Children.Count == 0)
        {
            if (LeftSibling != null)
            {
                PreliminaryPositionX = LeftSibling.LayoutInformation.PreliminaryPositionX + SIBLING_SEPARATION + NODE_WIDTH;
            }
            else
            {
                PreliminaryPositionX = 0;
            }
            return;
        }

        // Otherwise, recurse
        foreach (var child in Children)
        {
            child.LayoutInformation.GenerateLayoutFirstWalk(previousNodeAtDepth, depth + 1);
        }

        // Determine midpoint
        var midpoint = ((Children.First()?.LayoutInformation.PreliminaryPositionX ?? 0) + 
                        (Children.Last()?.LayoutInformation.PreliminaryPositionX ?? 0)) 
                        / 2;

        if (LeftSibling != null)
        {
            PreliminaryPositionX = LeftSibling.LayoutInformation.PreliminaryPositionX + SIBLING_SEPARATION + NODE_WIDTH;
            PositionModifier = PreliminaryPositionX - midpoint;
            GenerateLayoutApportion(depth);
        }
        else
        {
            PreliminaryPositionX = midpoint;
        }
    }

    //----------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Second pass of the layout algorithm
    /// </summary>
    /// <param name="depth"></param>
    /// <param name="modifierSum"></param>
    private void GenerateLayoutSecondWalk(int depth = 0, int modifierSum = 0)
    {
        Node.X = PreliminaryPositionX + modifierSum;
        Node.Y = depth * LEVEL_SEPARATION;

        // Recurse
        foreach (var child in Children)
        {
            child.LayoutInformation.GenerateLayoutSecondWalk(depth + 1, modifierSum + PositionModifier);
        }
    }

    //----------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Gets the leftmost position of any descendant in this subtree
    /// </summary>
    /// <returns></returns>
    public float GetLeftmostPosition()
    {
        if (Children.Count > 0)
        {
            float leftest = float.MaxValue;
            foreach (var child in Children)
            {
                var val = child.LayoutInformation.GetLeftmostPosition();
                if (val < leftest)
                {
                    leftest = val;
                }
            }
            return leftest;
        }
        else
        {
            return Node.X;
        }
    }

    //----------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Rebuilds sibling/parent links
    /// </summary>
    private void RebuildFamilyTree()
    {
        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            child.Parent = this.Node;
            if (i > 0)
            {
                child.LayoutInformation.LeftSibling = Children[i - 1];
            }
            else
            {
                child.LayoutInformation.LeftSibling = null;
            }
            if (i < Children.Count - 1)
            {
                child.LayoutInformation.RightSibling = Children[i + 1];
            }
            else
            {
                child.LayoutInformation.RightSibling = null;
            }
            child.LayoutInformation.RebuildFamilyTree();
        }
    }

    //----------------------------------------------------------------------------------------------------//

    /// <summary>
    /// Resets the information (or some of it) for a fresh layout.
    /// </summary>
    public void Reset()
    {
        LeftNeighbour        = null;
        PositionModifier     = 0;
        PreliminaryPositionX = 0;
        PreliminaryPositionY = 0;
    }

    #endregion Functions


    /*****************************************************************************************************/
    /* Events
    /*****************************************************************************************************/
    #region Events
    #endregion Events

}
