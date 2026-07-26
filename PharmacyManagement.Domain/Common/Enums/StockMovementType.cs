namespace PharmacyManagement.Domain.Common.Enums;

public enum StockMovementType
{
    PURCHASE = 1,
    SALE = 2,
    ADJUSTMENT = 3,
    RETURN_TO_SUPPLIER = 4,
    RETURN_FROM_CUSTOMER = 5,
    EXPIRED = 6,
    TRANSFER = 7
}
