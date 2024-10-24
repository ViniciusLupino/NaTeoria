using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookShoppingCartMvcUI.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class GenreController : Controller
    {
        private readonly IGenreRepository _genreRepo;

        public GenreController(IGenreRepository genreRepo)
        {
            _genreRepo = genreRepo;
        }

        public async Task<IActionResult> Index()
        {
            var generos = await _genreRepo.GetGenres();
            return View(generos);
        }

        public IActionResult AddGenre()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddGenre(GenreDTO genero)
        {
            if(!ModelState.IsValid)
            {
                return View(genero);
            }
            try
            {
                var genreToAdd = new Genero { GeneroName = genero.GeneroName, Id = genero.Id };
                await _genreRepo.AddGenre(genreToAdd);
                TempData["successMessage"] = "genero added successfully";
                return RedirectToAction(nameof(AddGenre));
            }
            catch(Exception ex)
            {
                TempData["errorMessage"] = "genero could not added!";
                return View(genero);
            }

        }

        public async Task<IActionResult> UpdateGenre(int id)
        {
            var genero = await _genreRepo.GetGenreById(id);
            if (genero is null)
                throw new InvalidOperationException($"genero with id: {id} does not found");
            var genreToUpdate = new GenreDTO
            {
                Id = genero.Id,
                GeneroName = genero.GeneroName
            };
            return View(genreToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGenre(GenreDTO genreToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return View(genreToUpdate);
            }
            try
            {
                var genero = new Genero { GeneroName = genreToUpdate.GeneroName, Id = genreToUpdate.Id };
                await _genreRepo.UpdateGenre(genero);
                TempData["successMessage"] = "genero is updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "genero could not updated!";
                return View(genreToUpdate);
            }

        }

        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genero = await _genreRepo.GetGenreById(id);
            if (genero is null)
                throw new InvalidOperationException($"genero with id: {id} does not found");
            await _genreRepo.DeleteGenre(genero);
            return RedirectToAction(nameof(Index));

        }

    }
}
