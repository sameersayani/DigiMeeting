using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiMeeting.API.DTOs
{
    public class TeamRequest: BaseRequest
    {
        public required List<string> Email { get; set; } 
    }
}