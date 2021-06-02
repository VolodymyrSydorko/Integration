using DAL;
using DTO;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLService
{
    public class NewsSchema : Schema
    {
        public NewsSchema(IDependencyResolver resolver) : base(resolver)
        { 
            
            Query = resolver.Resolve<NewsQuery>();
            Mutation = resolver.Resolve<NewsMutation>();
        }
    }

    public class NewsMutation : ObjectGraphType
    {
        public NewsMutation(INewsRepository newsRepository)
        {
            Field<NewsType>(
                "addNews",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<NewsInputType>> { Name = "news" }
                ),
                resolve: context =>
                {
                    var n = context.GetArgument<NewsDTO>("news");
                    return newsRepository.UpdateTask(n);
                });
        }
    }
    public class NewsQuery : ObjectGraphType
    {
        public NewsQuery(INewsRepository newsRepository, IDistributedCache cache)
        {
            Field<ListGraphType<NewsType>>(
                "news",
                resolve: context =>
                {
                    string recordKey = "CNNNews_" + DateTime.Now.ToString("yyyyMMdd_hhmm");

                    var news = cache.GetRecordAsync<NewsDTO[]>(recordKey).Result;

                    if (news is null)
                    {
                        news = newsRepository.GetAllNewsAsync().Result;

                        cache.SetRecordAsync(recordKey, news).Wait();
                    }

                    return news;
                }
            );

            Field<NewsType>(
                "newsById",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "id" }),
                resolve: context => newsRepository.GetNewsByIdAsync(context.GetArgument<string>("id"))
            );
        }
    }
    public class NewsType : ObjectGraphType<NewsDTO>
    {
        public NewsType()
        {
            Field(x => x.Id, true);
            Field(x => x.Author, true);
            Field(x => x.Title, true);
            Field(x => x.Url, true);
            Field(x => x.Description, true);
            Field(x => x.DateOfPublication, true);
            Field(x => x.IsLiked, false);
        }
    }

    public class NewsInputType : InputObjectGraphType
    {
        public NewsInputType()
        {
            Name = "NewsInput";
            Field<StringGraphType>("id");
            Field<StringGraphType>("author");
            Field<StringGraphType>("title");
            Field<StringGraphType>("url");
            Field<StringGraphType>("description");
            Field<DateGraphType>("dateOfPublication");
            Field<BooleanGraphType>("isLiked");
        }
    }
}
