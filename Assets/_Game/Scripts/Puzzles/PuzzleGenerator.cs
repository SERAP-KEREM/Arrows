using System;
using System.Collections.Generic;

namespace ArrowNook.Puzzles
{
    public readonly struct Cell : IEquatable<Cell>
    {
        public readonly int X;
        public readonly int Y;
        public Cell(int x, int y) { X = x; Y = y; }
        public bool Equals(Cell other) => X == other.X && Y == other.Y;
        public override bool Equals(object other) => other is Cell cell && Equals(cell);
        public override int GetHashCode() => unchecked(X * 397 ^ Y);
        public static Cell operator +(Cell a, Cell b) => new Cell(a.X + b.X, a.Y + b.Y);
        public static Cell operator -(Cell a, Cell b) => new Cell(a.X - b.X, a.Y - b.Y);
    }

    public sealed class ArrowPath
    {
        public IReadOnlyList<Cell> Cells { get; }
        public Cell Head => Cells[Cells.Count - 1];
        public Cell Direction => Head - Cells[Cells.Count - 2];
        public ArrowPath(IEnumerable<Cell> cells) => Cells = new List<Cell>(cells).AsReadOnly();
    }

    public sealed class PuzzleDefinition
    {
        public int LevelNumber { get; }
        public int Version { get; }
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyList<ArrowPath> Arrows { get; }
        public bool UsedFallback { get; }

        public PuzzleDefinition(int levelNumber, int version, int width, int height,
            IEnumerable<ArrowPath> arrows, bool usedFallback = false)
        {
            LevelNumber = levelNumber;
            Version = version;
            Width = width;
            Height = height;
            Arrows = new List<ArrowPath>(arrows).AsReadOnly();
            UsedFallback = usedFallback;
        }
    }

    public static class PuzzleGenerator
    {
        public const int CurrentVersion = 1;
        public const int AuthoredLevelCount = 10;
        private static readonly Cell[] Directions =
        {
            new Cell(1, 0), new Cell(0, 1), new Cell(-1, 0), new Cell(0, -1)
        };

        // Version 1 is intentionally frozen: the same level must survive app updates and retries.
        public static PuzzleDefinition Generate(int levelNumber, int version = CurrentVersion)
        {
            if (levelNumber < 1) throw new ArgumentOutOfRangeException(nameof(levelNumber));
            if (version != 1) throw new NotSupportedException("Unsupported puzzle generator version.");
            int difficulty = Math.Min(7, (levelNumber - 1) / 12);
            int width = 7 + difficulty;
            int height = 9 + difficulty;
            var random = new StableRandom(unchecked((uint)levelNumber * 2654435761u ^ 0xA770B001u));
            var occupied = new bool[width, height];
            var arrows = new List<ArrowPath>();
            int usedCells = 0;
            int targetCells = width * height * 4 / 5;
            int attempts = width * height * 40;

            while (attempts-- > 0 && usedCells < targetCells)
            {
                var head = new Cell(random.Next(width), random.Next(height));
                Cell direction = Directions[random.Next(4)];
                Cell previous = head - direction;
                if (!IsInside(previous, width, height) || occupied[head.X, head.Y] ||
                    occupied[previous.X, previous.Y] || !HasOpenRay(head, direction, occupied, width, height))
                    continue;

                var cells = new List<Cell> { head, previous };
                occupied[head.X, head.Y] = true;
                occupied[previous.X, previous.Y] = true;
                int length = 3 + random.Next(5 + difficulty / 2);
                Cell tail = previous;
                while (cells.Count < length)
                {
                    int firstDirection = random.Next(4);
                    bool extended = false;
                    for (int i = 0; i < 4; i++)
                    {
                        Cell next = tail + Directions[(firstDirection + i) % 4];
                        if (!IsInside(next, width, height) || occupied[next.X, next.Y]) continue;
                        occupied[next.X, next.Y] = true;
                        cells.Add(next);
                        tail = next;
                        extended = true;
                        break;
                    }
                    if (!extended) break;
                }

                cells.Reverse();
                arrows.Add(new ArrowPath(cells));
                usedCells += cells.Count;
            }

            // Each new head has an exit past older arrows. Reverse insertion order is a solution.
            var puzzle = new PuzzleDefinition(levelNumber, version, width, height, arrows);
            if (arrows.Count >= 6 && TrySolve(puzzle, out _)) return puzzle;
            return CreateFallback(levelNumber, width, height);
        }

