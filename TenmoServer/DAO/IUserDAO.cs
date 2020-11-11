using System.Collections.Generic;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IUserDAO
    {
        User GetUser(string username);
        User AddUser(string username, string password);
        List<User> GetUsers();
        double GetBalance(int id);
        bool TransferMoneyFrom(int id, double amountToDeduct);
        bool TransferMoneyTo(int id, int idToSendTo, double amountToDeduct);
        List<TransferMoney> ViewTransfer(int id);
        List<TransferMoney> TransferSearch(int transfer_Id);
    }
}
