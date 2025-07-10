namespace BookShoppingWebUI.Repository
{
    public interface IStockRepository
    {
        Task<Stock?> GetStockById(int bookId);
        Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "");
        Task ManageStock(StockDTO stockToManage);
    }
}