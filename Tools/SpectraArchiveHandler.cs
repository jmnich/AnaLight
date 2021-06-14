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

            using (var stringWriter = new StringWriter())
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
