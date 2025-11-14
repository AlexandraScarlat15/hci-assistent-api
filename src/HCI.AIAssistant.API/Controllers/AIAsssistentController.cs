using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using HCI.AIAssistant.API.Models.DTOs.AIAssistentController;
using System.Reflection.Metadata.Ecma335;


namespace HCI.AIAssistant.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AIAsssistentController : ControllerBase
{
    [HttpPost("/message")]
    // public async Task<ActionResult<AIAssistentControllerPostMessageResponseDTO>>PostMessageAsync([FromBody] AIAssistantControllerPostMessageRequestDTO request)
    // [
    //     TextMessage = $"You said: {request.TextMessage}"
    // ];
    // return Ok(response);

    public async Task<ActionResult<AIAssistentControllerPostMessageResponseDTO>> PostMessageAsync([FromBody] AIAssistentControllerPostMessageRequestDTO request)
    {
        // Here you would typically process the request and generate a response.
        AIAssistentControllerPostMessageResponseDTO response = new AIAssistentControllerPostMessageResponseDTO
        {
            TextMessage = $"You said: {request.TextMessage}"
        };

        return Ok(response);
    }
}
