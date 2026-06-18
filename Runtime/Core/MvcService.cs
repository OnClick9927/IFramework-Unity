/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
namespace IFramework
{
    interface IMCBase : IInjectAble
    {
        void Init();
        void Quit();
    }

    public class CtrlBase : IMCBase
    {
        void IMCBase.Init() => Init();
        void IMCBase.Quit() => Quit();
        protected virtual void Init() { }
        protected virtual void Quit() { }
    }
    public class ModelBase : IMCBase
    {
        public virtual void FreshRedPoints() { }
        void IMCBase.Init() => Init();
        void IMCBase.Quit() => Quit();
        protected virtual void Init() { }
        protected virtual void Quit() { }
    }

    class MvcService : GameServiceBase
    {
        public MvcService(IReadOnlyList<ModelBase> models, IReadOnlyList<CtrlBase> ctrls)
        {
            this.models = models;
            this.ctrls = ctrls;
        }

        protected override void OnUse(Game game)
        {
            this.models = models ?? new List<ModelBase>();
            this.ctrls = ctrls ?? new List<CtrlBase>();
            for (int i = 0; i < models.Count; i++)
            {
                var model = models[i];
                (model as IMCBase).Init();
                game.RegisterValue(model.GetType(), model, string.Empty);
            }
            for (int i = 0; i < ctrls.Count; i++)
            {
                var ctrl = ctrls[i];
                (ctrl as IMCBase).Init();
                game.RegisterValue(ctrl.GetType(), ctrl, string.Empty);
            }

            for (int i = 0; i < models.Count; i++) game.InjectFields(models[i]);
            for (int i = 0; i < ctrls.Count; i++) game.InjectFields(ctrls[i]);
        }
        protected override void OnQuit(Game game)
        {
            if (ctrls != null)
                for (int i = 0; i < ctrls.Count; i++) (ctrls[i] as IMCBase).Quit();
            if (models != null)
                for (int i = 0; i < models.Count; i++) (models[i] as IMCBase).Quit();
        }
        public IReadOnlyList<ModelBase> models { get; private set; }
        public IReadOnlyList<CtrlBase> ctrls { get; private set; }

    }
    public static class MvcServiceEx
    {
        public static Game UseMvc(this Game game, IReadOnlyList<ModelBase> models, IReadOnlyList<CtrlBase> ctrls)
        {
            MvcService service = new MvcService(models, ctrls);
            game.UseService(service);
            return game;
        }
        public static IReadOnlyList<ModelBase> GetModels(this Game game)
        {
            var service = game.GetService<MvcService>();
            return service?.models;
        }
        public static IReadOnlyList<CtrlBase> GetCtrls(this Game game)
        {
            var service = game.GetService<MvcService>();
            return service?.ctrls;
        }
        public static T GetCtrl<T>(this Game game) where T : CtrlBase
        {
            var service = game.GetValue<T>(string.Empty);
            return service;
        }
        public static T GetModel<T>(this Game game) where T : ModelBase
        {
            var service = game.GetValue<T>(string.Empty);
            return service;
        }
    }
}
