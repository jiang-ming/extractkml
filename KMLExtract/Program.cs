using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace KMLExtract
{
     
    class Program
    {
       

        static void Main(string[] args)
        {
            XmlReaderSettings settingsR= new XmlReaderSettings();
            settingsR.IgnoreComments = true;
            settingsR.IgnoreWhitespace = true;
            Placemarks placemarks = new Placemarks();
            using (XmlReader reader = XmlReader.Create(@"C:\temp\SS_Merged2015.kml", settingsR))
            {
                //reader.MoveToContent();
                while(reader.Read())
                {
                    if (reader.Name==Placemarks.XmlName)
                    {
                        placemarks.ReadXml(reader);
                    }
                }
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\temp\SS_Merged2015.csv"))
            foreach (Placemark pm in placemarks.collection)
            {
                file.WriteLine(pm.Name+",");
            }
            Console.WriteLine(placemarks.collection.Count() + "in total!");
            Console.Read();
        }
    }
    
    
    public class Description
    {
        public const string XmlName = "table";
        public string Northing, Easting, Elevation;
        public Photos Photos= new Photos();
        public Description() { }
        public Description(string strDes) { ReadXmlString(strDes); }
        public void ReadXmlString(string strDescription)
        {
            using (XmlReader rDescription = XmlReader.Create(new StringReader(strDescription)))
            {
                bool isEmpty = rDescription.IsEmptyElement;
                rDescription.ReadStartElement();
                if (isEmpty) return;
                while (rDescription.NodeType == XmlNodeType.Element)
                {
                    rDescription.ReadStartElement("tr");
                    rDescription.ReadElementContentAsString("th", "");
                    rDescription.ReadEndElement();


                }
            }
           
        }
        private void ReadXmlDesHead(XmlReader r)
        {
            r.ReadStartElement("tr");

            if (r.ReadElementContentAsString("th", "")=="Coordinates")
            r.ReadEndElement();
        }
    }
    public class Photos
    {
        public const string XmlName = "div";
        public IList<Photo> collection= new List<Photo>();
        public void ReadXml(XmlReader r)
        {
            bool isEmpty = r.IsEmptyElement;
            r.ReadStartElement();
            if (isEmpty) return;
            while(r.NodeType==XmlNodeType.Element)
            {
            }
        }
    }
    public class Photo
    {
        //public
    }
    public class Placemarks
    {
        public const string  NameSpaceXmlns="http://earth.google.com/kml/2.1";
        public const string XmlName = "Folder";

        public IList<Placemark> collection = new List<Placemark>();
        public void ReadXml(XmlReader r)
        {
            bool isEmpty = r.IsEmptyElement;
            r.ReadStartElement();
            if (isEmpty) return;
            while (r.NodeType==XmlNodeType.Element)
            {
                if (r.Name == Placemark.XmlName) collection.Add(new Placemark(r));
                else r.Skip();//skip no Placemark
                Console.WriteLine(collection.Count());
                if (collection.Count()==3808)
                {

                }
            }
            
            r.ReadEndElement();
        }
    }
    public class Placemark
    {
        public const string XmlName = "Placemark";
        public string Name, Coordinate, styleUrl,strDescription;

        public Description Description;
        public Placemark() { }
        public Placemark(XmlReader r) { ReadXml(r); }
        public void ReadXml(XmlReader r)
        {
            r.ReadStartElement();
            Name = r.ReadElementContentAsString("name", Placemarks.NameSpaceXmlns);
            strDescription = r.ReadElementContentAsString("description", Placemarks.NameSpaceXmlns);
            Description =new Description(strDescription);
            r.Read();
            styleUrl = r.ReadElementContentAsString("styleUrl", Placemarks.NameSpaceXmlns);
            r.ReadStartElement("Point");
            Coordinate = r.ReadElementContentAsString("coordinates", Placemarks.NameSpaceXmlns);
            r.ReadEndElement();
            r.ReadEndElement();
        }
    }
}
