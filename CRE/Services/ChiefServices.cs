using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class ChiefServices : IChiefServices
    {
        private readonly CREdbContext _context;
        public ChiefServices(CREdbContext context)
        {
            _context = context;
        }
    }
}
