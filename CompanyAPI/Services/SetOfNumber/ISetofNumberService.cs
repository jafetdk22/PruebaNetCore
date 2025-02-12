namespace CompanyAPI.Services.SetOfNumber
{
    public interface ISetofNumberService
    {
        Task<string> Extract(int number);
        Task<int> CalculateMissingNumber();
    }
}
