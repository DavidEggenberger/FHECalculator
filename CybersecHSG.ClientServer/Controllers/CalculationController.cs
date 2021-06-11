using CybersecHSG.Common.Models;
using CybersecHSG.Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CybersecHSG.API.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class CalculationController : ControllerBase
    {
        private readonly SEALContext _sealContext;
        private Evaluator _evaluator;
        public CalculationController()
        {
            _sealContext = SEALUtils.GetContext();
            _evaluator = new Evaluator(_sealContext);
        }
        [HttpPost("")]
        public ActionResult<SummaryItem> AddRunItem(CalculationItem request)
        {
            var weight = SEALUtils.BuildCiphertextFromBase64String(request.Weight, _sealContext);
            List<(Ciphertext METs, Ciphertext Duration)> activities = request.Activities
                .Select(a => (SEALUtils.BuildCiphertextFromBase64String(a.METs, _sealContext), SEALUtils.BuildCiphertextFromBase64String(a.Duration, _sealContext)))
                .ToList();

            Ciphertext[] activitiesCalculated = Enumerable.Range(0, activities.Count).Select(s => new Ciphertext()).ToArray();
            for (int i = 0; i < activities.Count; i++)
            {
                Ciphertext ciphertextTemp = new Ciphertext();
                _evaluator.Multiply(activities[i].METs, activities[i].Duration, ciphertextTemp);
                _evaluator.Multiply(ciphertextTemp, weight, activitiesCalculated[i]);
            }

            Ciphertext totalCalories = SumEncryptedValues(activitiesCalculated);

            return Ok(new SummaryItem { TotalKalories = SEALUtils.CiphertextToBase64String(totalCalories) });
        }
        private Ciphertext SumEncryptedValues(IEnumerable<Ciphertext> encryptedData)
        {
            Ciphertext encTotal = new Ciphertext();
            _evaluator.AddMany(encryptedData, encTotal);
            return encTotal;
        }
    }
}
