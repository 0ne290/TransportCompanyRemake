namespace YooKassa.Entities.Payment;

public class UnissuedPayment
{
    public UnissuedPayment(Amount amount, bool capture, dynamic confirmation, string description)
    {
        if (confirmation is not Confirmation)
            throw new TypeAccessException("Confirmation type is invalid.");
    }
}