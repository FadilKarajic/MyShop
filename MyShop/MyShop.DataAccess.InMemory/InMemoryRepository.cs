using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using MyShop.Core.Models;
using MyShop.Core.Contracts;

namespace MyShop.DataAccess.InMemory
{
    //Placeholder can be "anything"
    public class InMemoryRepository<Anything> : IRepository<Anything> where Anything:BaseEntity
    {
        ObjectCache cache = MemoryCache.Default;
        List<Anything> items;
        string className;


        public InMemoryRepository()
        {
            className = typeof(Anything).Name;
            items = cache[className] as List<Anything>;
            if (items == null)
            {
                items=new List<Anything>();
            }
        }

        public void Commit()
        {
            cache[className] = items;
        }

       
        public void Insert(Anything a)
        {
            items.Add(a);
        }

        public void Update(Anything a)
        {
            Anything toUpdate = items.Find(i => i.Id == a.Id);
            if (toUpdate != null)
            {
                toUpdate = a;
            }
            else
            {
                throw new Exception(className+"Not found");
            }
        }

        public Anything Find(string Id)
        {
            Anything a = items.Find(i => i.Id == Id);
            if (a != null)
            {
                return a;
            }
            else

            {
                throw new Exception(className + "Not found");
            }
        }

        public IQueryable<Anything> Collection()
        {
            return items.AsQueryable();
        }

        public void Delete(string Id)
        {
            Anything toDelete  = items.Find(i => i.Id == Id);
            if (toDelete != null)
            {
                items.Remove(toDelete);
            }
            else

            {
                throw new Exception(className + "Not found");
            }

        }
    }
}
