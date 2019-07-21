namespace BankAccount.Bank.entity
{
    public class HoubiAddress
    {
        public string Address { get; set; }
        public string PrivateKey { get; set; }
        public double Balance { get; set; }

        public HoubiAddress()
        {
        }

        public HoubiAddress(string address, string privateKey, double balance)
        {
            Address = address;
            PrivateKey = privateKey;
            Balance = balance;
        }
    }
}