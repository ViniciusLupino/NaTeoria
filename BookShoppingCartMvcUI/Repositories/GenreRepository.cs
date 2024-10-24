using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMvcUI.Repositories;

public interface IGenreRepository
{
    Task AddGenre(Genero genero);
    Task UpdateGenre(Genero genero);
    Task<Genero?> GetGenreById(int id);
    Task DeleteGenre(Genero genero);
    Task<IEnumerable<Genero>> GetGenres();
}
public class GenreRepository : IGenreRepository
{
    private readonly ApplicationDbContext _context;
    public GenreRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddGenre(Genero genero)
    {
        _context.Generos.Add(genero);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateGenre(Genero genero)
    {
        _context.Generos.Update(genero);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteGenre(Genero genero)
    {
        _context.Generos.Remove(genero);
        await _context.SaveChangesAsync();
    }

    public async Task<Genero?> GetGenreById(int id)
    {
        return await _context.Generos.FindAsync(id);
    }

    public async Task<IEnumerable<Genero>> GetGenres()
    {
        return await _context.Generos.ToListAsync();
    }

    
}
