using System.Net;
using Microsoft.AspNetCore.Mvc;
using TradeForge.Backend.Data.Responses;

namespace TradeForge.Backend.Controllers;

[ApiController]
[Route("")]
public class DefaultController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Redirect("/api");
}