using System.Linq;
using MyShop.Core.Models;

namespace MyShop.Core.Contracts
{
    public interface IRepository<Anything> where Anything : BaseEntity
    {
        IQueryable<Anything> Collection();
        void Commit();
        void Delete(string Id);
        Anything Find(string Id);
        void Insert(Anything a);
        void Update(Anything a);
    }
}