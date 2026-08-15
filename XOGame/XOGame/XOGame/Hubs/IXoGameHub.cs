using XOGame.Shared;

namespace XOGame.Hubs
{
    public interface IXoGameHub
    {
        Task ShowRooms(List<Room> rooms);
        Task NewPlayerJoin(string playerName);
        Task ChangeMyRoomStatus(Room room);
    }
}
