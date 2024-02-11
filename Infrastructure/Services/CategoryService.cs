using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class CategoryService(CategoryRepository categoryRepository)
{
    private readonly CategoryRepository _categoryRepository = categoryRepository;

    /// <summary>
    /// A method to create a new category with a given name to the database
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns>True if hte new category was saved to the database, else returns false.</returns>
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

    /// <summary>
    /// This method gets a categoryEntity from the database, and then converts it to a CategoryDto.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>Returns a CategoryDto, else null. </returns>
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

    /// <summary>
    /// Gets all the Categoryentities from the database, then converts them to a list of CategoryDto.
    /// </summary>
    /// <returns>Returns the list of CategoryDto, else null.</returns>
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

    /// <summary>
    /// Updates a categoryEntity in the database based on the information in a CategoryDto.
    /// </summary>
    /// <param name="updatedCategory"></param>
    /// <returns>Returns a new CategoryDto with the updated information. Else null with a error-messagge.</returns>
    public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto updatedCategory)
    {
        try
        {
            var categoryEntity = new CategoryEntity { Id = updatedCategory.Id, CategoryName = updatedCategory.CategoryName };
            var updatedCategoryEntity = await _categoryRepository.UpdateAsync(x => x.Id == updatedCategory.Id, categoryEntity);

            if(updatedCategoryEntity != null)
            {
                var categoryDto = new CategoryDto(updatedCategory.Id, updatedCategory.CategoryName);
                return categoryDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return null!;
    }

    /// <summary>
    /// Deletes a categoryEntity from the database, based on Id.
    /// </summary>
    /// <param name="categoryId"></param>
    /// <returns>Returns the result if succesfull, else false.</returns>
    public async Task<bool> DeleteCategoryAsync(int categoryId)
    {
        try
        {
            Expression<Func<CategoryEntity, bool>> expression = c => c.Id == categoryId;

            var result = await _categoryRepository.DeleteAsync(expression);
            return result;
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return false;
    }
}
