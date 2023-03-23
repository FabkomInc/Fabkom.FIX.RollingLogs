using NLog.Config;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using QuickFix;
using NLog.Layouts;
using NLog.Targets;
using System.Xml.Linq;
using System.Data;
using QuickFix.Config;

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
    }

    public class NLogInternalLoggerConfig
    {
        public readonly bool UseInternalLogger = false;
        public bool AutoReload { get; set; }
        public bool ThrowExceptions { get; set; }
        public bool ThrowConfigExceptions { get; set; }
        public bool LogToConsole { get; set; }
        public string LogFile { get; set; }
        public string LogLevel { get; set; }
        public NLogInternalLoggerConfig(bool _useInternalLogger)
        {
            UseInternalLogger = _useInternalLogger;
            AutoReload = false;
            ThrowExceptions = false;
            ThrowConfigExceptions = false;
        }
    }

    public static class NLogXMLConfig
    {
        public const string XMLTRUE = "true";
        public const string XMLFALSE = "false";
        public const string AUTORELOAD = "autoReload";
        public const string THROWEXCEPTIONS = "throwExceptions";
        public const string THROWCONFIGEXCEPTIONS = "throwConfigExceptions";
        public const string INTERNALLOGLEVEL = "internalLogLevel";
        public const string INTERNALLOGLEVEL_DEBUG = "Debug";
        public const string INTERNALLOGLEVEL_TRACE = "Trace";
        public const string INTERNALLOGFILE = "internalLogFile";
        public const string INTERNALLOGTOCONSOLE = "internalLogToConsole";


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




        public static string CreateNLogXMLConfigFromFIXSessionSettings
        (
            NLogInternalLoggerConfig nLogInternalLoggerConfig,
            SessionSettings settings
        )
        {




            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<nlog");
            sb.AppendLine("  xmlns =\"http://www.nlog-project.org/schemas/NLog.xsd\"");
            sb.AppendLine("  xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"");

            var internalLoggerConfig = FormatInternalLoggerConfig(nLogInternalLoggerConfig);
            sb.AppendLine(internalLoggerConfig + ">");

            sb.AppendLine("");
            sb.AppendLine("  <targets>");
            sb.AppendLine("    <!-- Useful for debugging -->");
            sb.AppendLine("    <target");
            sb.AppendLine("      xsi:type=\"ColoredConsole\"");
            sb.AppendLine("      name=\"consoleLog\"");
            sb.AppendLine("      layout =\"${date:format=HH\\:mm\\:ss}|${level}|${stacktrace}|${message}\" />");


            var sessions = settings.GetSessions();
            foreach (var sessionID in sessions)
            {
                var prefix = NLogLog.Prefix(sessionID);
                var fileLogPath = settings.Get(sessionID).GetString(SessionSettings.FILE_LOG_PATH);
                var logFileNameMessages = System.IO.Path.Combine(fileLogPath, prefix + ".messages.current.log");
                var logFileNameEvents = System.IO.Path.Combine(fileLogPath, prefix + ".event.current.log");

                sb.AppendLine("    <target");
                sb.AppendLine("      xsi:type=\"File\"");
                sb.AppendLine($"      name=\"{prefix}_Messages\"");
                sb.AppendLine($"      fileName=\"{logFileNameMessages}\"");
                sb.AppendLine("      layout =\"${longdate} ${message}\"/>");

                sb.AppendLine("    <target");
                sb.AppendLine("      xsi:type=\"File\"");
                sb.AppendLine($"      name=\"{prefix}_Events\"");
                sb.AppendLine($"      fileName=\"{logFileNameEvents}\"");
                sb.AppendLine("      layout =\"${longdate} ${message}\"/>");
            }


            sb.AppendLine("  </targets>");
            sb.AppendLine("");
            sb.AppendLine("  <rules>");
            sb.AppendLine("    <logger");
            sb.AppendLine("      name=\"ColoredConsoleLog\"");
            sb.AppendLine("      minlevel=\"Trace\"");
            sb.AppendLine("      maxlevel=\"Fatal\"");
            sb.AppendLine("      writeTo=\"consoleLog\" />");


            foreach (var sessionID in sessions)
            {
                var prefix = NLogLog.Prefix(sessionID);

                sb.AppendLine("    <logger");
                sb.AppendLine($"      name=\"Logger_{prefix}_Messages\"");
                sb.AppendLine("      minlevel=\"Trace\"");
                sb.AppendLine("      maxlevel=\"Fatal\"");
                sb.AppendLine($"      writeTo=\"{prefix}_Messages\" />");

                sb.AppendLine("    <logger");
                sb.AppendLine($"      name=\"Logger_{prefix}_Events\"");
                sb.AppendLine("      minlevel=\"Trace\"");
                sb.AppendLine("      maxlevel=\"Fatal\"");
                sb.AppendLine($"      writeTo=\"{prefix}_Events\" />");
            }
            sb.AppendLine("  </rules>");

            sb.AppendLine("</nlog>");

            sb.SaveToFile(@"C:\DEV\Fabkom.MDF.Replay\NLog\nlog.config.generated");

            return sb.ToString();
            //XNamespace xmlns = "http://www.nlog-project.org/schemas/NLog.xsd";
            //XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

            //XDocument d = new XDocument()
            //{
            //    Declaration = new XDeclaration("1.0", "utf-8", "true")
            //};
            //XElement root = new XElement(xmlns + "nlog");
            //root.Add(new XAttribute(XNamespace.Xmlns + "xsi", xsi));




            //root.Add(new XComment("Useful for debugging"));
            //XElement targets = new XElement("targets");
            //XElement targetConsole = new XElement("target");
            //targetConsole.Add(new XAttribute("name", "consoleLog"));
            //targetConsole.Add(new XAttribute("type", "console"));
            //targetConsole.Add(new XAttribute("layout", @"${date:format=HH\:mm\:ss}|${level}|${stacktrace}|${message}"));
            //targets.Add(targetConsole);
            //root.Add(targets);

            //XElement rules = new XElement("rules");
            //XElement logger_CatchAll = new XElement("logger");
            //logger_CatchAll.Add(new XAttribute("name", "*"));
            //logger_CatchAll.Add(new XAttribute("minlevel", "Trace"));
            //logger_CatchAll.Add(new XAttribute("maxlevel", "Fatal"));
            //logger_CatchAll.Add(new XAttribute("writeTo", "consoleLog"));
            //rules.Add(logger_CatchAll);
            //root.Add(rules);


            //d.Add(root);
            ////Console.WriteLine(d);
            //d.Save(@"C:\DEV\Fabkom.MDF.Replay\FIX\Logs\NLog-Config.xml");
            //result = d.ToString();
            //return result;

        }

        private static string FormatInternalLoggerConfig(NLogInternalLoggerConfig nLogInternalLoggerConfig)
        {
            StringBuilder sb = new StringBuilder();
            if (true == nLogInternalLoggerConfig?.UseInternalLogger)
            {
                if (true == nLogInternalLoggerConfig?.AutoReload)
                    sb.AppendLine($"  {AUTORELOAD}=\"{XMLTRUE}\"");

                if (true == nLogInternalLoggerConfig?.ThrowExceptions)
                    sb.AppendLine($"  {THROWEXCEPTIONS}=\"{XMLTRUE}\"");

                if (true == nLogInternalLoggerConfig?.ThrowConfigExceptions)
                    sb.AppendLine($"  {THROWCONFIGEXCEPTIONS}=\"{XMLTRUE}\"");

                if (false == string.IsNullOrEmpty(nLogInternalLoggerConfig?.LogLevel))
                    sb.AppendLine($"  {INTERNALLOGLEVEL}=\"{nLogInternalLoggerConfig?.LogLevel}\"");
                else
                    sb.AppendLine($"  {INTERNALLOGLEVEL}=\"{INTERNALLOGLEVEL_TRACE}\"");

                if (true == nLogInternalLoggerConfig?.LogToConsole)
                    sb.AppendLine($"  {INTERNALLOGTOCONSOLE}=\"{XMLTRUE}\"");

                if (false == string.IsNullOrEmpty(nLogInternalLoggerConfig?.LogFile))
                    sb.AppendLine($"  {INTERNALLOGFILE}=\"{nLogInternalLoggerConfig.LogFile}\"");
                else
                    sb.AppendLine($"  {INTERNALLOGFILE}=\"{"${basedir}/App_Data/nlog-internal.log"}\"");
            }
            return sb.ToString().Trim(new char[] { '\r', '\n' });
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

