public class CreditSummaryDto
{
    public decimal TotalCredit { get; set; }  // sum of Credit type
    public decimal TotalDebt { get; set; }    // sum of Debt type
    public decimal TotalPaid { get; set; }    // optional: sum of PaidAmount
}
