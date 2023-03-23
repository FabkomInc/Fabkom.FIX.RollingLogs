using QuickFix;
using System.Collections.Generic;

public class MyQuickFixApp : IApplication
{
    public List<SessionID> sessions_ = new List<SessionID>();

    public bool isLoggedIn = false;
    //public SessionID SessionID { get; set; }

    public void FromApp(Message msg, SessionID sessionID) { }
    public void OnCreate(SessionID sessionID)
    {
        //this.SessionID = sessionID;
        sessions_.Add(sessionID);
    }
    public void OnLogout(SessionID sessionID)
    {
        isLoggedIn = false;
    }
    public void OnLogon(SessionID sessionID)
    {
        isLoggedIn = true;
    }
    public void FromAdmin(Message msg, SessionID sessionID) { }
    public void ToAdmin(Message msg, SessionID sessionID) { }
    public void ToApp(Message msg, SessionID sessionID) { }


    public void SendMarketDataIncrementalRefresh(string _MDReqID)
    {
        var m = new QuickFix.FIX44.MarketDataIncrementalRefresh();
        var _MDIncGrp = new QuickFix.FIX44.MarketDataIncrementalRefresh.NoMDEntriesGroup
        {
            MDUpdateAction = new QuickFix.Fields.MDUpdateAction(QuickFix.Fields.MDUpdateAction.NEW)
        };
        if (_MDReqID != null)
            m.MDReqID = new QuickFix.Fields.MDReqID(_MDReqID);
        m.AddGroup(_MDIncGrp);
        Session.SendToTarget(m, sessions_[0]);
    }
}