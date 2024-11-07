using CRE.Data;
using CRE.Interfaces;

namespace CRE.Services
{
    public class ExpertiseServices: IExpertiseServices
    {
        private readonly CREdbContext _context;
        public ExpertiseServices(CREdbContext context)
        {
            _context = context;
        }
    }
}
