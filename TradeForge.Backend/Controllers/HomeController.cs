using System.Net;
using Microsoft.AspNetCore.Mvc;
using TradeForge.Backend.Data.Responses;

namespace TradeForge.Backend.Controllers;

[ApiController]
[Route("api")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new BaseResponse((int)HttpStatusCode.OK, "Connection is OK", ""));
}