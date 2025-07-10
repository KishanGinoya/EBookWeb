using Microsoft.EntityFrameworkCore;

namespace BookShoppingWebUI.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _db;
        public StockRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<Stock?> GetStockById(int bookId)=>await 
            _db.Stocks.FirstOrDefaultAsync(x => x.BookId == bookId);

        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm="")
        {
            var stocks = await (from book in _db.Books
                                join stock in _db.Stocks
                                on book.Id equals stock.BookId
                                into book_stocks
                                from bookstock in book_stocks.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) || book.BookName.ToLower().Contains(sTerm.ToLower())
                                select new StockDisplayModel
                                {
                                    BookId = book.Id,
                                    BookName = book.BookName,
                                    Quantity = bookstock == null ? 0 : bookstock.Quantity
                                }).ToListAsync();
            return stocks;
        }
        public async Task ManageStock(StockDTO stockToManage)
        {
            var existingStock=await GetStockById(stockToManage.BookId);
            if(existingStock == null)
            {
                var stock=new Stock { 
                    BookId= stockToManage.BookId, Quantity = stockToManage.Quantity 
                    
                };
                _db.Stocks.Add(stock);

            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }
            await _db.SaveChangesAsync();
        }
    }
}
