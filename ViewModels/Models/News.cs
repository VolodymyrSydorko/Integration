using Newtonsoft.Json;
using PropertyChanged;

namespace CNNDesktop.Models
{
    [AddINotifyPropertyChangedInterface]
    public class News
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("isLiked")]
        public bool IsLiked { get; set; }
    }
}