using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace AutoFixture.AutoServiceCollection
{
    internal sealed class ISCWrapper : IServiceCollection
    {
        private readonly IServiceCollection _inner;
        private readonly IFixture _fixture;

        private static readonly object Lock = new object();

        public ISCWrapper(IServiceCollection inner, IFixture fixture)
        {
            _inner = inner;
            _fixture = fixture;
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_inner).GetEnumerator();
        }

        public void Add(ServiceDescriptor item)
        {
            lock (Lock)
            {
                _inner.Add(item);
                RegisterNewServiceProvider();
            }
        }

        private void RegisterNewServiceProvider()
        {
            _fixture.Eject<IServiceProvider>();
            var sp = this.BuildServiceProvider();
            _fixture.Inject(sp);
            _fixture
                .Customizations
                .OfType<ServiceCollectionConnector>()
                .ToList()
                .ForEach(x => x.ServiceProvider = sp);
        }

        public void Clear()
        {
            lock (Lock)
            {
                _inner.Clear();
                RegisterNewServiceProvider();
            }
        }

        public bool Contains(ServiceDescriptor item)
        {
            return _inner.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            lock (Lock)
            {
                var removed = _inner.Remove(item);
                RegisterNewServiceProvider();
                return removed;
            }
        }

        public int Count => _inner.Count;

        public bool IsReadOnly => _inner.IsReadOnly;

        public int IndexOf(ServiceDescriptor item)
        {
            return _inner.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            lock (Lock)
            {
                _inner.Insert(index, item);
                RegisterNewServiceProvider();
            }
        }

        public void RemoveAt(int index)
        {
            lock (Lock)
            {
                _inner.RemoveAt(index);
                RegisterNewServiceProvider();
            }
        }

        public ServiceDescriptor this[int index]
        {
            get => _inner[index];
            set
            {
                lock (Lock)
                {
                    _inner[index] = value;
                    RegisterNewServiceProvider();
                }
            }
        }
    }
}