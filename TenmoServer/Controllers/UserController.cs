using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;

namespace TenmoServer.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ITokenGenerator tokenGenerator;
        private readonly IPasswordHasher passwordHasher;
        private readonly IUserDAO userDAO;

        public UserController(ITokenGenerator _tokenGenerator, IPasswordHasher _passwordHasher, IUserDAO _userDAO)
        {
            tokenGenerator = _tokenGenerator;
            passwordHasher = _passwordHasher;
            userDAO = _userDAO;
        }

        [HttpGet("balance/{id}")]
        public double GetBalance(int id)
        {
            return userDAO.GetBalance(id);
        }

        [HttpGet("list")]
        public List<User> GetUsers()
        {
            return userDAO.GetUsers();
        }

        //[HttpPut("transfer/{id}")]
        //public void TransferMoneyFrom(int id, double amountToTransfer)
        //{
        //    userDAO.TransferMoneyFrom(id, amountToTransfer);
        //}

        [HttpPut("transfer/{id}")]
        public IActionResult TransferMoneyTo(int id, TransferMoney transfer)
        {
            userDAO.TransferMoneyTo(transfer.UserId, transfer.TransferToId, transfer.Amount);
            userDAO.TransferMoneyFrom(transfer.UserId, transfer.Amount);
            // call method to add to transfer table

            return base.NoContent();
        }
        
        [HttpGet("transferList/{id}")]
        public List<TransferMoney> TransferList(int id)
        {
            return userDAO.ViewTransfer(id);
        }

        [HttpGet("transfer/{id}")]
        public List<TransferMoney> Transfer(int id)
        {
            return userDAO.TransferSearch(id);
        }
    }
}