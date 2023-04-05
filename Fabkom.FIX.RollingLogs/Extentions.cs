using System.Text;
using System.IO;
using QuickFix;

namespace Fabkom.FIX.RollingLogs
{
    public static class Extentions
    {
        public static void SaveToFile(this StringBuilder sb, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(sb.ToString());
            }
        }

        public static string Normalize(this SessionID _sessionId)
        {
            var sessionId = _sessionId.ToString();
            foreach (char c in Path.GetInvalidFileNameChars())
                sessionId = sessionId.Replace(c, '-');
            while (sessionId.Contains("--"))
                sessionId = sessionId.Replace("--", "-");
            while (sessionId.Contains(" "))
                sessionId = sessionId.Replace(" ", "");
            sessionId = sessionId.TrimEnd(new char[] { ' ', '.' });
            return sessionId;
        }

        public static string Prefix(this SessionID sessionID)
        {
            System.Text.StringBuilder prefix = new System.Text.StringBuilder(sessionID.BeginString)
                .Append('-').Append(sessionID.SenderCompID);
            if (SessionID.IsSet(sessionID.SenderSubID))
                prefix.Append('_').Append(sessionID.SenderSubID);
            if (SessionID.IsSet(sessionID.SenderLocationID))
                prefix.Append('_').Append(sessionID.SenderLocationID);
            prefix.Append('-').Append(sessionID.TargetCompID);
            if (SessionID.IsSet(sessionID.TargetSubID))
                prefix.Append('_').Append(sessionID.TargetSubID);
            if (SessionID.IsSet(sessionID.TargetLocationID))
                prefix.Append('_').Append(sessionID.TargetLocationID);

            if (SessionID.IsSet(sessionID.SessionQualifier))
                prefix.Append('-').Append(sessionID.SessionQualifier);

            return prefix.ToString();
        }
    }
}


//XDocument d = new XDocument
//(
//    //new XComment("This is a comment."),
//    //new XProcessingInstruction("xml-stylesheet", "href='mystyle.css' title='Compact' type='text/css'"),
//    new XElement(xmlns + "nlog",
//        new XAttribute(XNamespace.Xmlns + "xsi", xsi),

//        new XElement("Book",
//            new XElement("Title", "Artifacts of Roman Civilization"),
//            new XElement("Author", "Moreno, Jordao")
//    ),
//    new XElement("Book",
//        new XElement("Title", "Midieval Tools and Implements"),
//        new XElement("Author", "Gazit, Inbar")
//    )
//),
//new XComment("This is another comment.")
//)
//{
//    Declaration = new XDeclaration("1.0", "utf-8", "true")
//};




//XmlDocument xmlDoc = new XmlDocument();
//XmlNode nLogNode = xmlDoc.CreateElement("nlog");

//nLogNode.Attributes.Append(CreateAttribute("xmlns", "http://www.nlog-project.org/schemas/NLog.xsd"));
//nLogNode.Attributes.Append(CreateAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance"));

/*
<nlog autoReload="true" throwExceptions="true" internalLogFile="${basedir}/App_Data/nlog.txt" internalLogLevel="Debug"
internalLogToConsole="true">

<targets>
<!--Useful for debugging-->
<target name="consolelog" type="ColoredConsole"
layout="${date:format=HH\:mm\:ss}|${level}|${stacktrace}|${message}" />
 */
//if (true == nLogInternalLoggerConfig?.UseInternalLogger)
//{
//    if (true == nLogInternalLoggerConfig?.AutoReload)
//        nLogNode.Attributes.Append(CreateAttribute(AUTORELOAD, XMLTRUE));

//    if (true == nLogInternalLoggerConfig?.ThrowExceptions)
//        nLogNode.Attributes.Append(CreateAttribute(THROWEXCEPTIONS, XMLTRUE));

//    if (true == nLogInternalLoggerConfig?.ThrowConfigExceptions)
//        nLogNode.Attributes.Append(CreateAttribute(THROWCONFIGEXCEPTIONS, XMLTRUE));

//    if (false == string.IsNullOrEmpty(nLogInternalLoggerConfig?.LogLevel))
//        nLogNode.Attributes.Append(CreateAttribute(INTERNALLOGLEVEL, nLogInternalLoggerConfig?.LogLevel));
//    else
//        nLogNode.Attributes.Append(CreateAttribute(INTERNALLOGLEVEL, INTERNALLOGLEVEL_TRACE));

//    if (true == nLogInternalLoggerConfig?.LogToConsole)
//        nLogNode.Attributes.Append(CreateAttribute(INTERNALLOGTOCONSOLE, XMLTRUE));

//if (false == string.IsNullOrEmpty(nLogInternalLoggerConfig?.LogFile))
//    nLogNode.Attributes.Append(CreateAttribute(INTERNALLOGFILE, nLogInternalLoggerConfig.LogFile));
//else
//    nLogNode.Attributes.Append(CreateAttribute(INTERNALLOGFILE, "${basedir}/App_Data/nlog-internal.log"));




//internalLogFile = "c:\nlog-internal.txt" internalLogLevel = "Trace"

