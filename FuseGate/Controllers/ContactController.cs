using API.Infrastructure.OpenApi;
using DAL.ModelView.Pension;
using EsbJson.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FuseGate.Controllers
{
    public class ContactController : BaseApiController
    {

        public ContactController()
        {

        }

        [HttpGet("GetContacts")]
        [ProducesResponseType(typeof(ContactResponseDTO), 200)]
         [PartnerCodeHeader]
        public async Task<IActionResult> ContactUs()
        {
            // Here you would typically process the contact request,
            // such as saving it to a database or sending an email.
            // For demonstration purposes, we'll just return a success response.
            return Ok(new { Message = "Your message has been received. We will get back to you shortly." });
        }

        [HttpPost("ContactUs")]
        [ProducesResponseType(typeof(string), 200)]
        [PartnerCodeHeader]
        public async Task<IActionResult> ContactUs([FromBody] ContactUsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Here you would typically process the contact request,
            // such as saving it to a database or sending an email.
            // For demonstration purposes, we'll just return a success response.
            return Ok(new { Message = "Your message has been received. We will get back to you shortly." });
        }
    }
}
