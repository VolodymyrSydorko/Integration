using DTO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class NewsRepository : INewsRepository
    {
        private readonly DBContext _context = null;

        public NewsRepository(MongoConfig config)
        {
            _context = new DBContext(config);
        }

        public async Task<NewsDTO[]> GetAllNewsAsync()
        {
            try
            {
                var news = await _context.News.Find(_ => true).ToListAsync();
                return news.Select(MapToNewDTO).ToArray();
            }
            catch(Exception ex)
            {
                return new NewsDTO[0];
            }
        }

        public async Task<NewsDTO> UpdateTask(NewsDTO news)
        {
            var filter = Builders<NewsEntity>.Filter.Where(_ => _.ID == news.Id);
            var update = Builders<NewsEntity>.Update.Set("IsLiked", news.IsLiked);
            var entity = await _context.News.UpdateOneAsync(filter, update);

            return await GetNewsByIdAsync(news.Id);
        }

        public async Task<NewsDTO> AddNews(NewsDTO item)
        {
            if (!(await _context.News.Find(q => q.Title.Equals(item.Title)).ToListAsync()).Any())
                await _context.News.InsertOneAsync(MapToNewEntity(item));

            return item;
        }

        public async Task<NewsDTO> GetNewsByIdAsync(string id)
        {
            var document = await _context.News.Find(_ => _.ID == id).FirstOrDefaultAsync();
            return MapToNewDTO(document);
        }

        private NewsEntity MapToNewEntity(NewsDTO news)
        {
            return new NewsEntity
            {
                ID = news.Id,
                Author = news.Author,
                DateOfPublication = news.DateOfPublication,
                Description = news.Description,
                Title = news.Title,
                Url = news.Url,
                IsLiked = news.IsLiked
            };
        }
        private NewsDTO MapToNewDTO(NewsEntity entity)
        {
            return new NewsDTO
            {
                Id = entity.ID,
                Author = entity.Author,
                DateOfPublication = entity.DateOfPublication,
                Description = entity.Description,
                Title = entity.Title,
                Url = entity.Url,
                IsLiked = entity.IsLiked
            };
        }
    }

}
