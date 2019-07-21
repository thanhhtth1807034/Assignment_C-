using System;
using BankAccount.Bank;
using BankAccount.Bank.entity;

namespace BankAccount
{
    class Program
    {
        public static SHBAccount currentLoggedInAccount;
        public static HoubiAddress currentLoggedInAddress;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                GiaoDich giaoDich = null;
                Console.WriteLine(" S H B B A N K ");
                Console.WriteLine("============================");
                Console.WriteLine("1. Giao dịch SHB bank.");
                Console.WriteLine("2. Giao dịch Blockchain.");
                Console.WriteLine("============================================");
                Console.WriteLine("Nhập lựa chọn của bạn: ");
                var ch = int.Parse(Console.ReadLine());
                switch (ch)
                {
                    case 1:
                        giaoDich = new GiaoDichSHB();
                        break;
                    case 2:
                        giaoDich = new GiaoDichBlockchain();
                        break;
                    default:
                        Console.WriteLine("Sai phương thức đăng nhập.");
                        break;
                }

                giaoDich.Login();
                if (currentLoggedInAccount != null)
                {
                    Console.WriteLine("Đăng nhập thành công với tài khoản.");
                    Console.WriteLine($"Tài khoản: {currentLoggedInAccount.Username}");
                    Console.WriteLine($"Số dư: {currentLoggedInAccount.Balance}");
                    Console.WriteLine("Ấn phím bất kỳ để tiếp tục giao dịch.");
                    Console.ReadLine();
                    GenerateTransactionMenu(giaoDich);
                }

                if (currentLoggedInAddress != null)
                {
                    Console.WriteLine("Dang nhap thanh cong voi dia chi vi MyEtherWallet.");
                    Console.WriteLine($"Address: {currentLoggedInAddress.Address}");
                    Console.WriteLine($"So du: {currentLoggedInAddress.Balance}");
                    Console.WriteLine("Enter de tiep tuc.");
                    Console.ReadLine();
                    GenerateCryptoTransactionMenu(giaoDich);
                }
            }
        }

        private static void GenerateTransactionMenu(GiaoDich giaoDich)
        {
            while (true)
            {
                Console.WriteLine("Vui lòng lựa chọn kiểu giao dịch: ");
                Console.WriteLine("============================================");
                Console.WriteLine("1. Rút tiền.");
                Console.WriteLine("2. Gửi tiền.");
                Console.WriteLine("3. Chuyển khoản.");
                Console.WriteLine("4. Thoát.");
                Console.WriteLine("============================================");
                Console.WriteLine("Nhập lựa chọn của bạn: ");
                var choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        giaoDich.RutTien();
                        break;
                    case 2:
                        giaoDich.GuiTien();
                        break;
                    case 3:
                        giaoDich.ChuyenKhoan();
                        break;
                    case 4:
                        Console.WriteLine("Thoát giao diện giao dịch.");
                        break;
                    default:
                        Console.WriteLine("Lựa chọn sai.");
                        break;
                }

                if (choice == 4)
                {
                    break;
                }
            }
        }

        private static void GenerateCryptoTransactionMenu(GiaoDich giaoDich)
        {
            while (true)
            {
                Console.WriteLine("Vui lòng lựa chọn kiểu giao dịch: ");
                Console.WriteLine("============================================");
                Console.WriteLine("1. Rút tiền.");
                Console.WriteLine("2. Gửi tiền.");
                Console.WriteLine("3. Mua.");
                Console.WriteLine("4. Bán.");
                Console.WriteLine("5. Thoát.");
                Console.WriteLine("============================================");
                Console.WriteLine("Nhập lựa chọn của bạn: ");
                var choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        giaoDich.RutTien();
                        break;
                    case 2:
                        giaoDich.GuiTien();
                        break;
                    case 3:
                        giaoDich.Mua();
                        break;
                    case 4:
                        giaoDich.Ban();
                        break;
                    case 5:
                        Console.WriteLine("Thoát giao diện giao dịch.");
                        break;
                    default:
                        Console.WriteLine("Lựa chọn sai.");
                        break;
                }

                if (choice == 5)
                {
                    break;
                }
            }
        }

        public interface GiaoDich
        {
            void RutTien();
            void GuiTien();
            void ChuyenKhoan();
            void Login();
            void Mua();
            void Ban();
        }
    }
}