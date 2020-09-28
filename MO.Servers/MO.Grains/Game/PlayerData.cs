using MO.GrainInterfaces.User;

namespace MO.Grains.Game
{
    public class PlayerData
    {
        private IUser _user;
        public double X { get; set; }
        public double Y { get; set; }
        public PlayerData(IUser user)
        {
            _user = user;
        }

        public void SetPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
