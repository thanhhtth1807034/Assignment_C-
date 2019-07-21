namespace BankAccount.Bank.entity
{
    public class HoubiTransaction
    {
        public string TransactionId { get; set; }
        public TransactionType Type { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverAddress { get; set; }
        public double Amount { get; set; }
        public long CreatedAtMLS { get; set; }
        public long UpdatedAtMLS { get; set; }
        public int Status { get; set; }

        public HoubiTransaction()
        {
        }

        public enum TransactionType
        {
            WITHDRAW = 1,
            DEPOSIT = 2,
            BUY = 3,
            SELL = 4
        }

        public HoubiTransaction(string transactionId, TransactionType type, string senderAddress,
            string receiverAddress, double amount, long createdAtMls, long updatedAtMls, int status)
        {
            TransactionId = transactionId;
            Type = type;
            SenderAddress = senderAddress;
            ReceiverAddress = receiverAddress;
            Amount = amount;
            CreatedAtMLS = createdAtMls;
            UpdatedAtMLS = updatedAtMls;
            Status = status;
        }
    }
}