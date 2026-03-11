//using API.Infrastructure.Auth;
//using DAL.ModelView;
//using EsbJson.Controllers;
//using Microsoft.AspNetCore.Mvc;

//namespace EsbJson.API.Controllers.MSure
//{
//    public class PartnersController : VersionNeutralApiController
//    {
//        private readonly IPartnerManager _partnerManager;
//        public PartnersController(IPartnerManager partnerManager)
//        {
//            _partnerManager = partnerManager;
//        }

//        ////    [HttpPost("Auth")]
//        ////    public async Task<IActionResult> AUth([FromBody] UserLoginDTO login)
//        ////    {
//        ////        var token= await _partnerManager.AuthenticatePartner(login);
//        ////        return Ok(token);
//        ////    }

//        [HttpPost("CreatePartner")]
//        public async Task<IActionResult> CreatePartner([FromBody] PartnerDTO parner)
//        {
//            var result = await _partnerManager.CreatePartner(parner);
//            return Ok(result);
//        }

//        [HttpPost("CreatePartnerUser")]
//        public async Task<IActionResult> CreatePartnerUser([FromBody] PartnerUserDTO user)
//        {
//            var result = await _partnerManager.CreatePartnerUser(user);
//            return Ok(result);
//        }
//    }
//}
