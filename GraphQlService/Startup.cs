using DAL;
using GraphQL;
using GraphQL.Types;
using GraphQL.Validation.Complexity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;

namespace GraphQLService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGraphQl(schema =>
            {
                schema.SetQueryType<NewsQuery>();
                schema.SetMutationType<NewsMutation>();
            });

            services.AddMvc();

            var dbConfig = new MongoConfig();
            Configuration.Bind("MongoConnection", dbConfig);
            services.AddSingleton(dbConfig);

            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<INewsRepository, NewsRepository>();
            services.AddSingleton<NewsQuery>();
            services.AddSingleton<NewsType>();
            services.AddSingleton<NewsInputType>();
            services.AddSingleton<NewsMutation>();     

            var sp = services.BuildServiceProvider();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "Redis_";
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseGraphiql("/graphiql", options =>
            {
                options.GraphQlEndpoint = "/graphql";
            });
            app.UseGraphQl("/graphql", options =>
            {
                options.FormatOutput = false;
                options.ComplexityConfiguration = new ComplexityConfiguration { MaxDepth = 15 };
            });

            app.UseMvcWithDefaultRoute();
        }
    }

}
