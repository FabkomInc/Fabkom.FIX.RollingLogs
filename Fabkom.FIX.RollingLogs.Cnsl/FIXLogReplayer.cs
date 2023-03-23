using QuickFix;
using QuickFix.FIX44;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

internal class FIXLogReplayer
{
    private readonly string FileName = "";
    private readonly string DataDictionary = "";
    public long ReplayedRecordCount = 0;
    public SessionID sessionID;

    public FIXLogReplayer()
    {
        FileName = "";
        DataDictionary = "";
    }

    public FIXLogReplayer(string fileName, string dd)
    {
        FileName = fileName;
        DataDictionary = dd;
    }

    public FIXLogReplayer(string fileName, string dd, SessionID sID)
    {
        FileName = fileName;
        DataDictionary = dd;
        sessionID = sID;
    }
    
    public static Stopwatch watch = new Stopwatch();

    public void ReadFile_AllLines_PLINQ_Convert()
    {
        var lines = File.ReadLines(FileName);
        var sessionLock = new object();
        watch.Start();
        var msg = new MarketDataIncrementalRefresh();
        lines
            .AsParallel()
            .WithDegreeOfParallelism(Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)))
            //.Select(line => line.Contains("35=X"))
            .ForAll(line =>
            {
                Interlocked.Increment(ref ReplayedRecordCount);
                if (line.Contains("35=X"))
                {
                    msg = FIXUtils.LoadMarketDataIncrementalRefreshFromString(line);
                    try
                    {
                        Monitor.Enter(sessionLock);
                        Session.SendToTarget(msg, sessionID);
                    }
                    finally
                    {
                        Monitor.Exit(sessionLock);
                    }
                }
            });

        Console.WriteLine($"==> Replayed: {ReplayedRecordCount}  Total Run Time: {watch.Elapsed:hh\\:mm\\:ss} Speed {(int)(ReplayedRecordCount / ((watch.ElapsedMilliseconds / 1000) + 1))} rec/sec.");
        watch.Stop();
    }
}
