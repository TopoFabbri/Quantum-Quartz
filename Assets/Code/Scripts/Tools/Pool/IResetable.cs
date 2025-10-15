namespace Code.Scripts.Tools.Pool
{
    public interface IResetable
    {
        public void Reset();
        public void Assign(params object[] parameters);
    }
}