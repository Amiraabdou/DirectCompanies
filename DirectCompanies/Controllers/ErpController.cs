using DirectCompanies.Dtos;
using DirectCompanies.Enums;
using DirectCompanies.Models;
using DirectCompanies.Security;
using DirectCompanies.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DirectCompanies.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [ApiKeyAuth]

    public class ErpController : ControllerBase
    {
        private readonly ISetupKeyValueService _setupKeyValueService;
        private readonly IUserService _userService;
        private readonly IEmployeeService _employeeService;
        private readonly IOutBoxEventService _outBoxEventService;

        public ErpController(ISetupKeyValueService setupKeyValueService, IUserService userService, IEmployeeService employeeService,IOutBoxEventService outBoxEventService)
        {
            _setupKeyValueService = setupKeyValueService;
            _userService = userService;
            _employeeService = employeeService;
            _outBoxEventService = outBoxEventService;
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSetup([FromBody] List<string> jsonData)
        {
            List<SetupKeyValueDto> Dtos = new();
            jsonData.ForEach(c => { Dtos.Add(JsonSerializer.Deserialize<SetupKeyValueDto>(c)); });
            await _setupKeyValueService.HandleSetup(Dtos);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateUsers([FromBody] List<string> jsonData)
        {
            List<ApplicationUserDto> Dtos = new();

            jsonData.ForEach(c => { Dtos.Add(JsonSerializer.Deserialize<ApplicationUserDto>(c)); });
            await _userService.HandleUsersSentFromErp(Dtos);
            return Ok();
        }
        public async Task<IActionResult> GetPendingOutboxEvents(string DocumentId)
        {
            var Data = await _outBoxEventService.GetPendingOutboxEvents(int.Parse(DocumentId));
            return Ok(Data);


        }
        [HttpPost]
        public async Task<IActionResult> ChangeOutBoxIsSent([FromBody] List<int> Ids)
        {
            await _outBoxEventService.ChangeOutBoxIsSent(Ids);
            return Ok();
        }
    }   }
