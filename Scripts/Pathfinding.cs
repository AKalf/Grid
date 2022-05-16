using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 12;
    public List<TNode> GetPath<TNode>(TNode startingNode, TNode endNode, TNode[,] nodes) where TNode : INode<TNode> {
        List<TNode> _openList = new List<TNode>(), _closedList = new List<TNode>();
        _openList.Clear();
        _closedList.Clear();

        _openList.Add(startingNode);
        for (int w = 0; w < nodes.GetLength(0); w++) {
            for (int h = 0; h < nodes.GetLength(1); h++) {
                TNode current = nodes[w, h];
                current.WalkingCost = int.MaxValue;
                current.CameFrom = default;
            }
        }
        startingNode.WalkingCost = 0;
        startingNode.HeuristicCost = CalculateDistanceCost(startingNode, endNode);
        while (_openList.Count > 0) {
            TNode current = GetLowestTotalCostNode<TNode>(_openList);
            if (current.Equals(endNode))
                return CalculatePathResult(endNode);
            _openList.Remove(current);
            _closedList.Add(current);
            foreach (TNode neighbour in GetNeigbours(current, nodes)) {
                if (_closedList.Contains(neighbour) || neighbour.CanBeNavigated == false)
                    continue;
                int tentativeWalkingCost = current.WalkingCost + CalculateDistanceCost(current, neighbour);
                if (tentativeWalkingCost < neighbour.WalkingCost) {
                    neighbour.CameFrom = current;
                    neighbour.WalkingCost = tentativeWalkingCost;
                    neighbour.HeuristicCost = CalculateDistanceCost(neighbour, endNode);
                    if (_openList.Contains(neighbour) == false)
                        _openList.Add(neighbour);
                }
            }
        }
        return null;
    }
    private List<TNode> GetNeigbours<TNode>(TNode target, TNode[,] nodes) where TNode : INode<TNode> {
        List<TNode> results = new List<TNode>();
        int width = nodes.GetLength(0);
        int height = nodes.GetLength(1);
        if (target.H > 0) {
            results.Add(nodes[target.W, target.H - 1]); // bottom
            if (target.W > 0)
                results.Add(nodes[target.W - 1, target.H - 1]); // bottom left
            if (target.W < width - 1)
                results.Add(nodes[target.W + 1, target.H - 1]); // bottom right
        }
        if (target.H < height - 1) {
            results.Add(nodes[target.W, target.H + 1]); // top;
            if (target.W > 0)
                results.Add(nodes[target.W - 1, target.H + 1]); // top left
            if (target.W < width - 1)
                results.Add(nodes[target.W + 1, target.H + 1]); // top right
        }
        if (target.W < width - 1)
            results.Add(nodes[target.W + 1, target.H]); // right
        if (target.W > 0)
            results.Add(nodes[target.W - 1, target.H]); // left
        return results;
    }

    private List<TNode> CalculatePathResult<TNode>(TNode endNode) where TNode : INode<TNode> {
        List<TNode> path = new List<TNode>();
        path.Add(endNode);
        INode<TNode> current = endNode;
        while (current.CameFrom != null) {
            path.Add((TNode)current.CameFrom);
            current = current.CameFrom;
        }
        path.Reverse();
        return path;
    }
    private int CalculateDistanceCost<TNode>(TNode startNode, TNode endNode) where TNode : INode<TNode> {
        int xDistance = (int)Mathf.Abs(startNode.GetPosition.x - endNode.GetPosition.x);
        int yDistance = (int)Mathf.Abs(startNode.GetPosition.y - endNode.GetPosition.y);
        int remaining = (int)Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }
    private TNode GetLowestTotalCostNode<TNode>(List<TNode> nodes) where TNode : INode<TNode> {
        TNode result = nodes[0];
        for (int i = 1; i < nodes.Count; i++) {
            if (nodes[i].TotalCost < result.TotalCost)
                result = nodes[i];
        }
        return result;
    }
    public interface INode<T> {
        int W { get; set; }
        int H { get; set; }
        Vector3 GetPosition { get; }
        int WalkingCost { get; set; }
        int HeuristicCost { get; set; }
        int TotalCost { get; }
        bool CanBeNavigated { get; set; }
        INode<T> CameFrom { get; set; }
        GameObject gameObject { get; set; }
        T GetNewNode(Vector3 position, Vector3 size);

    }
}
