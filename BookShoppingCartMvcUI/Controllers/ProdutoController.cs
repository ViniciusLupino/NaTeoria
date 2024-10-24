using BookShoppingCartMvcUI.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookShoppingCartMvcUI.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class ProdutoController : Controller
{
    private readonly IProdutoRepository _prodRep;
    private readonly IGenreRepository _genreRepo;
    private readonly IFileService _fileService;

    public ProdutoController(IProdutoRepository produtoRepo, IGenreRepository genreRepo, IFileService fileService)
    {
        _prodRep = produtoRepo;
        _genreRepo = genreRepo;
        _fileService = fileService;
    }

    public async Task<IActionResult> Index()
    {
        var produtos = await _prodRep.GetProdutos();
        return View(produtos);
    }

    public async Task<IActionResult> AddProduto()
    {
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genero => new SelectListItem
        {
            Text = genero.GeneroName,
            Value = genero.Id.ToString(),
        });
        ProdutoDTO produtoToAdd = new() { GenreList = genreSelectList };
        return View(produtoToAdd);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduto(ProdutoDTO produtoToAdd)
    {
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genero => new SelectListItem
        {
            Text = genero.GeneroName,
            Value = genero.Id.ToString(),
        });
        produtoToAdd.GenreList = genreSelectList;

        if (!ModelState.IsValid)
            return View(produtoToAdd);

        try
        {
            if (produtoToAdd.ImageFile != null)
            {
                if(produtoToAdd.ImageFile.Length> 1 * 1024 * 1024)
                {
                    throw new InvalidOperationException("Image file can not exceed 1 MB");
                }
                string[] allowedExtensions = [".jpeg",".jpg",".png"];
                string imageName=await _fileService.SaveFile(produtoToAdd.ImageFile, allowedExtensions);
                produtoToAdd.Image = imageName;
            }
            // manual mapping of BookDTO -> produto
            Produto produto = new()
            {
                Id = produtoToAdd.Id,
                ProdutoName = produtoToAdd.ProdutoName,
                Image = produtoToAdd.Image,
                GeneroId = produtoToAdd.GeneroId,
                Price = produtoToAdd.Price
            };
            await _prodRep.AddProduto(produto);
            TempData["successMessage"] = "produto is added successfully";
            return RedirectToAction(nameof(AddProduto));
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"]= ex.Message;
            return View(produtoToAdd);
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(produtoToAdd);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Error on saving data";
            return View(produtoToAdd);
        }
    }

    public async Task<IActionResult> UpdateProduto(int id)
    {
        var produto = await _prodRep.GetProdutoById(id);
        if(produto==null)
        {
            TempData["errorMessage"] = $"produto with the id: {id} does not found";
            return RedirectToAction(nameof(Index));
        }
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genero => new SelectListItem
        {
            Text = genero.GeneroName,
            Value = genero.Id.ToString(),
            Selected=genero.Id==produto.GeneroId
        });
        ProdutoDTO produtoToUpdate = new() 
        { 
            GenreList = genreSelectList,
            ProdutoName=produto.ProdutoName,
            GeneroId=produto.GeneroId,
            Price=produto.Price,
            Image=produto.Image 
        };
        return View(produtoToUpdate);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProdutos(ProdutoDTO produtoToUpdate)
    {
        var genreSelectList = (await _genreRepo.GetGenres()).Select(genero => new SelectListItem
        {
            Text = genero.GeneroName,
            Value = genero.Id.ToString(),
            Selected=genero.Id==produtoToUpdate.GeneroId
        });
        produtoToUpdate.GenreList = genreSelectList;

        if (!ModelState.IsValid)
            return View(produtoToUpdate);

        try
        {
            string imagemAntiga = "";
            if (produtoToUpdate.ImageFile != null)
            {
                if (produtoToUpdate.ImageFile.Length > 1 * 1024 * 1024)
                {
                    throw new InvalidOperationException("Image file can not exceed 1 MB");
                }
                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                string imageName = await _fileService.SaveFile(produtoToUpdate.ImageFile, allowedExtensions);
                
                imagemAntiga = produtoToUpdate.Image;
                produtoToUpdate.Image = imageName;
            }
            Produto produto = new()
            {
                Id=produtoToUpdate.Id,
                ProdutoName = produtoToUpdate.ProdutoName,
                GeneroId = produtoToUpdate.GeneroId,
                Price = produtoToUpdate.Price,
                Image = produtoToUpdate.Image
            };
            await _prodRep.UpdateProdutos(produto);
            if(!string.IsNullOrWhiteSpace(imagemAntiga))
            {
                _fileService.DeleteFile(imagemAntiga);
            }
            TempData["successMessage"] = "Produto atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(produtoToUpdate);
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(produtoToUpdate);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Não foi possível salvar os dados.";
            return View(produtoToUpdate);
        }
    }

    public async Task<IActionResult> DeleteProduto(int id)
    {
        try
        {
            var produto = await _prodRep.GetProdutoById(id);
            if (produto == null)
            {
                TempData["errorMessage"] = $"produto with the id: {id} does not found";
            }
            else
            {
                await _prodRep.DeleteProduto(produto);
                if (!string.IsNullOrWhiteSpace(produto.Image))
                {
                    _fileService.DeleteFile(produto.Image);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Error on deleting the data";
        }
        return RedirectToAction(nameof(Index));
    }

}
