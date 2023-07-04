namespace IFramework.Recorder
{
    public partial class OperationRecorderModule
    {
        private class HeadState : BaseState
        {
            protected override void OnRedo() { }

            protected override void OnUndo() { }

            protected override void OnReset() { }

            public override object Clone()
            {
                return null;
            }
        }
    }
}
