using PgcDemuxLib;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;

string folder = "C:\\Users\\Zack\\Downloads\\ANIMUSIC_2\\VIDEO_TS";
string file = "VTS_03_0.IFO";
string output = "C:\\Users\\Zack\\Downloads\\TestOutput";

IfoOptions options = new();
options.PGC = 1; // 6
options.Angle = 1;
options.ExportVOB = true;
options.DomainType = DemuxingDomain.Titles; // Menus
var app = PgcDemux.TryOpenFile(Path.Combine(folder, file), options);
if (app == null)
{
    Console.WriteLine("Failed to read IFO file.");
    return;
}

Print();
app.Demux(output);

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
void Print()
{
    /*JsonObject doc = new();

    doc["Menus (Language Unuts)"] = PrintMenus();
    doc["Titles"] = PrintTitles();

    Console.WriteLine(doc.ToJsonString(options));*/
    var options = new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
    string jsonString = JsonSerializer.Serialize<Ifo>(app.ifo, options);
    Console.WriteLine(jsonString);
}