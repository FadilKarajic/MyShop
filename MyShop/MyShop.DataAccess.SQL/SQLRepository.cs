using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShop.Core.Contracts;
using MyShop.Core.Models;

namespace MyShop.DataAccess.SQL
{
    public class SQLRepository<Anything> : IRepository<Anything> where Anything : BaseEntity
    {
        internal DataContext context;
        internal DbSet<Anything> dbSet;

        public SQLRepository(DataContext context)
        {
            this.context = context;
            this.dbSet = context.Set<Anything>();
        }
        public IQueryable<Anything> Collection()
        {
            return dbSet;
        }

        public void Commit()
        {
            context.SaveChanges();
        }

        public void Delete(string Id)
        {
            var a = Find(Id);
            if (context.Entry(a).State == EntityState.Detached)
                dbSet.Attach(a);
            dbSet.Remove(a);
        }

        public Anything Find(string Id)
        {
            return dbSet.Find(Id);
        }

        public void Insert(Anything a)
        {
            dbSet.Add(a);
        }

        public void Update(Anything a)
        {
            dbSet.Attach(a);
            context.Entry(a).State = EntityState.Modified;
        }
    }
}
