/*********************************************************************************
*Author:         OnClick
*Date:           2026-05-20
*********************************************************************************/
namespace IFramework.KK
{
    public class CubeView : GameObjectView, IPoolAbleGameObjectView
    {

        class View {
            //FieldsStart
        		public UnityEngine.GameObject Sphere;

            //FieldsEnd

            public View(CubeView context)
            {
                //InitComponentsStart
            			Sphere = context.GetGameObject("Sphere@sm");

                //InitComponentsEnd
            }
        }

        private View view;
        string IPoolAbleGameObjectView.PoolKey { get; set; }
        protected override void InitComponents()
        {
            view = new View(this);
        }


    }
}