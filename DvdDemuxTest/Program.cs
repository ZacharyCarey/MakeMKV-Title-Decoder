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

Dvd? dvd = Dvd.ParseFolder(animusic);
if (dvd == null) throw new Exception("Failed to parse!");

var ifo = dvd.TitleSets[21];
var pgc = 1; // 6
var angle = 1;
Console.WriteLine($"Selected options: TitleSet={ifo.TitleSet} PGC={pgc} Angle={angle}");

//Print();
if(!ifo.DemuxTitle(output, pgc, angle))
{
    Console.WriteLine("Demux failed");
}

Print(dvd);

static long BCD2Dec(long BCD)
{
    return (BCD / 0x10) * 10 + (BCD % 0x10);
}

static string FormatDuration(long duration)
{
    string csAux;

    if (duration < 0)
        csAux = "Unknown";
    else
    {
        long h = BCD2Dec(duration / (256 * 256 * 256));
        long m = BCD2Dec((duration / (256 * 256)) % 256);
        long s = BCD2Dec((duration / 256) % 256);
        long frames = BCD2Dec((duration % 256) & 0x3f);
        long fps;

        //TODO handle in pgc/VideoAttributes
        switch((duration % 256) & 0xC0)
        {
            case 0xC0: fps = 30; break;
            default: fps = 25; break;
        }

        long ms = (frames * 1000) / fps;

        csAux = $"{h:00}:{m:00}:{s:00}.{ms:000}";
    }
    return csAux;
}
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