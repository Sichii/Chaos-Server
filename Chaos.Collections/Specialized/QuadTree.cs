using System.Collections;
using Chaos.Collections.ObjectPool;
using Chaos.Extensions.Geometry;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Microsoft.Extensions.ObjectPool;

// ReSharper disable once CheckNamespace
namespace Chaos.Collections.Specialized;

/// <summary>
///     A data structure that repeatedly divides a 2D space into quadrants to performantly store and query items via
///     spatial means.
/// </summary>
/// <typeparam name="T">
///     An implementation of <see cref="IPoint" />
/// </typeparam>
public class QuadTree<T> : IEnumerable<T>, IResettable where T: IPoint
{
    private static QuadTreePool<T> Pool;

    // ReSharper disable once StaticMemberInGenericType
    #pragma warning disable CA2211
    /// <summary>
    ///     The maximum number of items a <see cref="QuadTree{T}" /> can hold before subdividing.
    /// </summary>

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public static int Capacity = 32;
    #pragma warning restore CA2211
    /// <summary>
    ///     The bounds of the <see cref="QuadTree{T}" />.
    /// </summary>
    protected IRectangle Bounds { get; set; }

    /// <summary>
    ///     The number of items in the <see cref="QuadTree{T}" /> or it's children.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    ///     Whether the <see cref="QuadTree{T}" /> has been divided into quadrants.
    /// </summary>
    protected bool Divided { get; set; }

    /// <summary>
    ///     The children of the <see cref="QuadTree{T}" />.
    /// </summary>
    protected QuadTree<T>?[] Children { get; }

    /// <summary>
    ///     The items in the <see cref="QuadTree{T}" />.
    /// </summary>
    protected List<T> Items { get; }

    static QuadTree() => Pool = new QuadTreePool<T>(short.MaxValue / 4);

