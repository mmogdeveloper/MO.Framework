using MO.GrainInterfaces.User;

namespace MO.Grains.Game
{
    public class PlayerData
    {
        private IUser _user;
        public float X { get; set; }
        public float Y { get; set; }
        public PlayerData(IUser user)
        {
            _user = user;
        }

        public void SetPoint(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
