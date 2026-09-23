using ArrowNook.Puzzles;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

static string Fingerprint(PuzzleDefinition puzzle)
{
    var text = new StringBuilder($"{puzzle.Version}:{puzzle.Width}:{puzzle.Height};");
    foreach (var arrow in puzzle.Arrows)
    {
        foreach (var cell in arrow.Cells) text.Append($"{cell.X},{cell.Y};");
        text.Append('|');
    }
    return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString())));
}

static void Expect(bool value, string message)
{
    if (!value) throw new Exception(message);
}

var watch = Stopwatch.StartNew();
Expect(Fingerprint(PuzzleGenerator.Generate(11)) == "4BFD25AF73B7EE080F4471E19E8BEE4A1530119B1734EB17ADC652793FA800DA",
    "Version 1 changed: existing saves must keep their boards after an update.");
var fingerprints = new HashSet<string>();
int fallbackCount = 0;
int minimumArrows = int.MaxValue;
for (int level = 1; level <= 10000; level++)
{
    PuzzleDefinition puzzle = PuzzleGenerator.Generate(level);
    Expect(PuzzleGenerator.TrySolve(puzzle, out var solution), $"Unsolvable level {level}");
    Expect(solution.Distinct().Count() == puzzle.Arrows.Count, $"Incomplete solution for level {level}");
    Expect(puzzle.Arrows.Count >= 6, $"Insufficient content at level {level}");
    string fingerprint = Fingerprint(puzzle);
    Expect(fingerprint == Fingerprint(PuzzleGenerator.Generate(level)), $"Non-deterministic level {level}");
    Expect(fingerprints.Add(fingerprint), $"Duplicate board at level {level}");
    minimumArrows = Math.Min(minimumArrows, puzzle.Arrows.Count);
    if (puzzle.UsedFallback) fallbackCount++;
}

foreach (int level in new[] { 10001, 100000, 1000000, int.MaxValue - 1, int.MaxValue })
    Expect(PuzzleGenerator.TrySolve(PuzzleGenerator.Generate(level), out _), $"Large seed failed: {level}");

var deadlock = new PuzzleDefinition(1, 1, 4, 2, new[]
{
    new ArrowPath(new[] { new Cell(0, 0), new Cell(1, 0) }),
    new ArrowPath(new[] { new Cell(3, 0), new Cell(2, 0) })
});
Expect(!PuzzleGenerator.TrySolve(deadlock, out _), "Validator accepted a deadlock.");
var overlap = new PuzzleDefinition(1, 1, 4, 2, new[]
{
    new ArrowPath(new[] { new Cell(0, 0), new Cell(1, 0) }),
    new ArrowPath(new[] { new Cell(1, 0), new Cell(2, 0) })
});
Expect(!PuzzleGenerator.TrySolve(overlap, out _), "Validator accepted overlapping arrows.");
var jump = new PuzzleDefinition(1, 1, 4, 2, new[] { new ArrowPath(new[] { new Cell(0, 0), new Cell(3, 0) }) });
Expect(!PuzzleGenerator.TrySolve(jump, out _), "Validator accepted a discontinuous arrow.");

try { PuzzleGenerator.Generate(0); throw new Exception("Zero seed accepted."); }
catch (ArgumentOutOfRangeException) { }
try { PuzzleGenerator.Generate(11, 2); throw new Exception("Unknown version accepted."); }
catch (NotSupportedException) { }

Console.WriteLine($"PASS: 10,000 unique, solvable, deterministic boards; {fallbackCount} fallbacks; minimum {minimumArrows} arrows.");
Console.WriteLine($"PASS: large seeds, overlap/deadlock/path validation, version bounds ({watch.ElapsedMilliseconds} ms).");
Console.WriteLine($"Version 1 level 11 fingerprint: {Fingerprint(PuzzleGenerator.Generate(11))}");

Expect(!BannerPolicy.IsEligible(3, 500, 360), "Tutorial ads were allowed.");
Expect(!BannerPolicy.IsEligible(4, 89.9, 360), "Warmup was skipped.");
Expect(!BannerPolicy.IsEligible(4, 90, 319), "Banner would overflow a narrow viewport.");
Expect(BannerPolicy.IsEligible(4, 90, 320), "Eligible banner was rejected.");
Expect(BannerPolicy.RetryDelaySeconds(1) == 60 && BannerPolicy.RetryDelaySeconds(2) == 120 &&
    BannerPolicy.RetryDelaySeconds(20) == 300, "Ad retry backoff changed.");
Console.WriteLine("PASS: banner eligibility, narrow-screen protection and retry backoff.");
