namespace GDLibrary.Components
{
    public class PickupBehaviour : Behaviour
    {
        private string desc;
        private int value;
        public string Desc { get => desc; }
        public int Value { get => value; }

        public PickupBehaviour(string desc, int value)
        {
            this.desc = desc;
            this.value = value;
        }
    }
}