        public static bool TrySolve(PuzzleDefinition puzzle, out int[] solution)
        {
            solution = Array.Empty<int>();
            if (puzzle == null || puzzle.Width < 2 || puzzle.Height < 2 ||
                puzzle.Width > 64 || puzzle.Height > 64 || puzzle.Arrows.Count == 0) return false;
            var occupied = new int[puzzle.Width, puzzle.Height];
            for (int x = 0; x < puzzle.Width; x++)
                for (int y = 0; y < puzzle.Height; y++) occupied[x, y] = -1;

            for (int i = 0; i < puzzle.Arrows.Count; i++)
            {
                ArrowPath arrow = puzzle.Arrows[i];
                if (arrow == null || arrow.Cells.Count < 2) return false;
                for (int c = 0; c < arrow.Cells.Count; c++)
                {
                    Cell cell = arrow.Cells[c];
                    if (!IsInside(cell, puzzle.Width, puzzle.Height) || occupied[cell.X, cell.Y] != -1)
                        return false;
                    if (c > 0)
                    {
                        Cell difference = cell - arrow.Cells[c - 1];
                        if (Math.Abs(difference.X) + Math.Abs(difference.Y) != 1) return false;
                    }
                    occupied[cell.X, cell.Y] = i;
                }
            }

            var removed = new bool[puzzle.Arrows.Count];
            var order = new List<int>();
            while (order.Count < puzzle.Arrows.Count)
            {
                bool progressed = false;
                for (int i = 0; i < puzzle.Arrows.Count; i++)
                {
                    if (removed[i]) continue;
                    ArrowPath arrow = puzzle.Arrows[i];
                    Cell cursor = arrow.Head + arrow.Direction;
                    bool blocked = false;
                    while (IsInside(cursor, puzzle.Width, puzzle.Height))
                    {
                        int occupant = occupied[cursor.X, cursor.Y];
                        if (occupant != -1 && occupant != i) { blocked = true; break; }
                        cursor += arrow.Direction;
                    }
                    if (blocked) continue;
                    foreach (Cell cell in arrow.Cells) occupied[cell.X, cell.Y] = -1;
                    removed[i] = true;
                    order.Add(i);
                    progressed = true;
                }
                if (!progressed) return false;
            }
            solution = order.ToArray();
            return true;
        }

        private static bool HasOpenRay(Cell head, Cell direction, bool[,] occupied, int width, int height)
        {
            Cell cursor = head + direction;
            while (IsInside(cursor, width, height))
            {
                if (occupied[cursor.X, cursor.Y]) return false;
                cursor += direction;
            }
            return true;
        }

        private static bool IsInside(Cell cell, int width, int height) =>
            cell.X >= 0 && cell.Y >= 0 && cell.X < width && cell.Y < height;

        private static PuzzleDefinition CreateFallback(int levelNumber, int width, int height)
        {
            var arrows = new List<ArrowPath>();
            for (int y = 0; y < height; y++)
            {
                var cells = new List<Cell>();
                for (int x = 0; x < width; x++) cells.Add(new Cell(x, y));
                if ((y + levelNumber) % 2 == 0) cells.Reverse();
                arrows.Add(new ArrowPath(cells));
            }
            return new PuzzleDefinition(levelNumber, 1, width, height, arrows, true);
        }

        private struct StableRandom
        {
            private uint _state;
            public StableRandom(uint seed) => _state = seed == 0 ? 1u : seed;
            public int Next(int limit)
            {
                _state ^= _state << 13;
                _state ^= _state >> 17;
                _state ^= _state << 5;
                return (int)(_state % (uint)limit);
            }
        }
    }
}
