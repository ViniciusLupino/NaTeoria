namespace EcoImpulse
{
    public interface IHomeRepository
    {
        Task<IEnumerable<Produto>> GetProdutos(string sTerm = "", int GeneroId = 0);
        Task<IEnumerable<Genero>> Generos();
    }
}