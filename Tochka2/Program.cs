using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    

    public static (char, (char, char)) GetNextPositionAndCurrentAction(
        Dictionary<char, List<char>> graph, char currPosition)
    {
        var stack1 = new Stack<char>();
        stack1.Push(currPosition);
        var stack2 = new Stack<char>();
        
        var visited = new HashSet<char>();
        visited.Add(currPosition);
        
        var dictionaryForPath = new Dictionary<char, char>();
        dictionaryForPath.Add(currPosition, '\0');
        var pathForSort = new List<List<char>>();
        while (stack1.Count + stack2.Count > 0)
        {
            while (stack1.Count > 0)
            {
                currPosition = stack1.Pop();
                if (Char.IsUpper(currPosition))
                    pathForSort.Add(GetPathFromDict(dictionaryForPath, currPosition));
                foreach (var nextChar in graph[currPosition])
                {
                    if (visited.Contains(nextChar)) continue;
                    dictionaryForPath[nextChar] = currPosition;
                    visited.Add(nextChar);
                    stack2.Push(nextChar);
                }
            }
            if (pathForSort.Count > 0) break;
            stack1 = stack2;
            stack2 = new Stack<char>();
        }

        return GetNextPositionAndCurrentActionFromListPaths(pathForSort);
    }

    public static List<char> GetPathFromDict(Dictionary<char, char> dictionaryForPath, char end)
    {
        var result = new List<char>();
        var currChar = end;
        while (currChar != '\0')
        {
            result.Add(currChar);
            currChar = dictionaryForPath[currChar];
        }

        return result;
    }

    public static (char, (char, char)) GetNextPositionAndCurrentActionFromListPaths(List<List<char>> pathForSort)
    {
        Comparer<List<char>> comparer = Comparer<List<char>>.Create((list1, list2) =>
        {
            var lastSymbol1 = list1[list1.Count - 1];
            var lastSymbol2 = list2[list2.Count - 1];
            if (lastSymbol1.CompareTo(lastSymbol2) != 0)
                return lastSymbol1.CompareTo(lastSymbol2);
            return list1[1].CompareTo(list2[2]);
        });
        pathForSort.Sort(comparer);
        var pathOnCurrentStep = pathForSort[0];
        var l = pathOnCurrentStep.Count;
        return (pathOnCurrentStep[1], (pathOnCurrentStep[l - 1], pathOnCurrentStep[l - 2]));
    }
    
    public static Dictionary<char, List<char>> CreateGraph(List<(string, string)> edges)
    {
        var result = new Dictionary<char, List<char>>();
        foreach (var edge in edges)
        {
            if (!result.ContainsKey(edge.Item1[0]))
                result.Add(edge.Item1[0], new List<char>());
            result[edge.Item1[0]].Add(edge.Item2[0]);
            if (!result.ContainsKey(edge.Item2[0]))
                result.Add(edge.Item2[0], new List<char>());
            result[edge.Item2[0]].Add(edge.Item1[0]);
        }

        return result;
    }
    
    static int GetAmountSteps(List<(string, string)> edges)
    {
        var amountGateway = 0;
        foreach (var edge in edges)
        {
            if (Char.IsUpper(edge.Item1[0]))
                amountGateway += 1;
            if (Char.IsUpper(edge.Item2[0]))
                amountGateway += 1;
        }
        return amountGateway;
    }
    
    static List<string> Solve(List<(string, string)> edges)
    {
        var result = new List<string>();
        var graph = CreateGraph(edges);
        var amountSteps = GetAmountSteps(edges);
        var currentPositionVirus = 'a';
        for (var i = 0; i <= amountSteps; i++)
        {
            var step = GetNextPositionAndCurrentAction(graph, currentPositionVirus);
            currentPositionVirus = step.Item1;
            var pairForDelete = step.Item2;
            graph[pairForDelete.Item1].Remove(pairForDelete.Item2);
            graph[pairForDelete.Item2].Remove(pairForDelete.Item1);
            Console.WriteLine($"{pairForDelete.Item1}-{pairForDelete.Item2}");
        }
        
        return result;
    }

    static void Main()
    {
        var edges = new List<(string, string)>();
        string line;

        while ((line = Console.ReadLine()) != null)
        {
            line = line.Trim();
            if (!string.IsNullOrEmpty(line))
            {
                var parts = line.Split('-');
                if (parts.Length == 2)
                {
                    edges.Add((parts[0], parts[1]));
                }
            }
        }

        var result = Solve(edges);
        foreach (var edge in result)
        {
            Console.WriteLine(edge);
        }
    }
}