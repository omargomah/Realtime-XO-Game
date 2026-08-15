using Microsoft.AspNetCore.SignalR;
using XOGame.Shared;

namespace XOGame.Hubs
{
    public class XoGameHub:Hub<IXoGameHub>
    {
        private static readonly List<Room> _rooms = new();
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"new user Connect with connection Id: `{Context.ConnectionId}` ");

            await Clients.Caller.ShowRooms(_rooms.OrderBy(x => x.Name).ToList());            

            await base.OnConnectedAsync();
        }
        public async Task<CreateRoomResult> CreateRoom(string roomName,string playerName)
        {
            Room? room = _rooms.SingleOrDefault(x => string.Equals(x.Name, roomName, StringComparison.OrdinalIgnoreCase));
            if (room is null)
            {
                room = new Room()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roomName.Trim()
                };
                _rooms.Add(room);
                Player player = new Player()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = playerName,
                    ConnectionId =Context.ConnectionId
                };
                room.AddPlayer(player);
                await Clients.All.ShowRooms(_rooms.OrderBy(x => x.Name).ToList());
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Id);
                return new(true,room);            
            }
            return new(false,null,"The Room Already Exist Join It Instead of Create or use another name");            
        }
        public async Task<CreateRoomResult> JoinRoom(string roomId, string playerName)
        {
            Room? roomToJoinIt = _rooms.SingleOrDefault(x => x.Id == roomId);
            if (roomToJoinIt is null)
                return new(false, Error: "Invalid Room Id");

            Player player = new Player()
            {
                Id = Guid.NewGuid().ToString(),
                Name = playerName,
                ConnectionId = Context.ConnectionId
            };

            if (!roomToJoinIt.AddPlayer(player))
                return new(false, Error: "The Game is already full or you exist in the Room");

            // 1. Add player to SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);

            // 3. Optional: Notify the main page room list that player count changed
            await Clients.All.ShowRooms(_rooms.OrderBy(x => x.Name).ToList());

            // 2. Notify EVERYONE in the room (including Player 1) with the updated Room model
            await Clients.Group(roomId).ChangeMyRoomStatus(roomToJoinIt);


            return new(true, Room: roomToJoinIt);
        }
        public async Task StartGame(string roomId)
        {
            Room room = _rooms.Single(x => x.Id == roomId);
            room.Game.StartNewGame();
            await Clients.Group(room.Id).ChangeMyRoomStatus(room);
        }

        public async Task MakeMove(int r  ,int c ,string currentPlayerId,string roomId)
        { 
            Room room = _rooms.Single(x => x.Id == roomId);
            room.Game.Board[r][c] = room.Game.CurrentSymbolRound;

            for (int i = 0; i < 3; i++)
            {
                if (room.Game.Board[i].All(x => x == "X")|| room.Game.Board[i].All(x => x == "O"))
                    room.Game.GameOver = true;
                
                if (room.Game.Board[0][i] == room.Game.Board[1][i] && room.Game.Board[2][i] == room.Game.Board[1][i] && room.Game.Board[0][i] != string.Empty)
                    room.Game.GameOver = true;
                

            }
            if (room.Game.Board[0][0] == room.Game.Board[1][1] && room.Game.Board[1][1] == room.Game.Board[2][2] && room.Game.Board[0][0] != string.Empty)
                room.Game.GameOver = true;

            if (room.Game.Board[0][2] == room.Game.Board[1][1] && room.Game.Board[1][1] == room.Game.Board[2][0] && room.Game.Board[0][2] != string.Empty)
                room.Game.GameOver = true;

            if (room.Game.GameOver)
            {
                room.Game.WinnerPlayerId = currentPlayerId;
            }
            else
            {
                if (room.Game.Board.All(r => r.All(c => c != string.Empty)))
                    room.Game.Draw = true;
                else
                {
                    room.Game.CurrentPlayerRoundId = currentPlayerId == room.Game.OPlayer ? room.Game.XPlayer: room.Game.OPlayer;
                }
            }
            await Clients.Group(roomId).ChangeMyRoomStatus(room);

        }


    }
}
