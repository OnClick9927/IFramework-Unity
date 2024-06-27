namespace IFramework
{
    interface IGenericPriorityQueueNode<TPriority>
    {

        TPriority priority { get; protected internal set; }

        int position { get; internal set; }


        long insertPosition { get; internal set; }

    }

}
