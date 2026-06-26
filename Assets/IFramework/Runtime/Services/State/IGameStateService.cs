/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    public interface IGameStateService:IService
    {
        IGameState state { get; set; }

        void ListenStateChange(GameStateChange call);
        void RemoveListenStateChange(GameStateChange call);
    }
}