using System;
using BankAccount.Bank.entity;
using BankAccount.Bank.model;

namespace BankAccount.Bank
{
    public class GiaoDichBlockchain : Program.GiaoDich
    {
        private static HoubiAddressModel _addressModel;

        public GiaoDichBlockchain()
        {
            _addressModel = new HoubiAddressModel();
        }


        public void Login()
        {
            Program.currentLoggedInAddress = null;
            Console.Clear();
            Console.WriteLine("Dang nhap he thong Houbi in Blockchain.");
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine("Vui long nhap dia chi vi : ");
            var address = Console.ReadLine();
            Console.WriteLine("Vui long nhap mat khau : ");
            var privateKey = Console.ReadLine();

            var houbiAddress = _addressModel.FindByAddressAndPrivatekey(address, privateKey);
            if (houbiAddress == null)
            {
                Console.WriteLine("Sai thong tin dang nhap, vui long dang nhap lai.");
                Console.WriteLine("An de tiep tuc.");
                Console.ReadLine();
                return;
            }

            Program.currentLoggedInAddress = houbiAddress;
        }

        public void RutTien()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Vui lòng nhập số tiền cần rút :");
                var Amount = Double.Parse(Console.ReadLine());
                if (Amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var transaction = new HoubiTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Type = HoubiTransaction.TransactionType.WITHDRAW,
                    Amount = Amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                if (_addressModel.WithdrawAndDeposit(Program.currentLoggedInAddress, transaction))
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
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Vui lòng nhập số tiền cần gui :");
                var Amount = Double.Parse(Console.ReadLine());
                if (Amount <= 0)
                {
                    Console.WriteLine("Số lượng không hợp lệ, vui lòng thử lại.");
                    return;
                }

                var transaction = new HoubiTransaction
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = Program.currentLoggedInAddress.Address,
                    Type = HoubiTransaction.TransactionType.DEPOSIT,
                    Amount = Amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };
                if (_addressModel.WithdrawAndDeposit(Program.currentLoggedInAddress, transaction))
                {
                    Console.WriteLine("Giao dịch thành công.");
                }
            }

            else
            {
                Console.WriteLine("Vui lòng đăng nhập để sử dụng chức năng này.");
            }
        }

        public void Mua()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Vui lòng nhập dia chi mua coin: ");
                var address = Console.ReadLine();
                Console.WriteLine("Nhập số coin can mua : ");
                var Amount = Double.Parse(Console.ReadLine());
                var transactionHB = new HoubiTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Type = HoubiTransaction.TransactionType.BUY,
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = address,
                    Amount = Amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };

                if (_addressModel.BuyAndSell(Program.currentLoggedInAddress, transactionHB))
                {
                    Console.WriteLine("Giao dịch thành công.");
                }
                else
                {
                    Console.WriteLine("Giao dịch thất bại, vui lòng thử lại.");
                }
            }
        }

        public void Ban()
        {
            if (Program.currentLoggedInAddress != null)
            {
                Console.Clear();
                Console.WriteLine("Vui lòng nhập địa chỉ bán coin: ");
                var address = Console.ReadLine();
                Console.WriteLine("Nhập số coin cần bán : ");
                var Amount = Double.Parse(Console.ReadLine());
                var transactionHB = new HoubiTransaction()
                {
                    TransactionId = Guid.NewGuid().ToString(),
                    Type = HoubiTransaction.TransactionType.SELL,
                    SenderAddress = Program.currentLoggedInAddress.Address,
                    ReceiverAddress = address,
                    Amount = Amount,
                    CreatedAtMLS = DateTime.Now.Ticks,
                    UpdatedAtMLS = DateTime.Now.Ticks,
                    Status = 1
                };

                if (_addressModel.BuyAndSell(Program.currentLoggedInAddress, transactionHB))
                {
                    Console.WriteLine("Giao dịch thành công.");
                }
                else
                {
                    Console.WriteLine("Giao dịch thất bại, vui lòng thử lại.");
                }
            }
        }

        public void ChuyenKhoan()
        {
            throw new NotImplementedException();
        }
    }
}