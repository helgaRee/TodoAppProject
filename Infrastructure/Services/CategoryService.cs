using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class CategoryService(CategoryRepository categoryRepository)
{
    private readonly CategoryRepository _categoryRepository = categoryRepository;

    public async Task<bool> CreateCategoryAsync(string categoryName)
    {
        try
        {
            var categoryExists = await _categoryRepository.ExistingAsync(x => x.CategoryName == categoryName);
            if (!categoryExists)
            {
                var categoryEntity = await _categoryRepository.CreateAsync(new CategoryEntity { CategoryName = categoryName });
                if (categoryEntity != null)
                {
                   return true;
                }
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + ex.Message); }
        return false;
    }

    public async Task<CategoryDto> GetCategoryAsync(Expression<Func<CategoryEntity, bool>> predicate)
    {
        try
        {
            //försöker Hämta entitet
            var categoryEntity = await _categoryRepository.GetAsync(predicate);
            if (categoryEntity != null)
            {
                //om hämtning lyckas, omvandla entiteten till en Dto med Id och Cname
                var categoryDto = new CategoryDto(categoryEntity.Id, categoryEntity.CategoryName);
                return categoryDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return null!;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync()
    {
        try
        {
            var categoryEntities = await _categoryRepository.GetAllAsync();

            if (categoryEntities != null)
            {
                var list = new List<CategoryDto>();
                foreach (var categoryEntity in categoryEntities)
                    list.Add(new CategoryDto(categoryEntity.Id, categoryEntity.CategoryName));
                return list;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return null!;
    }

    public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto updatedCategory)
    {
        try
        {
            //försöker Hämta entitet baserat på Id (den gör en hämtning via BaseRepo) och uppdaterar
            var categoryEntity = new CategoryEntity { Id = updatedCategory.Id, CategoryName = updatedCategory.CategoryName };
            var updatedCategoryEntity = await _categoryRepository.UpdateAsync(x => x.Id == updatedCategory.Id, categoryEntity);
            //om uppdateringen lyckades
            if(updatedCategoryEntity != null)
            {
                //skapa ny CategoryDto
                var categoryDto = new CategoryDto(updatedCategory.Id, updatedCategory.CategoryName);
                return categoryDto;
            }

        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return null!;
    }

    public async Task<bool> DeleteCategoryAsync(Expression<Func<CategoryEntity, bool>> expression)
    {
        try
        {
            var result = await _categoryRepository.DeleteAsync(expression);
            return result;
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return false;
    }

}
