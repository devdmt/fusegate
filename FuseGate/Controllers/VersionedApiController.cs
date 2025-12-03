using Microsoft.AspNetCore.Mvc.Versioning;
namespace EsbJson.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
public class VersionedApiController : BaseApiController
{
}

[Route("api/saf/v{version:apiVersion}/[controller]")]
public class SafaricomApiController : BaseApiController
{
}
[Route("api/claim/v{version:apiVersion}/[controller]")]
public class ClaimApiController : BaseApiController
{

}
[Route("api/financialplanning/v{version:apiVersion}/[controller]")]
public class FinancialPlanningController : BaseApiController
{

}