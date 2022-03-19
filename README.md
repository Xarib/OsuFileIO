![Nuget](https://img.shields.io/nuget/v/OsuFileIO?label=OsuFileIO%20nuget)
![Nuget](https://img.shields.io/nuget/v/OsuFileIO.Analyzer?label=OsuFileIO.Analyzer%20nuget)

# OsuFileIO

Yet another library to parse .osu files. Comes with an analyzer.

## Small example
```csharp
var path = @"/my/path/to/map.osu";

using var reader = new OsuFileReaderBuilder(path).Build();
var beatmap = reader.ReadFile();

if (beatmap is IReadOnlyBeatmap<StdHitObject> stdBeatmap)
{
    var result = stdBeatmap.Analyze();
}
```

## What works

**Parser**
- [x] standard
- [ ] mania
- [ ] taiko
- [ ] cbt

**Analyzer**
- [x] standard
- [ ] mania
- [ ] taiko
- [ ] cbt

**Writer**
- [ ] standard
- [ ] mania
- [ ] taiko
- [ ] cbt
