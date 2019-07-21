namespace BankAccount.Bank.entity
{
    public class SHBAccount
    {
        public string AccountNumber { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }

        public SHBAccount()
        {
        }


        public SHBAccount(string accountNumber, string username, string password, double balance)
        {
            AccountNumber = accountNumber;
            Username = username;
            Password = password;
            Balance = balance;
        }

        public override string ToString()
        {
            return $"Account: {AccountNumber},Username: {Username}, Password: {Password}, Balance: {Balance}";
        }
    }
}