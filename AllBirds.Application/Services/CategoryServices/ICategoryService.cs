using AllBirds.DTOs.Shared;
using AllBirds.DTOs.CategoryDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllBirds.Application.Services.CategoryServices
{
    public interface ICategoryService
    {
        public Task<ResultView<CUCategoryDTO>> CreateAsync(CUCategoryDTO entity);
        public Task<ResultView<CUCategoryDTO>> UpdateAsync(CUCategoryDTO entity);
        //public Task<ResultView<GetOneCategoryDTO>> HardDeleteAsync(int id);
        public Task<ResultView<GetOneCategoryDTO>> DeleteAsync(int id);
        public Task<ResultView<List<GetAllCategoryDTO>>> GetAllAsync(bool onlyParents = false);
        public Task<ResultView<EntityPaginated<GetAllCategoryDTO>>> GetAllPaginatedAsync(int pageNumber, int pageSize);
        public Task<ResultView<CUCategoryDTO>> GetOneAsync(int id);

        // services for api project
        //================================================================================================
        public Task<ResultView<List<GetAllCategoryNestedDTO>>> GetAllAPI();
        Task<ResultView<GetAllCategoryNestedDTO>> GetCategoryByIdAPI(int catId);

        // services for localization by Ahmed Elghoul
        //================================================================================================
        public Task<ResultView<List<GetAllCategoryWithLangDTO>>> GetAllAPIWithlang(string lang);
        public Task<ResultView<GetAllCategoryWithLangDTO>> GetCategoryByIdAPIWithLang(int catId,string lang);



    }
}
