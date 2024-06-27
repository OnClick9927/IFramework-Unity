namespace IFramework
{

    public struct ModulePriority
    {


        public const int Loom = 10;


        public const int Custom = 1000;
        private int _value;

        public ModulePriority(int value)
        {
            _value = value;
        }
   
        public int value { get { return _value; }set { _value = value; } }


        public static ModulePriority FromValue(int value)
        {
            return new ModulePriority(value);
        }

        public static implicit operator int(ModulePriority value)
        {
            return value.value;
        }

        public static implicit operator ModulePriority(int value)
        {
            return new ModulePriority(value);
        }

        public static ModulePriority operator +(ModulePriority a, ModulePriority b)
        {
            return new ModulePriority(a.value + b.value);
        }

        public static ModulePriority operator -(ModulePriority a, ModulePriority b)
        {
            return new ModulePriority(a.value - b.value);
        }
    }
}
