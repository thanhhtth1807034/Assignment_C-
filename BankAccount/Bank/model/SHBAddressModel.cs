using System;
using System.Data;
using BankAccount.Bank.entity;
using MySql.Data.MySqlClient;

namespace BankAccount.Bank.model
{
    public class SHBAddressModel
    {
        public SHBAccount FindByUsernameandPassword(string username, string password)
        {
            var cmd = new MySqlCommand("select * from accounts where Username = @username and Password = @password",
                ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password);
            SHBAccount shbAccount = null;
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                shbAccount = new SHBAccount
                {
                    AccountNumber = reader.GetString("AccountNumber"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    Balance = reader.GetDouble("Balance")
                };
            }

            reader.Close();
            return shbAccount;
        }

        public bool UpdateBalance(SHBAccount currentLoggedInAccount, SHBTransaction transaction)
        {
            var trans = ConnectionHelper.GetConnection().BeginTransaction();
            try
            {
                var cmd = new MySqlCommand("select * from accounts where Username = @username ",
                    ConnectionHelper.GetConnection());
                cmd.Parameters.AddWithValue("@username", currentLoggedInAccount.Username);
                double currentAccountBalance = 0;
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDouble("Balance");
                }

                reader.Close();

                if (transaction.Type == SHBTransaction.TransactionType.WITHDRAW &&
                    currentAccountBalance < transaction.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");
                }

                if (transaction.Type == SHBTransaction.TransactionType.WITHDRAW)
                {
                    currentAccountBalance -= transaction.Amount;
                }
                else if (transaction.Type == SHBTransaction.TransactionType.DEPOSIT)
                {
                    currentAccountBalance += transaction.Amount;
                }

                var updateQuery = ("update accounts set Balance = @balance where Username = @username");
                var sqlCmd = new MySqlCommand(updateQuery, ConnectionHelper.GetConnection());
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@username", currentLoggedInAccount.Username);
                var updateRs = sqlCmd.ExecuteNonQuery();

                var transactionQuery =
                    "insert into shbtransaction (transaction_id, type, sender_account_number, receiver_account_number, amount, message, createdAt, updatedAt, status) " +
                    "values (@id, @type, @senderAccountNumber, @receiverAccountNumber, @amount, @message, @createdAtMLS, @updatedAtMLS, @status)";
                var historyTransactionCmd =
                    new MySqlCommand(transactionQuery, ConnectionHelper.GetConnection());
                historyTransactionCmd.Parameters.AddWithValue("@id", transaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@type", transaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    transaction.SenderAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    transaction.ReceiverAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@amount", transaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@message", transaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@createdAtMLS", transaction.CreatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@updatedAtMLS", transaction.UpdatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@status", transaction.Status);
                var historyResult = historyTransactionCmd.ExecuteNonQuery();
                Console.WriteLine("query status: " + historyResult);

                if (updateRs != 1 || historyResult != 1)
                {
                    throw new Exception("Không thể thêm giao dịch hoặc update tài khoản.");
                }

                trans.Commit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Err: " + e.Message);
                try
                {
                    trans.Rollback();
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

        public bool Tranfer(SHBAccount currentLoggedInAccount, SHBTransaction shbTransaction)
        {
            var conn = ConnectionHelper.GetConnection();

            var myTransaction = conn.BeginTransaction();
            try
            {
                var balanceSender = new MySqlCommand("select * from accounts where AccountNumber = @AccountNumber ",
                    conn);
                balanceSender.Parameters.AddWithValue("@AccountNumber", currentLoggedInAccount.AccountNumber);
                double currentAccountBalance = 0;
                var reader = balanceSender.ExecuteReader();
                if (reader.Read())
                {
                    currentAccountBalance = reader.GetDouble("Balance");
                }

                reader.Close();
                if (shbTransaction.Type == SHBTransaction.TransactionType.TRANFER &&
                    currentAccountBalance < shbTransaction.Amount)
                {
                    throw new Exception("Không đủ tiền trong tài khoản.");
                }

                currentAccountBalance -= shbTransaction.Amount;


                var updateQuery = ("update accounts set Balance = @balance where AccountNumber = @AccountNumber");
                var sqlCmd = new MySqlCommand(updateQuery, conn);
                sqlCmd.Parameters.AddWithValue("@balance", currentAccountBalance);
                sqlCmd.Parameters.AddWithValue("@AccountNumber", currentLoggedInAccount.AccountNumber);
                var updateRs = sqlCmd.ExecuteNonQuery();

                var balanceReceiver = new MySqlCommand("select * from accounts where AccountNumber = @AccountNumber ",
                    conn);
                balanceReceiver.Parameters.AddWithValue("@AccountNumber", shbTransaction.ReceiverAccountNumber);
                double receiverBalance = 0;
                var readerReceiver = balanceReceiver.ExecuteReader();
                if (readerReceiver.Read())
                {
                    receiverBalance = readerReceiver.GetDouble("balance");
                }

                readerReceiver.Close();

                receiverBalance += shbTransaction.Amount;


                var updateQueryReceiver =
                    ("update accounts set Balance = @balance where AccountNumber = @AccountNumber");
                var sqlCmdReceiver = new MySqlCommand(updateQueryReceiver, conn);
                sqlCmdReceiver.Parameters.AddWithValue("@balance", receiverBalance);
                sqlCmdReceiver.Parameters.AddWithValue("@AccountNumber", shbTransaction.ReceiverAccountNumber);
                var updateResultReceiver = sqlCmdReceiver.ExecuteNonQuery();

                var historyTransactionQuery =
                    "insert into shbtransaction (transaction_id, type, sender_account_number, receiver_account_number, amount, message, createdAt, updatedAt, status) " +
                    "values (@id, @type, @senderAccountNumber, @receiverAccountNumber, @amount, @message, @createdAtMLS, @updatedAtMLS, @status)";
                var historyTransactionCmd =
                    new MySqlCommand(historyTransactionQuery, conn);
                historyTransactionCmd.Parameters.AddWithValue("@id", shbTransaction.TransactionId);
                historyTransactionCmd.Parameters.AddWithValue("@type", shbTransaction.Type);
                historyTransactionCmd.Parameters.AddWithValue("@senderAccountNumber",
                    shbTransaction.SenderAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@receiverAccountNumber",
                    shbTransaction.ReceiverAccountNumber);
                historyTransactionCmd.Parameters.AddWithValue("@amount", shbTransaction.Amount);
                historyTransactionCmd.Parameters.AddWithValue("@message", shbTransaction.Message);
                historyTransactionCmd.Parameters.AddWithValue("@createdAtMLS", shbTransaction.CreatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@updatedAtMLS", shbTransaction.UpdatedAtMLS);
                historyTransactionCmd.Parameters.AddWithValue("@status", shbTransaction.Status);
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

        public SHBAccount GetAccountByAccountNumber(string accountNumber)
        {
            var queryString = "select * from `accounts` where `AccountNumber` = @accountNumber ";
            var cmd = new MySqlCommand(queryString, ConnectionHelper.GetConnection());
            cmd.Parameters.AddWithValue("@accountNumber", accountNumber);
            var reader = cmd.ExecuteReader();
            SHBAccount account = null;
            if (reader.Read())
            {
                account = new SHBAccount()
                {
                    AccountNumber = reader.GetString("AccountNumber"),
                    Username = reader.GetString("Username"),
                    Password = reader.GetString("Password"),
                    Balance = reader.GetDouble("Balance"),
                };
            }

            reader.Close();
            ConnectionHelper.CloseConnection();
            return account;
        }
    }
}