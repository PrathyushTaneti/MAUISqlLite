using MAUISqlLite.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MAUISqlLite.Data
{
    public class DbContext : IAsyncDisposable
    {
        private const string DbName = "MyDatabase.db3";

        private static string DbPath => Path.Combine(Path.Combine(FileSystem.AppDataDirectory), DbName);

        private SQLiteAsyncConnection _connection;

        private SQLiteAsyncConnection Database => _connection ??= new SQLiteAsyncConnection(DbPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache) ;

        private async Task CreateTableIfNotExists<T>() where T : class, new()
        {
            await Database.CreateTableAsync<T>();
        }

        private async Task<TResult> Execute<T, TResult>(Func<Task<TResult>> action ) where T : class, new()
        {
            await CreateTableIfNotExists<T>();
            return await action();
        }

        private async Task<AsyncTableQuery<T>> GetTableAsync<T>() where T : class, new()
        {
            await CreateTableIfNotExists<T>();
            return Database.Table<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
        {
            var table = await GetTableAsync<T>();
            return await table.ToListAsync();
        }

        public async Task<T> GetItemByKeyAsync<T>(object primaryKey) where T : class, new()
        {
            /*var table = await GetTableAsync<T>();
            return await Database.GetAsync<T>(primaryKey);*/
            return await Execute<T, T>(async () => await Database.GetAsync<T>(primaryKey));
        }

        public async Task<IEnumerable<T>> GetByFilterAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
        {
            var table = await GetTableAsync<T>();
            return await table.Where(predicate).ToListAsync();
        }

        public async Task<bool> AddItemAsync<T>(T item) where T : class, new()
        {
            await CreateTableIfNotExists<T>();
            return await Database.InsertAsync(item) > 0;
        }

        public async Task<bool> UpdateItemAsync<T>(T item) where T : class, new()
        {
            await CreateTableIfNotExists<T>();
            return await Database.UpdateAsync(item) > 0;
        }

        public async Task<bool> DeleteItemAsync<T>(T item) where T : class, new()
        {
            await CreateTableIfNotExists<T>();
            return await Database.DeleteAsync(item) > 0;
        }

        public async Task<bool> DeleteItemByKeyAsync<T>(object primaryKey) where T : class, new()
        {
            await CreateTableIfNotExists<T>();
            return await Database.DeleteAsync<T>(primaryKey) > 0;
        }

        public async ValueTask DisposeAsync()
        {
            await _connection?.CloseAsync();
        }
    }
}
