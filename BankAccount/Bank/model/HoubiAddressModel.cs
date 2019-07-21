using System;
using BankAccount.Bank.entity;
using MySql.Data.MySqlClient;

namespace BankAccount.Bank.model
{
    public class HoubiAddressModel
    {
        public HoubiAddress FindByAddressAndPrivatekey(string address, string privateKey)
        {
            var conn = ConnectionHelper.GetConnection();
            var cmd = new MySqlCommand(
                "select * from houbi_address where address = @address and private_key = @privateKey", conn);
            cmd.Parameters.AddWithValue("@address", address);
            cmd.Parameters.AddWithValue("@privateKey", privateKey);

            HoubiAddress houbiAddress = null;

            var readerData = cmd.ExecuteReader();
            if (readerData.Read())
            {
                houbiAddress = new HoubiAddress
                {
                    Address = readerData.GetString("address"),
                    PrivateKey = readerData.GetString("private_key"),
                    Balance = readerData.GetDouble("balance")
                };
            }

            ConnectionHelper.CloseConnection();
            return houbiAddress;
        }

        public bool WithdrawAndDeposit(HoubiAddress currentLoggedInAddress, HoubiTransaction houbiTransaction)
        {
            var conn = ConnectionHelper.GetConnection();
            var transaction = conn.BeginTransaction();
            try
            {
                var cmd = new MySqlCommand("select * from houbi_address where address = @address", conn, transaction);
                cmd.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                double currentAddressBalance = 0;
                var readerData = cmd.ExecuteReader();
                if (readerData.Read())
                {
                    currentAddressBalance = readerData.GetDouble("balance");
                }

                readerData.Close();

                if (houbiTransaction.Type == HoubiTransaction.TransactionType.WITHDRAW &&
                    currentAddressBalance < houbiTransaction.Amount)
                {
                    throw new Exception(" KHong du tien trong vi.");
                }

                if (houbiTransaction.Type == HoubiTransaction.TransactionType.WITHDRAW)
                {
                    currentAddressBalance -= houbiTransaction.Amount;
                }
                else if (houbiTransaction.Type == HoubiTransaction.TransactionType.DEPOSIT)
                {
                    currentAddressBalance += houbiTransaction.Amount;
                }

                var updateQuery = ("update houbi_address set balance = @balance where address = @address");
                var sqlCmd = new MySqlCommand(updateQuery, conn, transaction);
                sqlCmd.Parameters.AddWithValue("@balance", currentAddressBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                var updateResultSet = sqlCmd.ExecuteNonQuery();

                var transactionQuery =
                    "insert into houbi_transactions (transaction_id, type, sender_address, receiver_address, amount, createAt, updatedAt, status) " +
                    "values (@id, @type, @senderAddress, @receiverAddress, @amount, @createdAtMLS, @updatedAtMLS, @status)";
                var historyTransactionCmd =
                    new MySqlCommand(transactionQuery, conn, transaction);
                historyTransactionCmd.Parameters.AddWithValue("@id", houbiTransaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@type", houbiTransaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@senderAddress",
                    houbiTransaction.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAddress",
                    houbiTransaction.ReceiverAddress);
                historyTransactionCmd.Parameters.AddWithValue("@amount", houbiTransaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@createdAtMLS", houbiTransaction.CreatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@updatedAtMLS", houbiTransaction.UpdatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@status", houbiTransaction.Status);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();
                Console.WriteLine("query status: " + historyResult);

                if (updateResultSet != 1 || historyResult != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Err: " + e.Message);
                try
                {
                    transaction.Rollback();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    throw;
                }

                return false;
            }

            ConnectionHelper.CloseConnection();
            return true;
        }

        public bool BuyAndSell(HoubiAddress currentLoggedInAddress, HoubiTransaction transactionHb)
        {
            var conn = ConnectionHelper.GetConnection();

            var myTransaction = conn.BeginTransaction();
            try
            {
                var balanceSender = new MySqlCommand("select * from houbi_address where address = @address ",
                    conn, myTransaction);
                balanceSender.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);

                double currentAccountBalance = 0;
                var reader = balanceSender.ExecuteReader();
                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDouble("balance");
                }

                reader.Close();
                if (transactionHb.Type == HoubiTransaction.TransactionType.BUY &&
                    currentAccountBalance < transactionHb.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");
                }

                if (transactionHb.Type == HoubiTransaction.TransactionType.BUY)
                {
                    currentAccountBalance -= (transactionHb.Amount * 200);
                }
                else if (transactionHb.Type == HoubiTransaction.TransactionType.SELL)
                {
                    currentAccountBalance += (transactionHb.Amount * 200);
                }

                var updateQuery = ("update houbi_address set balance = @balance where address = @address");
                var sqlCmd = new MySqlCommand(updateQuery, conn, myTransaction);
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@address", currentLoggedInAddress.Address);
                var updateRs = sqlCmd.ExecuteNonQuery();

                var balanceReceiver = new MySqlCommand("select * from houbi_address where address = @address ",
                    conn, myTransaction);
                balanceReceiver.Parameters.AddWithValue("@address", transactionHb.ReceiverAddress);
                double receiverBalance = 0;
                var readerReceiver = balanceReceiver.ExecuteReader();
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDouble("balance");
                }

                readerReceiver.Close();

                if (transactionHb.Type == HoubiTransaction.TransactionType.BUY)
                {
                    receiverBalance += (transactionHb.Amount * 200);
                }
                else if (transactionHb.Type == HoubiTransaction.TransactionType.SELL)
                {
                    receiverBalance -= (transactionHb.Amount * 200);
                }


                var updateQueryReceiver =
                    ("update houbi_address set balance = @balance where address = @address");
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, conn, myTransaction);
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@address", transactionHb.ReceiverAddress);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();

                var historyTransactionQuery =
                    "insert into houbi_transactions (transaction_id, type, sender_address, receiver_address, amount, createAt, updatedAt, status) " +
                    "values (@id, @type, @senderAddress, @receiverAddress, @amount, @createdAtMLS, @updatedAtMLS, @status)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, conn, myTransaction);
                historyTransactionCmd.Parameters.AddWithValue("@id", transactionHb.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@type", transactionHb.Type);
                historyTransactionCmd.Parameters.AddWithValue("@senderAddress",
                    transactionHb.SenderAddress);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAddress",
                    transactionHb.ReceiverAddress);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transactionHb.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@createdAtMLS", transactionHb.CreatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@updatedAtMLS", transactionHb.UpdatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@status", transactionHb.Status);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();

                if (updateRs != 1 || historyResult != 1 || updateResultReceiver != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                myTransaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                myTransaction.Rollback();
                return false;
            }
            finally
            {
                conn.Close();
            }
        }
    }
}