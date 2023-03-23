using Fabkom.MDF.UTIL;
using QuickFix;


public class FIXUtils
{
    private static readonly IMessageFactory _defaultMsgFactory = new DefaultMessageFactory();
    private static readonly QuickFix.DataDictionary.DataDictionary dd = new QuickFix.DataDictionary.DataDictionary();

    public static QuickFix.FIX44.MarketDataIncrementalRefresh LoadMarketDataIncrementalRefreshFromString(string rec)
    {
        if (string.IsNullOrEmpty(rec))
            return null;
        rec = FIXUtils.TrimFIX(rec);
        var msg = new QuickFix.FIX44.MarketDataIncrementalRefresh();
        msg.FromString(rec, false, null, dd, _defaultMsgFactory);
        return msg;
    }
    public static void LoadDictionary(string dictFile)
    {
        dd.Load(dictFile);
    }
    private static string TrimFIX(string line)
    {
        if (line.Contains("8=FIXT.1.1"))
            line = line.Right(line.Length - line.IndexOf("8=FIXT.1.1"));
        else if (line.Contains("8=FIX.4.4"))
            line = line.Right(line.Length - line.IndexOf("8=FIX.4.4"));
        return line;
    }

}

