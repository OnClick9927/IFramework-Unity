/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework
{
    public class SingleLinkedNode<T>
    {
        public T data;
        public SingleLinkedNode<T> next;
        public SingleLinkedNode(T data)
        {
            this.data = data;
        }
    }
}
