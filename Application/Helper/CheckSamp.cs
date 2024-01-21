using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Application.Helper
{
    public class CheckSamp
    {
        private readonly ApplicationDbContext _context;

        public CheckSamp(ApplicationDbContext context)
        {
            _context = context;
        }

        /*public bool CheckifExists(int id)
        {
            //Write the generic code here, for example:
            return _context.Set<T>().Any(w=> w.);
        }*/
    }
}
