using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class TransferMoney
    {
        public int UserId { get; set; }
        public int TransferToId { get; set; }
        public double Amount { get; set; }
        public int TransferId { get; set; }
        public int TransferTypeId { get; set; }
        public int TransferStatusId { get; set; }

        public TransferMoney()
        {

        }


        public TransferMoney(int userId, int transfertoId, double amount)
        {
            UserId = userId;
            TransferToId = transfertoId;
            Amount = amount;
        }

        public TransferMoney(int transfer_id, int transfer_type_id, int transfer_status_id, int userId, int transferToId, double amount)
        {
            UserId = userId;
            TransferToId = transferToId;
            Amount = amount;
            TransferId = transfer_id;
            TransferTypeId = transfer_type_id;
            TransferStatusId = transfer_status_id;
        }

        public override string ToString()
        {
            return TransferId + " | Send | Approved | " + UserId + " | " + TransferToId + " | $" + Amount;
        }
    }
}
