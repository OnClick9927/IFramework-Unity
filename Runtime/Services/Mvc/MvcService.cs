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

    class MvcService : ServiceBase
    {
        public MvcService(IReadOnlyList<ModelBase> models, IReadOnlyList<CtrlBase> ctrls)
        {
            this.models = models;
            this.ctrls = ctrls;
        }

        protected override void OnUse(IServiceCollection services)
        {
            this.models = models ?? new List<ModelBase>();
            this.ctrls = ctrls ?? new List<CtrlBase>();
            for (int i = 0; i < models.Count; i++)
            {
                var model = models[i];
                (model as IMCBase).Init();
                services.RegisterValue(model.GetType(), model, string.Empty);
            }
            for (int i = 0; i < ctrls.Count; i++)
            {
                var ctrl = ctrls[i];
                (ctrl as IMCBase).Init();
                services.RegisterValue(ctrl.GetType(), ctrl, string.Empty);
            }

            for (int i = 0; i < models.Count; i++) services.InjectFields(models[i]);
            for (int i = 0; i < ctrls.Count; i++) services.InjectFields(ctrls[i]);
        }
        protected override void OnQuit(IServiceCollection services)
        {
            if (ctrls != null)
                for (int i = 0; i < ctrls.Count; i++) (ctrls[i] as IMCBase).Quit();
            if (models != null)
                for (int i = 0; i < models.Count; i++) (models[i] as IMCBase).Quit();
        }
        public IReadOnlyList<ModelBase> models { get; private set; }
        public IReadOnlyList<CtrlBase> ctrls { get; private set; }

    }
}
