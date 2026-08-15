using System;
using System.Collections.Generic;
using System.Text;

namespace XOGame.Shared
{
    public record CreateRoomResult(bool IsSuccess , Room? Room = null , string? Error = null);
}
