using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CNNDesktop.Models
{
    static class GraphqlClient
    {
        public static GraphQLHttpClient CreateGraphqlClient()
        {
            var client = new GraphQLHttpClient("http://localhost:5000/graphql", new NewtonsoftJsonSerializer());

            return client;
        }

        public static GraphQLRequest RequestGetNews()
        {
            return new GraphQLRequest
            {
                Query = @"query { news{ id, title, author, description, url, isLiked } }",
            };
        }

        public static GraphQLRequest RequestChangeLiked(string id, bool isLiked)
        {
            return new GraphQLRequest
            {
                Query = @"mutation($id: String!, $isLiked : Boolean!) { addNews(news : { id: $id, isLiked: $isLiked }){id, title, author, description, url, isLiked } }",
                Variables = new
                {
                    id = id,
                    isLiked = isLiked
                }
            };
        }

        public static async Task<News> UpdateNews(string id, bool isLiked)
        {
            var client = CreateGraphqlClient();
            var requst = RequestChangeLiked(id, isLiked);

            var response = await client.SendMutationAsync<object>(requst);

            if (response.Errors == null)
            {
                JObject data = JObject.Parse(response.Data?.ToString() ?? "{}");

                return data["addNews"].ToObject<News>();
            }

            return default;
        }

        public static async Task<List<News>> GetAllNews()
        {
            var client = CreateGraphqlClient();
            var requst = RequestGetNews();

            var response = await client.SendQueryAsync<object>(requst);

            if (response.Errors == null)
            {
                JObject data = JObject.Parse(response.Data?.ToString() ?? "{}");

                return data["news"].ToObject<List<News>>();
            }

            return default;
        }
    }
}
