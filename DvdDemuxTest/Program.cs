using FfmpegInterface;
using PgcDemuxLib;
using PgcDemuxLib.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

string animusic = "C:\\Users\\Zack\\Downloads\\ANIMUSIC_2";
string willywonka = "C:\\Users\\Zack\\Downloads\\WILLY_WONKA";

string folder = "C:\\Users\\Zack\\Downloads\\ANIMUSIC_2\\VIDEO_TS";
//string file = "VTS_21_0.IFO";
string output = "C:\\Users\\Zack\\Downloads\\TestOutput";

Dvd? dvd = Dvd.ParseFolder(willywonka);
if (dvd == null) throw new Exception("Failed to parse!");

if (!dvd.DemuxAll(output))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Failed to demux at least one file.");
    Console.ResetColor();
    return;
}

ffmpeg lib = new();
lib.ExtractFrame(
    Path.Combine(output, "VTS-03_Title-001_Angle-1.VOB"),
    TimeSpan.FromSeconds(0.25),
    Path.Combine(output, "out1.png")
);

//var ifo = dvd.TitleSets[3];
//var pgc = 0;
//var angle = 0;
//Console.WriteLine($"Selected options: Title -> TitleSet={ifo.TitleSet} PGC={pgc} Angle={angle}");
//if (!ifo.DemuxTitle(output, pgc, angle))
//{
//    Console.WriteLine("Demux failed");
//}

//var ifo = dvd.TitleSets[21];
//var pgc = 6;
//Console.WriteLine($"Selected options: Menu -> TitleSet={ifo.TitleSet} PGC={pgc}");
//if (!ifo.DemuxMenu(output, pgc))
//{
//    Console.WriteLine("Demux failed");
//}

Print(dvd);


/*
JsonNode PrintTitles()
{
    JsonArray result = new();
    for (int i = 0; i < app.ifo.TitleProgramChainTable.NumberOfProgramChains; i++)
    {
        JsonObject obj = new();
        result.Add(obj);
        obj["Index"] = i;

        var pgc = app.ifo.TitleProgramChainTable[i];
        obj["Duration"] = FormatDuration(pgc.RawDuration);
        obj["Number of cells"] = pgc.NumberOfCells;
        obj["Number of angles"] = pgc.NumberOfAngles;
    }
    return result;
}

JsonNode PrintMenus()
{
    JsonArray result = new();
    for (int i = 0; i < app.ifo.MenuProgramChainTable.NumberOfLanguageUnits; i++)
    {
        JsonObject luObj = new();
        result.Add(luObj);

        var lu = app.ifo.MenuProgramChainTable.LanguageUnits[i];
        luObj["Index"] = i;
        JsonArray pgcs = new();
        luObj["PGCs"] = pgcs;

        for (int j = 0; j < lu.NumberOfProgramChains; j++)
        {
            var pgcObj = new JsonObject();
            pgcs.Add(pgcObj);

            var pgc = lu.PGCs[j];
            pgcObj["Duration"] = FormatDuration(pgc.RawDuration);
            pgcObj["Number of cells"] = pgc.NumberOfCells;
            pgcObj["Number of angles"] = pgc.NumberOfAngles;
        }
    }

    return result;
}
*/
/*void Print()
{
    /*JsonObject doc = new();

    doc["Menus (Language Unuts)"] = PrintMenus();
    doc["Titles"] = PrintTitles();

    Console.WriteLine(doc.ToJsonString(options));
    var options = new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
    string jsonString = JsonSerializer.Serialize<VtsIfo>(app.ifo, options);
    Console.WriteLine(jsonString);
}*/

void Print<T>(T obj)
{
    var options = new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
    //string jsonString = JsonSerializer.Serialize<T>(obj, options);
    //Console.WriteLine(jsonString);

    string path = Path.Combine(output, "Log.json");
    using var stream = File.Create(path);
    JsonSerializer.Serialize<T>(stream, obj, options);
    stream.Flush();
    Console.WriteLine("Wrote log to " + path);
}