using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigiMeeting.API.DTOs
{
    public class RoomRequest: BaseRequest
    {
        public int Capacity { get; set; }
    }
}