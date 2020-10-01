using MO.GrainInterfaces.User;

namespace MO.Grains.Game
{
    public class PlayerData
    {
        public IUser User { get; }
        public float X { get; set; }
        public float Y { get; set; }
        public PlayerData(IUser user)
        {
            User = user;
        }

        public void SetPoint(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
