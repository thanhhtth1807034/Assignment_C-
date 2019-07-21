using System;

namespace BankAccount.Bank.entity
{
    public class SHBTransaction
    {
        public string TransactionId { get; set; }
        public TransactionType Type { get; set; } //1.withdarw , 2.deposit , 3. tranfer
        public string SenderAccountNumber { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public double Amount { get; set; }
        public string Message { get; set; }
        public long CreatedAtMLS { get; set; }
        public long UpdatedAtMLS { get; set; }
        public int Status { get; set; }

        public SHBTransaction()
        {
        }
        
        public enum TransactionType
        {
            WITHDRAW = 1,
            DEPOSIT = 2,
            TRANFER = 3
        }
        
        

        public SHBTransaction(string transactionId, TransactionType type, string senderAccountNumber, string receiverAccountNumber, double amount, string message, long createdAtMls, long updatedAtMls, int status)
        {
            TransactionId = transactionId;
            Type = type;
            SenderAccountNumber = senderAccountNumber;
            ReceiverAccountNumber = receiverAccountNumber;
            Amount = amount;
            Message = message;
            CreatedAtMLS = createdAtMls;
            UpdatedAtMLS = updatedAtMls;
            Status = status;
        }
    }
}