//XmlNode nLogTargetsNode = xmlDoc.CreateElement("targets");
//XmlNode nLogTargetNode = xmlDoc.CreateElement("target");
//nLogAttribute = xmlDoc.CreateAttribute("xsi:type");
//nLogAttribute.Value = "File";
//nLogTargetNode.Attributes.Append(nLogAttribute);
//nLogAttribute = xmlDoc.CreateAttribute("name");
//nLogAttribute.Value = "file";
//nLogTargetNode.Attributes.Append(nLogAttribute);
//nLogAttribute = xmlDoc.CreateAttribute("fileName");
//nLogAttribute.Value = "log.txt";
//nLogTargetNode.Attributes.Append(nLogAttribute);
//nLogAttribute = xmlDoc.CreateAttribute("archiveFileName");
//nLogAttribute.Value = "log.{#}.txt";
//nLogTargetNode.Attributes.Append(nLogAttribute);
//nLogAttribute = xmlDoc.CreateAttribute("archiveNumbering");
//nLogAttribute.Value = "Date";
//nLogTargetNode.Attributes.Append(nLogAttribute);
//nLogAttribute = xmlDoc.CreateAttribute("archiveEvery");
//nLogAttribute.Value = "Day";
//nLogTargetNode.Attributes.Append(nLogAttribute);
//nLogAttribute = xmlDoc.CreateAttribute("archiveDateFormat");
//nLogAttribute.Value = "yyyyMMdd";
//nLogTargetNode.Attributes.Append(nLogAttribute);
//nLogTargetsNode.AppendChild(nLogTargetNode);
//nLogNode.AppendChild(nLogTargetsNode);

//XmlNode nLogRulesNode = xmlDoc.CreateElement("rules");
//XmlNode nLogRuleNode = xmlDoc.CreateElement("logger");
//nLogAttribute = xmlDoc.CreateAttribute("name");
//nLogAttribute.Value = "*";
//nLogRuleNode.Attributes.Append(nLogAttribute);
//nLogAttribute = xmlDoc.CreateAttribute("minlevel");
//nLogAttribute.Value = "Trace";
//nLogRuleNode.Attributes.Append(nLogAttribute);
//nLogAttribute = xmlDoc.CreateAttribute("writeTo");
//nLogAttribute.Value = "file";
//nLogRuleNode.Attributes.Append(nLogAttribute);
//nLogRulesNode.AppendChild(nLogRuleNode);
//nLogNode.AppendChild(nLogRulesNode);



//           < targets >
//< !--Useful for debugging-- >
//< target name = "consolelog" type = "ColoredConsole"
//layout = "${date:format=HH\:mm\:ss}|${level}|${stacktrace}|${message}" />

//xmlDoc.AppendChild(nLogNode);

//result = XmlDocumentToString(xmlDoc);
//return result;

//System.Xml.XmlAttribute CreateAttribute(string name, string value)
//{
//    var attribute = xmlDoc.CreateAttribute(name);
//    attribute.Value = value;
//    return attribute;
//}


//NLog.LogManager.ThrowExceptions = true;
//NLog.LogManager.ThrowConfigExceptions = true;
//// enable internal logging to the console
//NLog.Common.InternalLogger.LogToConsole = true;

//// enable internal logging to a file
//NLog.Common.InternalLogger.LogFile = @"C:\DEV\Fabkom.MDF.Replay\FIX\Logs\NLog-internal.log";

//// enable internal logging to a custom TextWriter
////NLog.Common.InternalLogger.LogWriter = new StringWriter(); //e.g. TextWriter writer = File.CreateText("C:\\perl.txt")

//// set internal log level
//NLog.Common.InternalLogger.LogLevel = NLog.LogLevel.Debug;


//XmlNode userNode = xmlDoc.CreateElement("user");
//XmlAttribute attribute = xmlDoc.CreateAttribute("age");
//attribute.Value = "42";
//userNode.Attributes.Append(attribute);
//userNode.InnerText = "John Doe";
//rootNode.AppendChild(userNode);

//userNode = xmlDoc.CreateElement("user");
//attribute = xmlDoc.CreateAttribute("age");
//attribute.Value = "39";
//userNode.Attributes.Append(attribute);
//userNode.InnerText = "Jane Doe";
//rootNode.AppendChild(userNode);


//LogManager.Configuration = XmlLoggingConfiguration.CreateFromXmlString(xmlNLogConfig);

/*

 <?xml version="1.0" encoding="utf-8" ?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

<targets>
<target xsi:type="File"
name="file"
fileName="log.txt"
archiveFileName="log.{#}.txt"
archiveNumbering="Date"
archiveEvery="Day"
archiveDateFormat="yyyyMMdd" />
</targets>

<rules>
<logger name="*" minlevel="Trace" writeTo="file" />
</rules>
</nlog>



 * */



//private static string XmlDocumentToString(XmlDocument xmlDoc)
//{
//    string result;
//    using (var stringWriter = new StringWriter())
//    using (var xmlTextWriter = XmlWriter.Create(stringWriter))
//    {
//        xmlDoc.WriteTo(xmlTextWriter);
//        xmlTextWriter.Flush();
//        result = stringWriter.GetStringBuilder().ToString();
//    }
//    return result;
//}

