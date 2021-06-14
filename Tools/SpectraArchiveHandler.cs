using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnaLight.Containers;
using System.Xml;
using System.Diagnostics;
using System.IO;

namespace AnaLight.Tools
{
    public class SpectraArchiveHandler
    {
        /// <summary>
        /// Loads spectra from XML archive.
        /// </summary>
        /// <param name="pathToArchive">Path to XML archive</param>
        /// <returns>null if failed</returns>
        public static List<BasicSpectraContainer> LoadSpectra(string pathToArchive)
        {
            // validate provided path
            bool isValid;

            try
            {
                string fullPath = Path.GetFullPath(pathToArchive);

                string root = Path.GetPathRoot(pathToArchive);
                isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;

                if (pathToArchive.EndsWith(".xml") == false)
                {
                   isValid = false;
                }
            }
            catch (Exception ex)
            {
                isValid = false;
            }

            if (isValid == false) return null;

            XmlDocument doc;
            try
            {
                doc = new XmlDocument();
                doc.Load(pathToArchive);
            }
            catch(Exception)
            {
                return null;
            }

            try
            {
                List<BasicSpectraContainer> parsedSpectraList = new List<BasicSpectraContainer>();
                XmlNode archive = doc.SelectSingleNode("/Archive");

                foreach (XmlNode spectrum in archive.ChildNodes)
                {
                    string name = spectrum.Attributes["Name"]?.Value ?? "ERROR";
                    string sourceName = spectrum.Attributes["SourceName"]?.Value ?? "ERROR";

                    // make sure time stamp makes any sense at all
                    string ts = spectrum.Attributes["TimeStamp"].Value;
                    DateTime timeStamp;
                    if(ts is string)
                    {
                        try
                        {
                            timeStamp = DateTime.Parse(ts);
                        }
                        catch(Exception)
                        {
                            return null;
                        }
                    } 
                    else
                    {
                        return null;
                    }

                    int ptsCount;
                    if (int.TryParse(spectrum.Attributes["PointsCount"].Value, out ptsCount) == false)
                    {
                        return null;
                    }

                    parsedSpectraList.Add(new BasicSpectraContainer(sourceName, timeStamp)
                    {
                        Name = name,
                        XAxis = new double[ptsCount],
                        YAxis = new double[ptsCount],
                    });

                    // now parse comment
                    XmlNode comment = spectrum.SelectSingleNode("Comment");
                    if ((comment is XmlNode) == false) return null;
                    parsedSpectraList.Last().Comment = comment.InnerText;

                    // and for the last step - validate and parse data points
                    XmlNode dataPoints = spectrum.SelectSingleNode("DataPoints");
                    if ((dataPoints is XmlNode) == false) return null;
                    if (dataPoints.ChildNodes.Count != ptsCount) return null;

                    for(int i = 0; i < ptsCount; i++)
                    {
                        string x = dataPoints.ChildNodes[i].Attributes["X"].Value;
                        string y = dataPoints.ChildNodes[i].Attributes["Y"].Value;

                        if((x is string) && (y is string))
                        {
                            parsedSpectraList.Last().XAxis[i] = double.Parse(x);
                            parsedSpectraList.Last().YAxis[i] = double.Parse(y);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                // if everything went well return the parsed list
                return parsedSpectraList;
            }
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Save a list of spectra to an XML archive.
        /// </summary>
        /// <param name="spectra">list of spectra</param>
        /// <param name="path">path with included file name and .xml extension</param>
        /// <returns>false if saving failed</returns>
        public static bool SaveSpectra(List<BasicSpectraContainer> spectra, string path)
        {
            if (!(spectra is List<BasicSpectraContainer>))
                return false;

            if (spectra.Count < 1)
                return false;

            // Parse everything to XML document
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Archive");
            root.SetAttribute("CreationDate", $"{DateTime.Now:g}");
            root.SetAttribute("SpectraCount", $"{spectra.Count}");
            doc.AppendChild(root);
 
            foreach(BasicSpectraContainer spec in spectra)
            {
                XmlElement spectrumBase = doc.CreateElement("Spectrum");
                spectrumBase.SetAttribute("Name", spec.Name);
                spectrumBase.SetAttribute("SourceName", spec.SourceName);
                spectrumBase.SetAttribute("TimeStamp", $"{spec.TimeStamp:dd.MM.yyyy HH:mm:ss.fff}");
                spectrumBase.SetAttribute("PointsCount", $"{spec.XAxis.Length}");

                XmlElement comment = doc.CreateElement("Comment");
                comment.InnerText = spec.Comment;

                spectrumBase.AppendChild(comment);

                XmlElement data = doc.CreateElement("DataPoints");

                for(int i = 0; i < spec.XAxis.Length; i++)
                {
                    XmlElement point = doc.CreateElement("Point");
                    point.SetAttribute("Index", $"{i}");
                    point.SetAttribute("X", $"{spec.XAxis[i]}");
                    point.SetAttribute("Y", $"{spec.YAxis[i]}");

                    data.AppendChild(point);
                }

                spectrumBase.AppendChild(data);
                root.AppendChild(spectrumBase);
            }

            try
            {
                // parse XML to nice text and save it to file
                File.WriteAllText(path, PrintXML(doc));
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private static string PrintXML(XmlDocument xmlDocument)
        {
            // first, convert XML to an ugly string long enough to 
            // connect Earth to the Moon
            string xml;

            using (var stringWriter = new StringWriterUTF8())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                xmlDocument.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                xml = stringWriter.GetStringBuilder().ToString();
            }

            // now parse this ugly string to a human-readable form
            string result = "";

            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
            XmlDocument document = new XmlDocument();

            try
            {
                // Load the XmlDocument with the XML.
                document.LoadXml(xml);

                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read
                // its contents.
                mStream.Position = 0;

                // Read MemoryStream contents into a StreamReader.
                StreamReader sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                string formattedXml = sReader.ReadToEnd();

                result = formattedXml;
            }
            catch (XmlException)
            {
                // Handle the exception
            }

            mStream.Close();
            writer.Close();

            return result;
        }
    }
}
