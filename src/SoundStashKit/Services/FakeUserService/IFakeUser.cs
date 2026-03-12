namespace SoundStashKit.Services.FakeUserService
{
    public class FakeUser : IFakeUser
    {
        public Guid UserId => Guid.Parse("11111111-1111-1111-1111-111111111111");
    }
}