    /// <summary>
    ///     Initializes a new instance of the <see cref="QuadTree{T}" /> class.
    /// </summary>
    /// <param name="bounds">
    /// </param>
    public QuadTree(IRectangle bounds)
    {
        Items = new List<T>(Capacity);
        Bounds = bounds;
        Children = new QuadTree<T>[4];
        Divided = false;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="QuadTree{T}" /> class.
    /// </summary>
    public QuadTree()
        : this(new Rectangle()) { }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator()
    {
        // ReSharper disable once ArrangeMethodOrOperatorBody
        return new QuadTreeEnumerator(this);

        /*if (Count > 0)
        {
            if (Divided)
                for (var i = 0; i < Children.Length; i++)
                {
                    var child = Children[i]!;

                    if (child.Count == 0)
                        continue;

                    foreach (var item in child)
                        yield return item;
                }
            else
                for (var i = 0; i < Items.Count; i++)
                    yield return Items[i];
        }*/
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool TryReset()
    {
        Clear();

        return true;
    }

    /// <summary>
    ///     Clears the <see cref="QuadTree{T}" /> of all items. Returns children to the pool recursively.
    /// </summary>
    public virtual void Clear()
    {
        Items.Clear();
        Count = 0;

        if (Divided)
        {
            for (var i = 0; i < Children.Length; i++)
            {
                var child = Children[i]!;
                Pool.Return(child);
                Children[i] = null;
            }

            Divided = false;
        }
    }

    /// <summary>
    ///     Adds an item to the <see cref="QuadTree{T}" />.
    /// </summary>
    /// <param name="item">
    ///     The item to add to the quadtree
    /// </param>
    protected virtual void InnerAdd(T item)
    {
        Items.Add(item);
        Count++;
    }

    /// <summary>
    ///     Removes an item from the <see cref="QuadTree{T}" />.
    /// </summary>
    /// <param name="item">
    ///     The item to remove from the quadtree
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the item was found and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    protected virtual bool InnerRemove(T item)
    {
        if (Items.Remove(item))
        {
            Count--;

            return true;
        }

        return false;
    }

    /// <summary>
    ///     Inserts an item into the <see cref="QuadTree{T}" /> if it is within the bounds of the quadtree.
    /// </summary>
    /// <param name="item">
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the item was inserted into the quadtree, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public virtual bool Insert(T item)
    {
        if (!Bounds.Contains(item))
            return false;

        if (!Divided && (Count >= Capacity) && Bounds is { Width: > 1, Height: > 1 })
            Subdivide();

        if (Divided)
        {
            for (var i = 0; i < Children.Length; i++)
            {
                var child = Children[i]!;

                if (child.Insert(item))
                {
                    Count++;

                    return true;
                }
            }

            return false;
        }

        InnerAdd(item);

        return true;
    }

    private void Merge()
    {
        for (var i1 = 0; i1 < Children.Length; i1++)
        {
            var child = Children[i1]!;

            for (var i2 = 0; i2 < child.Items.Count; i2++)
            {
                var item = child.Items[i2];

                Items.Add(item);
            }

            Pool.Return(child);
            Children[i1] = null;
        }

        Divided = false;
    }

    /// <summary>
    ///     Queries the <see cref="QuadTree{T}" /> for items within the specified bounds.
    /// </summary>
    /// <param name="bounds">
    ///     The query bounds
    /// </param>
    /// <returns>
    ///     All items contained within the specified bounds
    /// </returns>
    /// <remarks>
    ///     We use a custom iterator here to avoid a lot of overhead. By default, iterator blocks are converted into classes,
    ///     so they are allocated on the heap. The typical recursive iteration of a QuadTree would result in the allocation of
    ///     a new iterator every time we go depth+1 from current, which would result in hundreds of allocations every time we
    ///     query the tree. Instead, only one instance of the custom iterator is needed.
    /// </remarks>
    public virtual IEnumerable<T> Query(IRectangle bounds) => new QuadTreeRectangleQueryIterator(this, bounds);

    /// <summary>
    ///     Queries the <see cref="QuadTree{T}" /> for items within the specified bounds.
    /// </summary>
    /// <param name="bounds">
    ///     The query bounds
    /// </param>
    /// <returns>
    ///     All items contained within the specified bounds
    /// </returns>
    /// <remarks>
    ///     We use a custom iterator here to avoid a lot of overhead. By default, iterator blocks are converted into classes,
    ///     so they are allocated on the heap. The typical recursive iteration of a QuadTree would result in the allocation of
    ///     a new iterator every time we go depth+1 from current, which would result in hundreds of allocations every time we
    ///     query the tree. Instead, only one instance of the custom iterator is needed.
    /// </remarks>
    public virtual IEnumerable<T> Query(ICircle bounds) => new QuadTreeCircleQueryIterator(this, bounds);

    /// <summary>
    ///     Queries the <see cref="QuadTree{T}" /> for items at the specified point.
    /// </summary>
    /// <param name="point">
    ///     The point to query items at
    /// </param>
    /// <returns>
    ///     All items located at the specified point
    /// </returns>
    public virtual IEnumerable<T> Query(IPoint point)
    {
        if ((Count > 0) && Bounds.Contains(point))
        {
            if (Divided)
                for (var i = 0; i < Children.Length; i++)
                {
                    var child = Children[i]!;

                    if (child.Count == 0)
                        continue;

                    foreach (var item in child.Query(point))
                        yield return item;
                }
            else
                for (var i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];

                    if (point.Equals(item))
                        yield return item;
                }
        }
    }

    /// <summary>
    ///     Removes an item from the <see cref="QuadTree{T}" />.
    /// </summary>
    /// <param name="entity">
    ///     The item to remove
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the item was found and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public virtual bool Remove(T entity)
    {
        if ((Count == 0) || !Bounds.Contains(entity))
            return false;

        if (Divided)
        {
            for (var i = 0; i < Children.Length; i++)
            {
                var child = Children[i]!;

                if (child.Remove(entity))
                {
                    Count--;

                    if (Count < Capacity)
                        Merge();

                    return true;
                }
            }

            return false;
        }

        return InnerRemove(entity);
    }

    /// <summary>
    ///     Sets the bounds of the <see cref="QuadTree{T}" />.
    /// </summary>
    /// <param name="bounds">
    /// </param>
    public virtual void SetBounds(IRectangle bounds) => Bounds = bounds;

    /// <summary>
    ///     Sets the capacity of the <see cref="QuadTree{T}" /> pool.
    /// </summary>
    /// <param name="capacity">
    ///     A number specific to your application needs. The pool is shared amonst all QuadTree instances.
    /// </param>
    public static void SetPoolCapacity(int capacity) => Pool = new QuadTreePool<T>(capacity);

    private void Subdivide()
    {
        var x = Bounds.Left;
        var y = Bounds.Top;
        var hw = Bounds.Width / 2;
        var hh = Bounds.Height / 2;

        Children[0] = Pool.Get(
            new Rectangle(
                x,
                y,
                hw,
                hh));

        Children[1] = Pool.Get(
            new Rectangle(
                x + hw,
                y,
                Bounds.Width - hw,
                hh));

        Children[2] = Pool.Get(
            new Rectangle(
                x,
                y + hh,
                hw,
                Bounds.Height - hh));

        Children[3] = Pool.Get(
            new Rectangle(
                x + hw,
                y + hh,
                Bounds.Width - hw,
                Bounds.Height - hh));

        Divided = true;

        for (var i1 = 0; i1 < Items.Count; i1++)
        {
            var item = Items[i1];

            for (var i2 = 0; i2 < Children.Length; i2++)
            {
                var child = Children[i2]!;

                if (child.Insert(item))
                    break;
            }
        }

        Items.Clear();
    }

    #region Enumerators
    private struct QuadTreeEnumerator : IEnumerator<T>
    {
        private readonly QuadTree<T> Tree;
        private readonly Stack<QuadTree<T>> Stack;
        private int Index;

        public QuadTreeEnumerator(QuadTree<T> tree)
        {
            Tree = tree;
            Stack = new Stack<QuadTree<T>>();

            Stack.Push(Tree);
        }

        /// <inheritdoc />
        public readonly void Dispose() { }

        /// <inheritdoc />
        public bool MoveNext()
        {
            while (Stack.Count > 0)
            {
                var current = Stack.Peek();

                if (current.Count == 0)
                {
                    Stack.Pop();

                    continue;
                }

                if (current.Divided)
                {
                    Stack.Pop();

                    for (var i = 0; i < current.Children.Length; i++)
                    {
                        var child = current.Children[i]!;

                        if (child.Count == 0)
                            continue;

                        Stack.Push(child);
                    }

                    continue;
                }

                if (Index >= current.Items.Count)
                {
                    Index = 0;
                    Stack.Pop();

                    continue;
                }

                Current = current.Items[Index++];

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            Index = 0;
            Stack.Clear();
        }

        /// <inheritdoc />
        public T Current { get; private set; } = default!;

        /// <inheritdoc />
        readonly object IEnumerator.Current => Current;
    }

    /// <summary>
    ///     An iterator for querying a <see cref="QuadTree{T}" /> with a rectangle.
    /// </summary>
    /// <remarks>
    ///     Since the query method would need to utilize an iterator block anyway, having the IEnumerator also be an
    ///     IEnumerable saves the runtime from creating an iterator block class
    /// </remarks>
    private struct QuadTreeRectangleQueryIterator : IEnumerator<T>, IEnumerable<T>
    {
        private readonly QuadTree<T> Tree;
        private readonly Stack<QuadTree<T>> Stack;
        private readonly IRectangle Bounds;
        private int Index;

        public QuadTreeRectangleQueryIterator(QuadTree<T> tree, IRectangle bounds)
        {
            Tree = tree;
            Stack = new Stack<QuadTree<T>>();
            Bounds = bounds;

            Stack.Push(Tree);
        }

        /// <inheritdoc />
        public readonly void Dispose() { }

        /// <inheritdoc />
        public bool MoveNext()
        {
            //this is basically a depth-first search
            while (Stack.Count > 0)
            {
                var current = Stack.Peek();

                if (current.Count == 0)
                {
                    Stack.Pop();

                    continue;
                }

                if (current.Divided)
                {
                    Stack.Pop();

                    for (var i = 0; i < current.Children.Length; i++)
                    {
                        var child = current.Children[i]!;

                        //skip empty nodes, and nodes that don't intersect the circle
                        if ((child.Count == 0) || !child.Bounds.Intersects(Bounds))
                            continue;

                        Stack.Push(child);
                    }

                    continue;
                }

                //while searching the children of a node, only pop the node if we have searched all of it's children
                if (Index >= current.Items.Count)
                {
                    Index = 0;
                    Stack.Pop();

                    continue;
                }

                var item = current.Items[Index++];

                //if the item is within the bounds, return it (set it as the current item for the iterator)
                if (Bounds.Contains(item))
                {
                    Current = item;

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            Index = 0;
            Stack.Clear();
        }

        /// <inheritdoc />
        public T Current { get; private set; } = default!;

        /// <inheritdoc />
        readonly object IEnumerator.Current => Current;

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => this;

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    ///     An iterator for querying a <see cref="QuadTree{T}" /> with a circle.
    /// </summary>
    /// <remarks>
    ///     Since the query method would need to utilize an iterator block anyway, having the IEnumerator also be an
    ///     IEnumerable saves the runtime from creating an iterator block class
    /// </remarks>
    private struct QuadTreeCircleQueryIterator : IEnumerator<T>, IEnumerable<T>
    {
        private readonly QuadTree<T> Tree;
        private readonly Stack<QuadTree<T>> Stack;
        private readonly ICircle Bounds;
        private int Index;

        public QuadTreeCircleQueryIterator(QuadTree<T> tree, ICircle bounds)
        {
            Tree = tree;
            Stack = new Stack<QuadTree<T>>();
            Bounds = bounds;

            Stack.Push(Tree);
        }

        /// <inheritdoc />
        public readonly void Dispose() { }

        /// <inheritdoc />
        public bool MoveNext()
        {
            //this is basically a depth-first search
            while (Stack.Count > 0)
            {
                var current = Stack.Peek();

                if (current.Count == 0)
                {
                    Stack.Pop();

                    continue;
                }

                if (current.Divided)
                {
                    Stack.Pop();

                    for (var i = 0; i < current.Children.Length; i++)
                    {
                        var child = current.Children[i]!;

                        //skip empty nodes, and nodes that don't intersect the circle
                        if ((child.Count == 0) || !child.Bounds.Intersects(Bounds, DistanceType.Manhattan))
                            continue;

                        Stack.Push(child);
                    }

                    continue;
                }

                //while searching the children of a node, only pop the node if we have searched all of it's children
                if (Index >= current.Items.Count)
                {
                    Index = 0;
                    Stack.Pop();

                    continue;
                }

                var item = current.Items[Index++];

                //if the item is within the bounds, return it (set it as the current item for the iterator)
                if (Bounds.Contains(item, DistanceType.Manhattan))
                {
                    Current = item;

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public void Reset()
        {
            Index = 0;
            Stack.Clear();
        }

        /// <inheritdoc />
        public T Current { get; private set; } = default!;

        /// <inheritdoc />
        readonly object IEnumerator.Current => Current;

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => this;

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    #endregion
}