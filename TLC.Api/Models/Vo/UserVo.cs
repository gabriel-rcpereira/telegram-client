namespace TLC.Api.Models.Vo
{
    public class UserVo
    {
        public int Id { get; set; }

        private UserVo() { }

        public class Builder
        {
            private UserVo _userVo = new UserVo();

            public Builder WithId(int id)
            {
                _userVo.Id = id;
                return this;
            }

            public UserVo Build()
            {
                return _userVo;
            }
        }
    }
}
