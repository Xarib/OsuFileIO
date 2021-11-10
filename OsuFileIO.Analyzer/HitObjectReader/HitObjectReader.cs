using OsuFileIO.HitObject;
using OsuFileIO.OsuFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuFileIO.Analyzer.HitObjectReader;

internal abstract class HitObjectReader<THitObject> where THitObject : IHitObject
{
    protected readonly Difficulty difficulty;
    protected readonly List<TimingPoint> timingPoints;
    protected readonly IReadOnlyList<THitObject> hitObjects;
    protected int indexHitObject;
    protected int indexTimingPoint;

    internal TimingPoint CurrentTimingPoint { get => this.timingPoints[this.indexTimingPoint]; }
    internal THitObject CurrentHitObject { get => this.hitObjects[this.indexHitObject]; }

    private List<(TimingPoint, THitObject)> History { get; init; }

    internal HitObjectReader(Difficulty difficulty, List<TimingPoint> timingPoints, IReadOnlyList<THitObject> hitObjects)
    {
        if (difficulty is null)
            throw new ArgumentNullException(nameof(difficulty));

        if (timingPoints is null)
            throw new ArgumentNullException(nameof(timingPoints));

        if (hitObjects is null)
            throw new ArgumentNullException(nameof(hitObjects));

        if (timingPoints.Count == 0)
            throw new ArgumentException("Map has to have timingPoints");

        if (hitObjects.Count == 0)
            throw new ArgumentException("Map has to have hit objects");

        if (difficulty.CircleSize is null)
            throw new ArgumentNullException(nameof(difficulty.CircleSize));

        this.difficulty = difficulty;
        this.timingPoints = timingPoints;
        this.hitObjects = hitObjects;
        this.History = new List<(TimingPoint, THitObject)>();
    }

    internal abstract bool ReadNext();

    internal TimingPoint GetTimingPointFromOffsetOrNull(int offsetFromCurrent)
    {
        var indexAfterOffset = this.indexTimingPoint + offsetFromCurrent;

        if (indexAfterOffset < 0 || indexAfterOffset >= this.timingPoints.Count)
            return null;

        return this.timingPoints[indexAfterOffset];
    }

    internal THitObject GetHitObjectFromOffsetOrNull(int offsetFromCurrent)
    {
        var indexAfterOffset = this.indexHitObject + offsetFromCurrent;

        if (indexAfterOffset < 0 || indexAfterOffset >= this.hitObjects.Count)
            return default;

        return this.hitObjects[indexAfterOffset];
    }

    internal (TimingPoint timingPoint, THitObject hitObject)? GetHistoryEntryOrNull(int offsetFromCurrent)
    {
        var index = this.History.Count + offsetFromCurrent - 1;

        if (index < 0 || index == this.History.Count)
            return null;

        return this.History[index];
    }

    protected void SetMostCurrentTimingPoint()
    {
        var hasChanged = false;
        while (this.indexTimingPoint < this.timingPoints.Count - 1 && this.CurrentTimingPoint.TimeInMs <= this.CurrentHitObject.TimeInMs)
        {
            this.indexTimingPoint++;
            hasChanged = true;
        }

        if (this.indexTimingPoint == this.timingPoints.Count - 1 && this.CurrentTimingPoint.TimeInMs <= this.CurrentHitObject.TimeInMs)
            return;

        if (hasChanged)
            this.indexTimingPoint--;
    }

    protected void AddCurrentToHistory()
        => this.History.Add((this.CurrentTimingPoint, this.CurrentHitObject));
}
