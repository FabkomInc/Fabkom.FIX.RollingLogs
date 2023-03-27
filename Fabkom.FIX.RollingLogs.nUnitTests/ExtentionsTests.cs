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


        const string beginString = "FIX4.4";
        const string senderCompID = "SENDER";
        const string senderSubID = "SENDER_SUBID";
        const string senderLocationID = "SENDER_LOCATIONID";
        const string targetCompID = "TARGET";
        const string targetSubID = "TARGET_SUBID";
        const string targetLocationID = "TARGET_LOCATIONID";
        const string sessionQualifier = "SESSIONQUALIFIER";

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
                    new SessionID(beginString, senderCompID, senderSubID, senderLocationID, targetCompID, targetSubID, targetLocationID, sessionQualifier),
                    "FIX4.4-SENDER-SENDER_SUBID-SENDER_LOCATIONID-TARGET-TARGET_SUBID-TARGET_LOCATIONID-SESSIONQUALIFIER"),
                new SessionIDTestValues(
                    new SessionID(beginString, senderCompID, targetCompID),
                    "FIX4.4-SENDER-TARGET"),
                new SessionIDTestValues(
                    new SessionID(beginString, senderCompID, senderSubID, targetCompID, targetSubID),
                    "FIX4.4-SENDER-SENDER_SUBID-TARGET-TARGET_SUBID"),
                new SessionIDTestValues(
                    new SessionID(beginString, senderCompID, senderSubID, senderLocationID, targetCompID, targetSubID, targetLocationID)
                    ,"FIX4.4-SENDER-SENDER_SUBID-SENDER_LOCATIONID-TARGET-TARGET_SUBID-TARGET_LOCATIONID"),
                new SessionIDTestValues(
                    new SessionID(beginString, senderCompID, targetCompID, sessionQualifier),
                    "FIX4.4-SENDER-TARGET-SESSIONQUALIFIER")
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
