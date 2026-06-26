using UnityEngine;
using IFramework;
public class RecordGame : Game, IInjectAble
{
    public UnityEngine.UI.Button left, right, add5, muti2;
    public UnityEngine.UI.Text txt;
    static float count;
    [Inject] IUndoService undo;
    //Recorder recorder;
    private void view()
    {
        txt.text = count.ToString();
        left.interactable = this.Undo().CouldUndo();
        right.interactable = this.Undo().CouldRedo();
    }

    protected override void Startup()
    {
        this.UseValues();
        this.UseUndo();
        this.Values().Inject(this);
        view();
        left.onClick.AddListener(() =>
        {
            undo.Undo();
            view();

        });
        right.onClick.AddListener(() =>
        {
            undo.Redo();
            view();
        });
        add5.onClick.AddListener(() =>
        {
            undo.Subscribe<UndoRecord>((e) =>
            {
                e.SetValue(() => { count += 5; }, () => { count -= 5; });
            }, true);
            view();
        });
        muti2.onClick.AddListener(() =>
        {
            undo.Subscribe<UndoRecord>((e) =>
            {
                e.SetValue(() => { count *= 2; }, () => { count /= 2; });
            }, true);
            view();
        });
    }
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }
}
