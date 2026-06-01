/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace IFramework
{
    interface IMCBase: IInjectAble
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


    public interface IInjectAble { }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InjectAttribute : System.Attribute { }
    public interface IGameState : IInjectAble
    {
        void OnExit();
        void OnEnter();
        void Update();
        void Init();
    }


    public abstract class Game : MonoBehaviour
    {
        public Modules modules => _modules;
        public static Game Current { get { return Launcher.Instance.game; } }
        private ValueContainer values;
        Modules _modules;

        private void Awake()
        {
            values = new ValueContainer();
            _modules = new Modules();
            transform.SetParent(Launcher.Instance.transform);
            Launcher.Instance.game = this;
            BindUpdate(_Update);
            Startup();
        }





        private bool quited;
        public void Quit()
        {
            if (quited) return;
            quited = true;
            OnQuit();
            state = null;
            QuitMcs();
            UnBindUpdate(_Update);
            ((IDisposable)_modules).Dispose();
            values.Clear();
            _modules = null;
        }
        protected virtual void OnQuit() { }

        private void OnDestroy()
        {
            Quit();
        }
        private void _Update()
        {
            _state?.Update();
            _modules?.Update();
        }
        public IReadOnlyList<ModelBase> models { get; private set; }
        public IReadOnlyList<CtrlBase> ctrls { get; private set; }


        private void QuitMcs()
        {
            if (ctrls != null)
                for (int i = 0; i < ctrls.Count; i++) (ctrls[i] as IMCBase).Quit();
            if (models != null)
                for (int i = 0; i < models.Count; i++) (models[i] as IMCBase).Quit();
        }

        protected void InitModelsAndCtrls(IReadOnlyList<ModelBase> models, IReadOnlyList<CtrlBase> ctrls)
        {
            this.models = models ?? new List<ModelBase>();
            this.ctrls = ctrls ?? new List<CtrlBase>();
            for (int i = 0; i < models.Count; i++)
            {
                var model = models[i];
                (model as IMCBase).Init();
                RegisterValue(model.GetType(), model);
            }
            for (int i = 0; i < ctrls.Count; i++)
            {
                var ctrl = ctrls[i];
                (ctrl as IMCBase).Init();
                RegisterValue(ctrl.GetType(), ctrl);
            }

            for (int i = 0; i < models.Count; i++) InjectValues(models[i]);
            for (int i = 0; i < ctrls.Count; i++) InjectValues(ctrls[i]);
        }


        protected virtual void Startup()
        {
            var states = GetGameStates();
            if (states != null)
            {
                for (int i = 0; i < states.Count; i++)
                {
                    var state = states[i];
                    this.RegisterValue(state.GetType(), state);
                    state.Init();
                    InjectValues(state);
                }
                var _default = GetDefaultState();
                if (_default != null)
                    SwitchState(_default);
            }
        }

        protected virtual IReadOnlyList<IGameState> GetGameStates() => null;
        protected virtual IGameState GetDefaultState() => null;

        private IGameState _state;
        public IGameState state
        {
            get => _state; set
            {
                if (value == _state) return;
                _state?.OnExit();
                _state = value;
                _state?.OnEnter();
            }
        }
        //private Dictionary<string, IGameState> _states = new Dictionary<string, IGameState>();

        public bool SwitchState(Type type)
        {
            var _state = FindState(type);
            if (_state == null) return false;
            this.state = _state;
            return true;
        }
        public bool SwitchState<T>() => SwitchState(typeof(T));
        public bool SwitchState(IGameState state) => SwitchState(state.GetType());
        public IGameState FindState(Type type) => GetValue(type) as IGameState;
        public IGameState FindState<T>() where T : IGameState => FindState(typeof(T));






        public void RegisterValue(Type type, object instance) => values.RegisterInstance(type, instance);
        public object GetValue(Type type) => values.Get(type);
        public void InjectValues(object obj) => values.Inject(obj);

        public void RegisterValue<T>(T instance) where T : class => RegisterValue(typeof(T), instance);
        public T GetValue<T>() where T : class => GetValue(typeof(T)) as T;
        public void RegisterValueType<TBaseType, TType>() where TType : class, TBaseType, new() => values.RegisterType<TBaseType, TType>();
        public void RegisterValueType<TType>() where TType : class, new() => RegisterValueType<TType, TType>();



        private class ValueContainer
        {
            private Dictionary<Type, object> values = new Dictionary<Type, object>();
            private Dictionary<Type, Type> typeMap = new Dictionary<Type, Type>();

            public void RegisterType<TBaseType, TType>() where TType : TBaseType, new()
            {
                var typeBase = typeof(TBaseType);
                var type = typeof(TType);
                typeMap[typeBase] = type;
            }

            public object RegisterInstance(Type type, object instance)
            {
                values[type] = instance;
                return instance;
            }


            public object Get(Type type)
            {
                object result = null;
                if (values.TryGetValue(type, out result))
                    return result;
                Type subType = null;
                if (typeMap.TryGetValue(type, out subType))
                {
                    var ins = Activator.CreateInstance(subType);
                    RegisterInstance(type, ins);
                    Inject(ins);
                    return ins;
                }

                return null;
            }
            public void Clear() { values.Clear(); typeMap.Clear(); }
            static Dictionary<Type, List<FieldInfo>> fieldsMap = new Dictionary<Type, List<FieldInfo>>();
            public void Inject(object inject)
            {
                if (!(inject is IInjectAble))
                    return;
                var type = inject.GetType();
                List<FieldInfo> fields;
                if (!fieldsMap.TryGetValue(type, out fields))
                {
                    fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                .Where(x => x.IsDefined(typeof(InjectAttribute), false) &&
                                !x.IsInitOnly && !x.FieldType.IsValueType).ToList();

                    fieldsMap[type] = fields;
                }
                for (int i = 0; i < fields.Count; i++)
                {
                    var field = fields[i];
                    field.SetValue(inject, Get(field.FieldType));
                }

            }

        }


        public static void BindUpdate(Action action) => Launcher.BindUpdate(action);
        public static void UnBindUpdate(Action action) => Launcher.UnBindUpdate(action);
        public static void BindFixedUpdate(Action action) => Launcher.BindFixedUpdate(action);
        public static void UnBindFixedUpdate(Action action) => Launcher.UnBindFixedUpdate(action);

        public static void BindLateUpdate(Action action) => Launcher.BindLateUpdate(action);
        public static void UnBindLateUpdate(Action action) => Launcher.UnBindLateUpdate(action);
        public static void BindOnApplicationFocus(Action<bool> action) => Launcher.BindOnApplicationFocus(action);
        public static void UnBindOnApplicationFocus(Action<bool> action) => Launcher.UnBindOnApplicationFocus(action);
        public static void BindOnApplicationPause(Action<bool> action) => Launcher.BindOnApplicationPause(action);
        public static void UnBindOnApplicationPause(Action<bool> action) => Launcher.UnBindOnApplicationPause(action);
        public static void BindDisable(Action action) => Launcher.BindDisable(action);
        public static void UnBindDisable(Action action) => Launcher.UnBindDisable(action);


    }
}
