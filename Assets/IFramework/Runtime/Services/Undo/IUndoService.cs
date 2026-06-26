using System.Collections.Generic;

namespace IFramework
{
    public interface IUndoService:IService
    {
        void Clear();
        bool CouldRedo();
        bool CouldUndo();
        BaseUndoRecord GetCurrent();
        List<string> GetRecordNames(out int index);
        bool Redo();
        void Subscribe(BaseUndoRecord state, bool redo = true);
        bool Undo();
    }
}