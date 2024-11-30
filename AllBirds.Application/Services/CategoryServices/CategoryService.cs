using AllBirds.Application.Contracts;
using AllBirds.DTOs.Shared;
using AllBirds.DTOs.CategoryDTOs;
using AllBirds.Models;
using AutoMapper;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace AllBirds.Application.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        public CategoryService(ICategoryRepository _categoryRepository, IProductRepository _productRepository, IMapper _mapper)
        {
            categoryRepository = _categoryRepository;
            productRepository = _productRepository;
            mapper = _mapper;
        }

        public async Task<ResultView<CUCategoryDTO>> CreateAsync(CUCategoryDTO entity)
        {
            ResultView<CUCategoryDTO> resultView = new();
            try
            {
                bool exist = (await categoryRepository.GetAllAsync()).Any(c => (c.NameEn == entity.NameEn) || (c.NameAr == entity.NameAr));
                if (exist)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Category With Same Name ({entity.NameEn}/{entity.NameAr}) Already Exist";
                }
                else
                {
                    // modifications by sonbaty
                    if (entity.ParentCategoryId != 0)
                    {
                        Category? parentCategory = (await categoryRepository.GetAllAsync()).FirstOrDefault(c => c.Id == entity.ParentCategoryId);
                        if (parentCategory is not null)
                        {
                            entity.Level = parentCategory.Level + 1;
                        }
                        else
                        {
                            resultView.IsSuccess = false;
                            resultView.Data = null;
                            resultView.Msg = $"Parent Category ({entity.ParentCategoryId}) Doesn't Exist";
                            return resultView;
                        }
                    }
                    // end modifications
                    Category category = mapper.Map<Category>(entity);
                    Category successCategory = await categoryRepository.CreateAsync(category);
                    await categoryRepository.SaveChangesAsync();
                    CUCategoryDTO successCategoryDTO = mapper.Map<CUCategoryDTO>(successCategory);
                    resultView.IsSuccess = true;
                    resultView.Data = successCategoryDTO;
                    resultView.Msg = $"Category ({entity.NameEn}) Created Successfully";
                }
            }
            catch (Exception ex)
            {

                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Creating Category " + ex.Message;
            }
            return resultView;
        }

        public async Task<ResultView<CUCategoryDTO>> UpdateAsync(CUCategoryDTO entity)
        {
            ResultView<CUCategoryDTO> resultView = new();
            try
            {
                bool exist = (await categoryRepository.GetAllAsync()).Any(c => c.Id == entity.Id);
                if (!exist)
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Category ({entity.NameEn}) Is Not Found";
                    return resultView;
                }
                // modifications by sonbaty
                else if ((await categoryRepository.GetAllAsync()).Any(c => (c.Id != entity.Id && c.NameEn == entity.NameEn) || (c.Id != entity.Id && c.NameAr == entity.NameAr)))
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"Category With Name ({entity.NameEn}/{entity.NameAr}) Exists Before";
                    return resultView;
                }
                if (entity.ParentCategoryId != 0)
                {
                    Category? parentCategory = (await categoryRepository.GetAllAsync()).FirstOrDefault(c => c.Id == entity.ParentCategoryId);
                    if (parentCategory is not null)
                    {
                        entity.Level = parentCategory.Level + 1;
                    }
                    else
                    {
                        resultView.IsSuccess = false;
                        resultView.Data = null;
                        resultView.Msg = $"Parent Category ({entity.ParentCategoryId}) Doesn't Exist";
                        return resultView;
                    }
                }
                // end modifications
                Category category = mapper.Map<Category>(entity);
                Category successCategory = await categoryRepository.UpdateAsync(category);
                await categoryRepository.SaveChangesAsync();
                CUCategoryDTO successCategoryDTO = mapper.Map<CUCategoryDTO>(successCategory);
                resultView.IsSuccess = true;
                resultView.Data = successCategoryDTO;
                resultView.Msg = $"Category ({entity.NameEn}) updated Successfully";
                return resultView;
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While updating Category ({entity.NameEn})" + ex.Message;
                return resultView;
            }
        }

        public async Task<ResultView<List<GetAllCategoryDTO>>> GetAllAsync(bool onlyParents = false)
        {
            ResultView<List<GetAllCategoryDTO>> resultView = new();
            try
            {
                List<Category> successCategorys;
                if (onlyParents)
                    successCategorys = [.. (await categoryRepository.GetAllAsync()).Where(a => !a.IsDeleted && a.IsParentCategory == true).OrderByDescending(c => c.Created)];
                else
                    successCategorys = [.. (await categoryRepository.GetAllAsync()).Where(a => !a.IsDeleted).OrderByDescending(c => c.Created)];
                List<GetAllCategoryDTO> successCategorySDTO = mapper.Map<List<GetAllCategoryDTO>>(successCategorys);
                resultView.IsSuccess = true;
                resultView.Data = successCategorySDTO;
                resultView.Msg = "All Categories Fetched Successfully";
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Fetching All Categories {ex.Message}";
            }
            return resultView;
        }

        // for pagination
        public async Task<ResultView<EntityPaginated<GetAllCategoryDTO>>> GetAllPaginatedAsync(int pageNumber, int pageSize)
        {
            ResultView<EntityPaginated<GetAllCategoryDTO>> resultView = new();
            try
            {
                List<Category> categories;
                categories = [.. (await categoryRepository.GetAllAsync())
                    .Where(a => !a.IsDeleted)
                    .OrderByDescending(c => c.Created)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)];

                List<GetAllCategoryDTO> categoryDTOs = mapper.Map<List<GetAllCategoryDTO>>(categories);
                int totalCategories = (await categoryRepository.GetAllAsync()).Count(a => !a.IsDeleted);

                resultView.IsSuccess = true;
                resultView.Data = new EntityPaginated<GetAllCategoryDTO>
                {
                    Data = categoryDTOs,
                    Count = totalCategories
                };
                resultView.Msg = "All Categories Fetched Successfully";
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happened While Fetching All Categories {ex.Message}";
            }
            return resultView;
        }

        public async Task<ResultView<CUCategoryDTO>> GetOneAsync(int id)
        {
            ResultView<CUCategoryDTO> resultView = new();
            try
            {
                Category? category = (await categoryRepository.GetAllAsync()).FirstOrDefault(c => c.Id == id && !c.IsDeleted);
                if (category is not null)
                {
                    CUCategoryDTO successCategoryDTO = mapper.Map<CUCategoryDTO>(category);
                    resultView.IsSuccess = true;
                    resultView.Data = successCategoryDTO;
                    resultView.Msg = $"Category {category.NameEn} Is Found";
                    return resultView;
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"category {id} is not found";
                    return resultView;
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While find Category " + ex.Message;
                return resultView;
            }
        }

        //public async Task<ResultView<GetOneCategoryDTO>> HardDeleteAsync(int id)
        //{
        //    ResultView<GetOneCategoryDTO> resultView = new();
        //    try
        //    {
        //        Category? successCategory = (await categoryRepository.GetAllAsync()).FirstOrDefault(c => c.Id == id);
        //        if (successCategory is not null)
        //        {
        //            bool dependentCats = (await categoryRepository.GetAllAsync()).Any(c => c.ParentCategoryId == id);
        //            if (dependentCats)
        //            {
        //                resultView.IsSuccess = false;
        //                resultView.Data = null;
        //                resultView.Msg = $"category {successCategory.NameEn} cannot be hard deleted as there are categories that depend on it";
        //            }
        //            else
        //            {
        //                Category successCategory2 = await categoryRepository.DeleteAsync(successCategory);
        //                await categoryRepository.SaveChangesAsync();
        //                GetOneCategoryDTO successCategoryDTO = mapper.Map<GetOneCategoryDTO>(successCategory2);
        //                resultView.IsSuccess = true;
        //                resultView.Data = successCategoryDTO;
        //                resultView.Msg = $"category {successCategory.NameEn} is hard deleted successfully";
        //            }
        //        }
        //        else
        //        {
        //            resultView.IsSuccess = false;
        //            resultView.Data = null;
        //            resultView.Msg = $"category ({id}) is not found";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        resultView.IsSuccess = false;
        //        resultView.Data = null;
        //        resultView.Msg = $"Error Happen While find Category ({id}) {ex.Message}";
        //    }
        //    return resultView;
        //}

        public async Task<ResultView<GetOneCategoryDTO>> DeleteAsync(int id)
        {
            ResultView<GetOneCategoryDTO> resultView = new();
            try
            {
                Category? successCategory = (await categoryRepository.GetAllAsync()).FirstOrDefault(c => c.Id == id);
                if (successCategory is not null)
                {
                    bool dependentCats = (await categoryRepository.GetAllAsync()).Any(c => c.ParentCategoryId == id);
                    if (dependentCats)
                    {
                        resultView.IsSuccess = false;
                        resultView.Data = null;
                        resultView.Msg = $"Category {successCategory.NameEn} Couldn't Be Deleted As There Are Categories That Depend On It";
                    }
                    else if ((await categoryRepository.GetAllAsync()).Any(c => c.Id == id && c.Products.Any(p => p.CategoryId == c.Id)))
                    {
                        resultView.IsSuccess = false;
                        resultView.Data = null;
                        resultView.Msg = $"Category {successCategory.NameEn} Couldn't Be Deleted As There Are Products That Depend On It";
                    }
                    else
                    {
                        Category successCategory2 = await categoryRepository.DeleteAsync(successCategory);
                        await categoryRepository.SaveChangesAsync();
                        GetOneCategoryDTO successCategoryDTO = mapper.Map<GetOneCategoryDTO>(successCategory2);
                        resultView.IsSuccess = true;
                        resultView.Data = successCategoryDTO;
                        resultView.Msg = $"Category {successCategory.NameEn} Was Deleted Successfully";
                    }
                }
                else
                {
                    resultView.IsSuccess = false;
                    resultView.Data = null;
                    resultView.Msg = $"category {successCategory.NameEn} is not found";
                }
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While find Category " + ex.Message;
            }
            return resultView;
        }

        public Task<int> SaveChangesAsync()
        {
            return categoryRepository.SaveChangesAsync();
        }

        // services for api project
        //================================================================================================

        public List<GetAllCategoryNestedDTO> test(List<Category> allCats, int parentId)
        {
            List<GetAllCategoryNestedDTO> result = new();
            //List<Category> subChildren = children.Where(c => c.ParentCategoryId == parentId).ToList();
            foreach (Category child in allCats.Where(c => c.ParentCategoryId == parentId))
            {
                GetAllCategoryNestedDTO obj = new();
                obj.Id = child.Id;
                obj.NameAr = child.NameAr;
                obj.NameEn = child.NameEn;
                obj.Level = child.Level;
                obj.IsParentCategory = child.IsParentCategory;
                obj.ParentCategoryId = child.ParentCategoryId;
                obj.Children = [];
                if (child.IsParentCategory)
                {
                    obj.Children = test(allCats, child.Id);
                }
                result.Add(obj);
            }
            return result;
        }

        public async Task<ResultView<List<GetAllCategoryNestedDTO>>> GetAllAPI()
        {
            ResultView<List<GetAllCategoryNestedDTO>> resultView = new();
            try
            {
                List<Category> allCats = (await categoryRepository.GetAllAsync()).Where(a => !a.IsDeleted).ToList();
                List<GetAllCategoryNestedDTO> result = new();
                List<Category> grandParents = allCats.Where(c => c.ParentCategoryId == 0).ToList();
                foreach (Category parent in grandParents)
                {
                    GetAllCategoryNestedDTO mappedObj = new();
                    mappedObj.Id = parent.Id;
                    mappedObj.NameAr = parent.NameAr;
                    mappedObj.NameEn = parent.NameEn;
                    mappedObj.Level = parent.Level;
                    mappedObj.IsParentCategory = parent.IsParentCategory;
                    mappedObj.ParentCategoryId = parent.ParentCategoryId;
                    //mappedObj.Children = parent.IsParentCategory ? test(allCats.Where(ch => ch.ParentCategoryId == parent.Id).ToList(), parent) : [];
                    mappedObj.Children = [];
                    if (parent.IsParentCategory)
                    {
                        mappedObj.Children = test(allCats, parent.Id);
                    }
                    result.Add(mappedObj);
                }
                resultView.IsSuccess = true;
                resultView.Data = result;
                resultView.Msg = "All Categories Fetched Successfully";
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Fetching Categories {ex.Message}";
            }
            return resultView;
            //string jsonResult = JsonSerializer.Serialize(result);
            //Console.WriteLine(jsonResult);
        }


        // Get Parent Category Additional All Sub Category Follower Parent Category 
        public async Task<ResultView<GetAllCategoryNestedDTO>> GetCategoryByIdAPI(int Id)
        {
            ResultView<GetAllCategoryNestedDTO> resultView = new();
            try
            {
                List<Category> allCats = [.. (await categoryRepository.GetAllAsync()).Where(a => !a.IsDeleted).AsNoTracking()];
                Category ParentCategory = allCats.FirstOrDefault(P => P.Id == Id);

                // Check On Parent Category Has Data Or Null !!
                if (ParentCategory == null)
                {
                    resultView.IsSuccess = false;
                    resultView.Msg = "Category not found.";
                    return resultView;
                }


                GetAllCategoryNestedDTO getAllCategoryNestedDTO = new()
                {
                    Id = ParentCategory.Id,
                    IsParentCategory = ParentCategory.IsParentCategory,
                    Level = ParentCategory.Level,
                    NameEn = ParentCategory.NameEn,
                    NameAr = ParentCategory.NameAr,
                    ParentCategoryId = ParentCategory.ParentCategoryId,
                    Children = [],
                };


                if (ParentCategory.IsParentCategory)
                {

                    // Check On Parent Category Has Chlid Or Sub Category Or No ?? 
                    foreach (Category nestedDTO in allCats.Where(P => P.ParentCategoryId == Id))
                    {

                        GetAllCategoryNestedDTO nestedDTO1 = new GetAllCategoryNestedDTO()
                        {
                            Id = nestedDTO.Id,
                            Level = nestedDTO.Level,
                            NameEn = nestedDTO.NameEn,
                            NameAr = nestedDTO.NameAr,
                            ParentCategoryId = nestedDTO.ParentCategoryId,
                            IsParentCategory = nestedDTO.IsParentCategory,
                            Children = [],
                        };
                        getAllCategoryNestedDTO.Children.Add(nestedDTO1);
                    }

                    resultView.IsSuccess = true;
                    resultView.Data = getAllCategoryNestedDTO;
                    resultView.Msg = "All Categories Fetched Successfully";
                }
            }

            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Fetching Categories {ex.Message}";
            }
            return resultView;

        }




        



        //Services for Localization  By ahmed Elghoul
        //================================================================================================

        public async Task<List<GetAllCategoryWithLangDTO>> testLocal(List<Category> allCats, int parentId,string lang)
        {
            List<GetAllCategoryWithLangDTO> result = new();
            //List<Category> subChildren = children.Where(c => c.ParentCategoryId == parentId).ToList();
            foreach (Category child in allCats.Where(c => c.ParentCategoryId == parentId))
            {
                GetAllCategoryWithLangDTO obj = new();
                obj.Id = child.Id;
                obj.Name = (lang == "en") ? child.NameEn : child.NameAr;
                obj.Level = child.Level;
                obj.IsParentCategory = child.IsParentCategory;
                obj.ParentCategoryId = child.ParentCategoryId;
                obj.Children = [];
                if (child.IsParentCategory)
                {
                    obj.Children = await testLocal(allCats, child.Id, lang);
                }
                else if (child.NameEn == "Bestsellers")
                {
                    obj.Children = [.. (await productRepository.GetAllAsync()).Where(p => p.Categories.Any(cp => cp.CategoryId == child.Id))
                        .Select(cp => new GetAllCategoryWithLangDTO()
                        {
                            Id = cp.Id,
                            Name = (lang == "en") ? cp.NameEn : cp.NameAr,
                            Children = null,
                            IsParentCategory = false,
                            ParentCategoryId = child.Id,
                            Level = child.Level + 1
                        })];
                }
                result.Add(obj);
            }
            return result;
        }



        public async Task<ResultView<List<GetAllCategoryWithLangDTO>>> GetAllAPIWithlang(string lang)
        {
            ResultView<List<GetAllCategoryWithLangDTO>> resultView = new();
            try
            {
                List<Category> allCats = (await categoryRepository.GetAllAsync()).Where(a => !a.IsDeleted).ToList();
                List<GetAllCategoryWithLangDTO> result = new();
                List<Category> grandParents = allCats.Where(c => c.ParentCategoryId == 0).ToList();
                foreach (Category parent in grandParents)
                {
                    GetAllCategoryWithLangDTO mappedObj = new();
                    mappedObj.Id = parent.Id;
                    mappedObj.Name = (lang == "en") ? parent.NameEn : parent.NameAr;
                    mappedObj.Level = parent.Level;
                    mappedObj.IsParentCategory = parent.IsParentCategory;
                    mappedObj.ParentCategoryId = parent.ParentCategoryId;
                    mappedObj.Children = [];
                    if (parent.IsParentCategory)
                    {
                        mappedObj.Children = await testLocal(allCats, parent.Id,lang);
                    }
                    result.Add(mappedObj);
                }
                resultView.IsSuccess = true;
                resultView.Data = result;
                resultView.Msg = "All Categories Fetched Successfully";
            }
            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Fetching Categories {ex.Message}";
            }
            return resultView;
            //string jsonResult = JsonSerializer.Serialize(result);
            //Console.WriteLine(jsonResult);
        }


        public async Task<ResultView<GetAllCategoryWithLangDTO>> GetCategoryByIdAPIWithLang(int Id,string lang)
        {
            ResultView<GetAllCategoryWithLangDTO> resultView = new();
            try
            {
                List<Category> allCats = [.. (await categoryRepository.GetAllAsync()).Where(a => !a.IsDeleted).AsNoTracking()];
                Category ParentCategory = allCats.FirstOrDefault(P => P.Id == Id);

                // Check On Parent Category Has Data Or Null !!
                if (ParentCategory == null)
                {
                    resultView.IsSuccess = false;
                    resultView.Msg = "Category not found.";
                    return resultView;
                }


                GetAllCategoryWithLangDTO getAllCategoryNestedDTO = new()
                {
                    Id = ParentCategory.Id,
                    IsParentCategory = ParentCategory.IsParentCategory,
                    Level = ParentCategory.Level,
                    Name = (lang == "en") ? ParentCategory.NameEn : ParentCategory.NameAr,
                    ParentCategoryId = ParentCategory.ParentCategoryId,
                    Children = [],
                };


                if (ParentCategory.IsParentCategory)
                {

                    // Check On Parent Category Has Chlid Or Sub Category Or No ?? 
                    foreach (Category nestedDTO in allCats.Where(P => P.ParentCategoryId == Id))
                    {

                        GetAllCategoryWithLangDTO nestedDTO1 = new GetAllCategoryWithLangDTO()
                        {
                            Id = nestedDTO.Id,
                            Level = nestedDTO.Level,
                            Name = (lang == "en") ? nestedDTO.NameEn : nestedDTO.NameAr,
                            ParentCategoryId = nestedDTO.ParentCategoryId,
                            IsParentCategory = nestedDTO.IsParentCategory,
                            Children = [],
                        };
                        getAllCategoryNestedDTO.Children.Add(nestedDTO1);
                    }

                    resultView.IsSuccess = true;
                    resultView.Data = getAllCategoryNestedDTO;
                    resultView.Msg = "All Categories Fetched Successfully";
                }
            }

            catch (Exception ex)
            {
                resultView.IsSuccess = false;
                resultView.Data = null;
                resultView.Msg = $"Error Happen While Fetching Categories {ex.Message}";
            }
            return resultView;

        }


    }
}
