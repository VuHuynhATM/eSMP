using eSMP.Models;

namespace eSMP.Services.OrderRepo
{
    public class OrderRepository:IOrderReposity
    {
        private readonly WebContext _context;

        public OrderRepository(WebContext context)
        {
            _context = context;
        }
    }
}
