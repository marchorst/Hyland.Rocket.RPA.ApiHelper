namespace Hyland.Rocket.RPA.ApiHelper
{
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;

    public static class RpaHelper
    {
        public static string SerializeDataTable(DataTable table)
        {
            return Serialize<DataTable>(table);
        }

        public static string Serialize<T>(T table)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringWriter textWriter = new StringWriter();
            serializer.Serialize(textWriter, table);
            return textWriter.ToString();
        }

        /// <summary>
        /// Converts an object to a json string
        /// </summary>
        /// <param name="o">Object</param>
        /// <returns>JSON String</returns>
        public static string ToJson(object o)
        {
            JsonSerializer serializer = JsonSerializer.Create();
            var strBuild = new StringBuilder();
            StringWriter sw = new StringWriter(strBuild);
            JsonWriter writer = new JsonTextWriter(sw);
            writer.Formatting = Formatting.None;
            serializer.Serialize(writer, o);
            return strBuild.ToString();
        }
    }
}
