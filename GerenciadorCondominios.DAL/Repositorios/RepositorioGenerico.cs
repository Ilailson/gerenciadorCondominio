using GerenciadorCondominios.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorCondominios.DAL.Repositorios
{
    public class RepositorioGenerico<TEntity> : IRepositorioGenerico<TEntity> where TEntity : class
    {

        private readonly Contexto _contexto;

        public RepositorioGenerico(Contexto contexto)
        {
            _contexto = contexto;
        }

        public async Task Atualizar(TEntity entity)
        {
            try
            {
                _contexto.Set<TEntity>().Update(entity);
                await _contexto.SaveChangesAsync(); //esperar finalizar tudo desse metodo para salva
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task Excluir(TEntity entity)
        {
            try
            {
                _contexto.Set<TEntity>().Remove(entity);
                await _contexto.SaveChangesAsync(); //esperar finalizar tudo desse metodo para salva
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Excluir(int id)
        {
            try
            {
                var entity = await PegarPeloId(id);
                _contexto.Set<TEntity>().Remove(entity);
                await _contexto.SaveChangesAsync(); //esperar finalizar tudo desse metodo para salva
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Excluir(string id)
        {
            try
            {
                var entity = await PegarPeloId(id);
                _contexto.Set<TEntity>().Remove(entity);
                await _contexto.SaveChangesAsync(); //esperar finalizar tudo desse metodo para salva
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task inserir(TEntity entity)
        {
           try
            {
                await _contexto.AddAsync(entity);
                await _contexto.SaveChangesAsync();
            }
           catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TEntity> PegarPeloId(int id)
        {
            try
            {
                return await _contexto.Set<TEntity>().FindAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TEntity> PegarPeloId(string id)
        {
            try
            {
                return await _contexto.Set<TEntity>().FindAsync(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<TEntity>> PegarTodos()
        {
            try
            {
                return await _contexto.Set<TEntity>().ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
