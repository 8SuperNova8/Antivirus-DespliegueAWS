using Api_Antivirus.Data;
using Api_Antivirus.Models;
using Microsoft.EntityFrameworkCore;


namespace Api_Antivirus.Services 
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService (ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task <IEnumerable<Category>> GetAllAsync ()
        {  
            return await _context.Categories.ToListAsync();
        }
        public async Task <Category?> GetByIdAsync(int id)
        {
            var category= await _context.Categories.FindAsync(id);
            if (category != null)
            {
                return category;
            }
            Console.WriteLine("Categoria No encontrada");
            return null;
        }
        public async Task CreateAsync (Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            category.Id = category.Id;
        }
        public async Task UpdateAsync (Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync  (int id)
        {
            var category =await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}