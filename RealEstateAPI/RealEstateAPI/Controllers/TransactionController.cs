using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.Data;
using RealEstateAPI.Model;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]

    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionRepository _transactionRepository;

        public TransactionController(TransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }


        #region GetAllTransaction
        [HttpGet]

        public IActionResult GetAllTransaction()
        {
            var transactions = _transactionRepository.SelectAllTransaction();
            return Ok(transactions);
        }
        #endregion

        #region GetTransactionByID

        [HttpGet("{TransactionID}")]

        public IActionResult GetTransactionByID(int TransactionID)
        {
            var transactions = _transactionRepository.SelectTransactionByID(TransactionID);
            return Ok(transactions);
        }
        #endregion

        #region InsertTransaction
        [HttpPost]
        public IActionResult InsertTransaction(TransactionModel transaction)
        {
            var transactions = _transactionRepository.InsertTransaction(transaction);
            if (!transactions)
            {
                return BadRequest("Failed to insert the transaction.");
            }
            return Ok(transaction); // Returning the user model for confirmation
        }
        #endregion

        #region UpdateTransaction

        [HttpPut("{transactionID}")]
        public IActionResult UpdateTransaction(int transactionID, TransactionModel transaction)
        {
            // Ensure the UserID from URL matches the UserID in the model
            if (transactionID != transaction.TransactionID)
            {
                return BadRequest("TransactionID mismatch.");
            }

            var transactions = _transactionRepository.UpdateTransaction(transaction);
            if (!transactions)
            {
                return BadRequest("Failed to update Transaction.");
            }

            return Ok(transactions);
        }
        #endregion

        #region DeleteTransaction

        [HttpDelete("{TransactionID}")]
        public IActionResult DeleteTransaction(int TransactionID)
        {
            var transactions = _transactionRepository.DeleteTransaction(TransactionID);
            if (!transactions)
            {
                return BadRequest();
            }
            return NoContent();
        }
        #endregion
    }
}
