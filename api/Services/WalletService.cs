public class WalletService
{
    private readonly IUnitOfWork _unitofwork;
    public WalletService(IUnitOfWork unitOfWork)
    {
        _unitofwork = unitOfWork;
    }

    
}
