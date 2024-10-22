using PgcDemuxLib;

string folder = "F:\\Video\\backup\\ANIMUSIC_2\\VIDEO_TS";
string file = "VTS_21_0.IFO";
string output = "C:\\Users\\Zach\\Downloads\\TEST OUTPUT";

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

app.Demux(output);