using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DTO
{
    [Serializable]
    public class NewsDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime DateOfPublication { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public bool IsLiked { get; set; } = false;

        public override string ToString()
        {
            return string.Format("{0}{1}Title:  {2}{1}Author:  {3}{1}Description:  {4}{1}Time:  {5}{1}{0}",
                "----------------------------",
                Environment.NewLine,
                Title,
                Author,
                Description,
                DateOfPublication.ToLongTimeString()

           );
        }
    }

    public class BinaryConverter
    {
        public static byte[] ObjectToByteArray<T>(T obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream(arrBytes))
            {
                var binForm = new BinaryFormatter();
                var obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}
