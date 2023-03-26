namespace GerenciadorCondominios.DAL.Interfaces
{
    public  interface IRepositorioGenerico<TEntity> where TEntity: class
    {
        Task<IEnumerable<TEntity>> PegarTodos();
        Task<TEntity> PegarPeloId(int id);
        Task<TEntity> PegarPeloId(string id);
        Task inserir(TEntity entity); //como vai retornar nada - não colocar TEntity
        Task Atualizar(TEntity entity); //como vai retornar nada - não colocar TEntity
        Task Excluir(TEntity entity); //como vai retornar nada - não colocar TEntity
        Task Excluir(int id); //como vai retornar nada - não colocar TEntity
        Task Excluir(string id); //como vai retornar nada - não colocar TEntity









    }
}
