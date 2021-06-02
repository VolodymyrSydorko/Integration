using DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL
{
    public interface INewsRepository
    {
        Task<NewsDTO[]> GetAllNewsAsync();
        Task<NewsDTO> AddNews(NewsDTO item);
        Task<NewsDTO> GetNewsByIdAsync(string id);

        Task<NewsDTO> UpdateTask(NewsDTO news);
    }
}
