using Fabkom.FIX.RollingLogs;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using NUnit.Framework;
using QuickFix;
using QuickFix.Fields;
using System;

namespace Fabkom.FIX.RollingLogs.nUnitTests
{
    [TestFixture]
    public class ExtentionsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        public const string BEGIN_STRING = "FIX.4.4";
        public const string SENDER_COMPID = "SENDER";
        public const string SENDER_SUBID = "SENDER_SUBID";
        public const string SENDER_LOCATIONID = "SENDER_LOCATIONID";
        public const string TARGET_COMPID = "TARGET";
        public const string TARGET_SUBID = "TARGET_SUBID";
        public const string TARGET_LOCATIONID = "TARGET_LOCATIONID";
        public const string SESSIONQUALIFIER = "SESSIONQUALIFIER";

        class SessionIDTestValues
        {
            public SessionID SessionID { get; set; }
            public string ExpectedNormalizedSessionID { get; set; }
            public string? ReceivedNormalizedSessionID { get; set; }

            public SessionIDTestValues(SessionID sessionID, string normalizedSessionID)
            {
                SessionID = sessionID;
                ExpectedNormalizedSessionID = normalizedSessionID;
            }
        }

        [Test]
        public void NormalizeSessionID_AllStationIDCTors_ExpectedToPass()
        {
            // Arrange
            List<SessionIDTestValues> expected = new List<SessionIDTestValues>
            {
                new SessionIDTestValues(
                    new SessionID(BEGIN_STRING, SENDER_COMPID, SENDER_SUBID, SENDER_LOCATIONID, TARGET_COMPID, TARGET_SUBID, TARGET_LOCATIONID, SESSIONQUALIFIER),
                    "FIX.4.4-SENDER-SENDER_SUBID-SENDER_LOCATIONID-TARGET-TARGET_SUBID-TARGET_LOCATIONID-SESSIONQUALIFIER"),
                new SessionIDTestValues(
                    new SessionID(BEGIN_STRING, SENDER_COMPID, TARGET_COMPID),
                    "FIX.4.4-SENDER-TARGET"),
                new SessionIDTestValues(
                    new SessionID(BEGIN_STRING, SENDER_COMPID, SENDER_SUBID, TARGET_COMPID, TARGET_SUBID),
                    "FIX.4.4-SENDER-SENDER_SUBID-TARGET-TARGET_SUBID"),
                new SessionIDTestValues(
                    new SessionID(BEGIN_STRING, SENDER_COMPID, SENDER_SUBID, SENDER_LOCATIONID, TARGET_COMPID, TARGET_SUBID, TARGET_LOCATIONID)
                    ,"FIX.4.4-SENDER-SENDER_SUBID-SENDER_LOCATIONID-TARGET-TARGET_SUBID-TARGET_LOCATIONID"),
                new SessionIDTestValues(
                    new SessionID(BEGIN_STRING, SENDER_COMPID, TARGET_COMPID, SESSIONQUALIFIER),
                    "FIX.4.4-SENDER-TARGET-SESSIONQUALIFIER")
            };

            // Act
            foreach (var sessionIDTest in expected)
            {
                sessionIDTest.ReceivedNormalizedSessionID = sessionIDTest.SessionID.Normalize();
            }

            // Assert
            Assert.Multiple(() =>
            {
                foreach (var sessionIDTest in expected)
                {
                    Assert.That(sessionIDTest.ExpectedNormalizedSessionID, Is.EqualTo(sessionIDTest.ReceivedNormalizedSessionID));
                }
            });
        }
    }
}
