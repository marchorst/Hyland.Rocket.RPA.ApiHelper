namespace Hyland.Rocket.RPA.ApiHelper
{
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;

    public static class RpaHelper
    {
        public static string SerializeDataTable(DataTable table) => Serialize(table);

        public static string Serialize<T>(T table)
        {
            var serializer = new XmlSerializer(typeof(T));
            var textWriter = new StringWriter();
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
            var serializer = JsonSerializer.Create();
            var strBuild = new StringBuilder();
            var sw = new StringWriter(strBuild);
            JsonWriter writer = new JsonTextWriter(sw)
            {
                Formatting = Formatting.None
            };
            serializer.Serialize(writer, o);
            return strBuild.ToString();
        }
    }
}
