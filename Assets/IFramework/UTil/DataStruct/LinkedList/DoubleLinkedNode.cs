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
	public class DoubleLinkedNode<T>
	{
        public T data;
        public DoubleLinkedNode<T> next;
        public DoubleLinkedNode<T> pre;

        public DoubleLinkedNode(T data)
        {
            this.data = data;
        }
    }
}
