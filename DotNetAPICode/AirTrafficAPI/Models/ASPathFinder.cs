namespace AirTrafficAPI.Models
{
    public class AStar
    {
        public List<FlightPath> FindPath(List<List<WeatherBlock>> grid, int startRow, int startCol, int endRow, int endCol)
        {
            // Initialize open and closed lists
            var open = new HashSet<Tuple<int, int,double>>();
            var closed = new HashSet<Tuple<int, int, double>>();

            // Add start node to open list
            open.Add(Tuple.Create(startRow, startCol, grid[startRow][startCol].HeuristicValue));

            // Initialize parent positions for each node
            var parentPositions = new Dictionary<Tuple<int, int>, Tuple<int, int>>();

            while (open.Count > 0)
            {
                // Find node with lowest f score
                Tuple<int, int, double> current = Tuple.Create(-1,-1,999999999999.0);
                foreach (var node in open)
                {
                    if (node.Item3 < current.Item3)
                    {
                        current = node;
                    }
                }

                // Goal check
                if (current.Item1 == endRow && current.Item2 == endCol)
                {
                    // Reconstruct and return path
                    List<FlightPath> path = new List<FlightPath>();
                    while (parentPositions.ContainsKey(Tuple.Create(current.Item1, current.Item2)))
                    {
                        var currentRow = current.Item1;
                        var currentCol = current.Item2;
                        path.Add(new FlightPath(grid[currentRow][currentCol].Latitude, grid[currentRow][currentCol].Longitude,currentRow,currentCol));
                        var currentBlock=grid[parentPositions[Tuple.Create(current.Item1, current.Item2)].Item1][parentPositions[Tuple.Create(current.Item1, current.Item2)].Item2];
                        current = Tuple.Create(parentPositions[Tuple.Create(current.Item1, current.Item2)].Item1, parentPositions[Tuple.Create(current.Item1, current.Item2)].Item2, currentBlock.HeuristicValue);
                    }
                    path.Reverse();
                    return path;
                }

                // Remove current node from open list and add to closed list
                open.Remove(current);
                closed.Add(current);

                // Generate successors
                foreach (var successor in GetSuccessors(grid, current, endRow, endCol))
                {
                    if (closed.Contains(successor))
                        continue;

                    if (!open.Contains(successor))
                    {
                        open.Add(successor);
                        parentPositions[Tuple.Create(successor.Item1, successor.Item2)] = Tuple.Create(current.Item1, current.Item2);
                    }
                    
                }
            }

            // No path found
            return null;
        }

        private List<Tuple<int, int,double>> GetSuccessors(List<List<WeatherBlock>> grid, Tuple<int, int,double> current, int endRow, int endCol)
        {
            List<Tuple<int, int, double>> successors = new List<Tuple<int, int, double>>();
            int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };

            int currentRow = current.Item1;
            int currentCol = current.Item2;

            // Generate successors
            for (int i = 0; i < 8; i++)
            {
                int newRow = currentRow + dr[i];
                int newCol = currentCol + dc[i];

                if (IsValidLocation(grid, newRow, newCol))
                {
                    successors.Add(Tuple.Create(newRow, newCol, grid[newRow][newCol].HeuristicValue));
                }
            }

            return successors;
        }

        private bool IsValidLocation(List<List<WeatherBlock>> grid, int row, int col)
        {
            return row >= 0 && row < grid.Count && col >= 0 && col < grid[row].Count;
        }

        private double CalculateHeuristic(Tuple<int, int> current, int endRow, int endCol)
        {
            // You can use any heuristic calculation method here, like Euclidean distance
            return Math.Sqrt(Math.Pow(current.Item1 - endRow, 2) + Math.Pow(current.Item2 - endCol, 2));
        }
    }
}