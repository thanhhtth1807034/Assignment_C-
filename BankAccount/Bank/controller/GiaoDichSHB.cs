using System;
using BankAccount.Bank.entity;
using BankAccount.Bank.model;

namespace BankAccount.Bank
{
    
    public class GiaoDichSHB : Program.GiaoDich
    {
        private SHBAddressModel shbAccountModel ;

        public GiaoDichSHB()
        {
            shbAccountModel = new SHBAddressModel();
        }
        public void RutTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Vui lòng nhập số tiền cần rút :");
                var Amount = Double.Parse(Console.ReadLine());
                if (Amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }
                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    Type = SHBTransaction.TransactionType.WITHDRAW,
                    Amount = Amount,
                    Message = "Tiến hành rút tiền tại ATM với số tiền: " + Amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                if (shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction))
                {
                    Console.WriteLine("Giao dịch thành công.");
                }
            }
            
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }

        }

        public void GuiTien()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Vui lòng nhập số tiền cần gui :");
                var Amount = Double.Parse(Console.ReadLine());
                
                var transaction = new SHBTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    Type = SHBTransaction.TransactionType.DEPOSIT,
                    Amount = Amount,
                    Message = "Tiến hành gui tiền tại ATM với số tiền: " + Amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                if (shbAccountModel.UpdateBalance(Program.currentLoggedInAccount, transaction))
                {
                    Console.WriteLine("Giao dịch thành công.");
                }
                else
                {
                    Console.WriteLine("Giao dịch eos thành công.");
                }
            }
            
            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }

        public void ChuyenKhoan()
        {
            if (Program.currentLoggedInAccount != null)
            {
                Console.Clear();
                Console.WriteLine("Vui lòng nhập số tài khoản chuyển tiền: ");
                var accountNumber = Console.ReadLine();
                var receiverAccount = shbAccountModel.GetAccountByAccountNumber(accountNumber);
                if (receiverAccount == null)
                {
                    Console.WriteLine("Tài khoản nhận tiền không tồn tại hoặc đã bị khoá.");
                    return;
                }
                Console.WriteLine("Tài khoản nhận tiền: " + accountNumber);
                Console.WriteLine("Chủ tài khoản: " + receiverAccount.Username);
                Console.WriteLine("Nhập số tiền chuyển khoản: ");
                var Amount = Double.Parse(Console.ReadLine());
                Console.WriteLine("Nhập nội dung giao dịch: ");
                var message = Console.ReadLine();
                var transactionShb = new SHBTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Type = SHBTransaction.TransactionType.TRANFER,
                    SenderAccountNumber = Program.currentLoggedInAccount.AccountNumber,
                    ReceiverAccountNumber = accountNumber,
                    Amount = Amount,
                    Message = "Tiến hành chuyen khoan với số tiền: " + Amount + ", Noi dung: " + message,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };

                if (shbAccountModel.Tranfer(Program.currentLoggedInAccount, transactionShb))
                {
                    Console.WriteLine("Giao dịch thành công.");
                }
                else
                {
                    Console.WriteLine("Giao dịch thất bại, vui lòng thử lại.");
                }
            }
        }

        public void Login()
        {
            Console.Clear();
            Program.currentLoggedInAccount = null;
            Console.Clear();
            Console.WriteLine("Vui lòng nhập usename: ");
            var Username = Console.ReadLine();
            Console.WriteLine("Vui lòng nhập mật khẩu: ");
            var Password = Console.ReadLine();
            var shbAccount = shbAccountModel.FindByUsernameandPassword(Username, Password );
            if (shbAccount == null)
            {
                Console.WriteLine("Sai thông tin tài khoản, vui lòng đăng nhập lại.");
                Console.WriteLine("Ấn phím bất kỳ để tiếp tục.");
                Console.Read();
                return;
            }

            Program.currentLoggedInAccount = shbAccount;
        }

        public void Mua()
        {
            throw new NotImplementedException();
        }

        public void Ban()
        {
            throw new NotImplementedException();
        }
    }
}