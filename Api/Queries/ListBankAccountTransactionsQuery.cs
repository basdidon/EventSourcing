namespace Api.Queries
{
    public record ListBankAccountTransactionsQuery(Guid AccountId,int Page,int PageSize);
}
