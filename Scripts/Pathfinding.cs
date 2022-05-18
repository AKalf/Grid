using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding {

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 12;


    public static List<IGridTile> GetPath(IGridTile startingNode, IGridTile endNode, IGridTile[,] nodes) {

        List<IGridTile> _openList = new List<IGridTile>(), _closedList = new List<IGridTile>();
        _openList.Clear();
        _closedList.Clear();

        _openList.Add(startingNode);
        for (int w = 0; w < nodes.GetLength(0); w++) {
            for (int h = 0; h < nodes.GetLength(1); h++) {
                IGridTile current = nodes[w, h];
                current.WalkingCost = int.MaxValue;
                current.CameFrom = default;
            }
        }
        startingNode.WalkingCost = 0;
        startingNode.HeuristicCost = CalculateDistanceCost(startingNode, endNode);
        while (_openList.Count > 0) {
            IGridTile current = GetLowestTotalCostNode(_openList);
            if (current.Equals(endNode))
                return CalculatePathResult(endNode);
            _openList.Remove(current);
            _closedList.Add(current);
            foreach (IGridTile neighbour in GetNeigbours(current, nodes)) {
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

    private static List<IGridTile> GetNeigbours(IGridTile target, IGridTile[,] nodes) {
        List<IGridTile> results = new List<IGridTile>();
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


    private static List<IGridTile> CalculatePathResult(IGridTile endNode) {
        List<IGridTile> path = new List<IGridTile>();
        path.Add(endNode);
        IGridTile current = endNode;
        while (current.CameFrom != null) {
            path.Add((IGridTile)current.CameFrom);
            current = (IGridTile)current.CameFrom;
        }
        path.Reverse();
        return path;
    }

    private static int CalculateDistanceCost(IGridTile startNode, IGridTile endNode) {
        int xDistance = (int)Mathf.Abs(startNode.GetPosition.x - endNode.GetPosition.x);
        int yDistance = (int)Mathf.Abs(startNode.GetPosition.y - endNode.GetPosition.y);
        int remaining = (int)Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private static IGridTile GetLowestTotalCostNode(List<IGridTile> nodes) {
        IGridTile result = nodes[0];
        for (int i = 1; i < nodes.Count; i++) {
            if (nodes[i].TotalCost < result.TotalCost)
                result = nodes[i];
        }
        return result;
    }

    public interface IGridTile {
        int W { get; set; }
        int H { get; set; }
        Vector3 GetPosition { get; }
        Vector3 GetSize { get; }
        int WalkingCost { get; set; }
        int HeuristicCost { get; set; }
        int TotalCost { get; }
        bool CanBeNavigated { get; set; }
        IGridTile CameFrom { get; set; }
        GameObject GameObject { get; set; }

    }
}
