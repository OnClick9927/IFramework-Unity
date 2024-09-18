using IFramework.Record;
using UnityEngine;

public class RecordGame : MonoBehaviour
{
    public UnityEngine.UI.Button left, right, add5, muti2;
    public UnityEngine.UI.Text txt;
    static float count;
    Recorder recorder;
    private void view()
    {
        txt.text = count.ToString();
        left.interactable = recorder.CouldUndo();
        right.interactable = recorder.CouldRedo();
    }
    void Start()
    {
        recorder = new Recorder();
        view();
        left.onClick.AddListener(() =>
        {
            recorder.Undo();
            view();

        });
        right.onClick.AddListener(() =>
        {
            recorder.Redo();
            view();
        });
        add5.onClick.AddListener(() =>
        {
            recorder.AllocateCommand().SetCommand(new Add() { add = true }, new Add() { add = false }).Subscribe(true);
            view();
        });
        muti2.onClick.AddListener(() =>
        {
            recorder.AllocateCommand().SetCommand(new Muti() { muti = true }, new Muti() { muti = false }).Subscribe(true);
            view();
        });
    }
    private class Add : IRecorderActor
    {
        public bool add;
        public void Execute()
        {
            count += add ? 5 : -5;
        }
    }
    private class Muti : IRecorderActor
    {
        public bool muti;
        public void Execute()
        {
            count *= muti ? 2 : 0.5f;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
