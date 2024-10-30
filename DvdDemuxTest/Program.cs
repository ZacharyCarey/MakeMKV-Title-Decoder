using PgcDemuxLib;

string folder = "C:\\Users\\Zack\\Downloads\\ANIMUSIC_2\\VIDEO_TS";
string file = "VTS_21_0.IFO";
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

app.Demux(output);