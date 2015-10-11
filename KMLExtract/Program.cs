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
                
                while(reader.Read())
                {
                    if (reader.Name==Placemarks.XmlName)
                    {
                        placemarks.ReadXml(reader);
                    }
                }
            }
            int photocount =0;
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\temp\SS_Merged2015.csv"))
            foreach (Placemark pm in placemarks.collection)
            {

                string content = "";
                content += pm.Name;
                if (pm.Description.Photos != null)
                {
                foreach(Photo photo in pm.Description.Photos.collection)
                {
                    photocount += 1;
                    content += ",";
                    content += photo.pn;
                }
                }

                file.WriteLine(content);
            }
            Console.WriteLine(placemarks.collection.Count() + "in total!");
            Console.WriteLine("Photo Number " + photocount + " in total!");
            Console.Read();
        }
    }
    
    
    public class Description
    {
        public const string XmlName = "table";
        public string Northing, Easting, Elevation;
        public Photos Photos;
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
                    ReadXmlDimension(rDescription);
                    Northing = ReadXmlDimension(rDescription);
                    Easting = ReadXmlDimension(rDescription);
                    Elevation = ReadXmlDimension(rDescription); 
                    if (rDescription.Name==Photos.XmlName)
                    {
                        Photos = new Photos(rDescription);
                    }
                }
                rDescription.ReadEndElement();
            }
        }

        private string ReadXmlDimension(XmlReader r)
        {
            r.ReadStartElement("tr");
            string returnValue="";
            string dimension = r.ReadElementContentAsString();
            if (dimension!="Coordinates")
            { 
                returnValue= r.ReadElementContentAsString();
            }
            r.ReadEndElement();
            return returnValue;
        }
    }
    public class Photos
    {
        public const string XmlName = "div";
        public IList<Photo> collection= new List<Photo>();
        public Photos() { }
        public Photos(XmlReader r) { ReadXml(r); }
        public void ReadXml(XmlReader r)
        {
            bool isEmpty = r.IsEmptyElement;
            r.ReadStartElement();
            if (isEmpty) return;
            while(r.NodeType==XmlNodeType.Element)
            {
                if (r.Name == Photo.XmlNamePP) collection.Add(new Photo(r));
                else r.Skip();//skip no Photo
            }
            r.ReadEndElement();
        }
    }
    public class Photo
    {
        public const string XmlNamePP = "tr";
        public const string XmlNamePN = "tr";
        public string pp, pn;
        public Photo() { }
        public Photo(XmlReader r) { ReadXml(r); }
        public void ReadXml(XmlReader r)
        {
            pn = ReadXmlPN(r);
            
            pp = ReadXmlPP(r);
            Console.WriteLine(pn+", "+pp);
        }
        private string ReadXmlPN(XmlReader r)
        {

            string photoName="";
            r.ReadStartElement();
            r.ReadStartElement();
            r.ReadStartElement();
            r.ReadStartElement();
            photoName = r.ReadElementContentAsString();
            r.ReadEndElement();
            r.ReadEndElement();
            r.ReadEndElement();
            r.ReadEndElement();
            return photoName;
        }
        private string ReadXmlPP(XmlReader r)
        {

            string photoPath = "";
            r.ReadStartElement();
            r.ReadStartElement();
            photoPath = r["src"];
            photoPath = photoPath.Substring(8, photoPath.Length - 8);
            r.Read();
            r.ReadEndElement();
            r.ReadEndElement();
            return photoPath;
        }
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
            }
            
            r.ReadEndElement();
        }
    }
    public class Placemark
    {
        public const string XmlName = "Placemark";
        public string Name, Coordinate, styleUrl;
        private string strDescription;
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
